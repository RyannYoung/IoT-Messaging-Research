using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using HTTP_Client;
using Spectre.Console;

var httpClient = new HttpClient();
var watch = new Stopwatch();
var packets = new List<PacketData>();
var count = 0;

var values = new Dictionary<string, string> { {"data", "1000"} };
var content = new FormUrlEncodedContent(values);

var table = new Table()
    .AddColumn("id")
    .AddColumn("Time")
    .AddColumn("RTT (ms)")
    .AddColumn("Size (bytes)")
    .Border(TableBorder.Heavy);

int iterations;
string fileOutput;

Init();
await Start(iterations);
SaveToFile();

void Init()
{
    AnsiConsole.Markup("[red][yellow]HTTP Response Packet Analysis Tool[/][/]\n");
    AnsiConsole.Markup("Created by Ryan Young u3188033\n\n");
    
    AnsiConsole.Markup("[bold]IMPORTANT! [/]Before proceeding ensure that HTTP_Server \nis currently running and connected to http://localhost:8888/\n\n");
    
    iterations = AnsiConsole.Prompt(new TextPrompt<int>(@"[red][[Required]][/] How many requests? ")
        .AllowEmpty());
    fileOutput = AnsiConsole.Prompt(new TextPrompt<string>(@"[grey][[Optional]][/] What is the full [green]filepath output[/] (def: C:\temp\iot_http_response.json)? ")
        .AllowEmpty());
}

async Task Start(int iterations = 5)
{
    AnsiConsole.Markup("\n" +
                       $"Sending {iterations} requests to [blue]http://localhost:8888/[/]" +
                       "\n\n" + 
                       "[bold]Tabular Results:[/]\n");
    
    await AnsiConsole.Live(table).StartAsync(async ctx =>
    {
        while (count < iterations)
        {
            watch.Start();

            // Get the response
            var response = await httpClient.PostAsync("http://localhost:8888/", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = responseString.Split('\n').Last().Split('=')[1];

            watch.Stop();

            // Get the packet data
            var rtt = watch.ElapsedMilliseconds;
            var countValue = count;
            var size = content.ToString().Length;
            var time = DateTime.Now.ToString("H:mm:ss.fff");
            
            packets.Add(new PacketData(time, count, rtt, size));

            // Print to console;
            table.AddRow(count.ToString(), time, rtt.ToString(), size.ToString());
            ctx.Refresh();
        
            // Reset
            watch.Reset();
            count++;
        }
    });
    
}

void SaveToFile(string path = @"C:\temp\iot_http_response.json")
{
    var json = JsonSerializer.Serialize(packets);
    File.WriteAllText(path, json);

    var formatPath = new TextPath(path)
        .LeafColor(Color.Green);


    AnsiConsole.WriteLine("Operation complete.");
    AnsiConsole.Write("Saved data to: ");
    AnsiConsole.Write(formatPath);
}