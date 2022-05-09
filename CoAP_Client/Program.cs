using System.Diagnostics;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using CoAP;
using CoAP.Log;
using CoAP_Client;
using Spectre.Console;

// Recording values
var watch = new Stopwatch();
var packets = new List<PacketData>();
var count = 0;

// mock data to send
var values = new Dictionary<string, string>();
values.Add("value", DateTime.UnixEpoch.ToString());
values.Add("mockAPIKey", "thisisamockapikey1234567890!@#$%^&*()");
values.Add("user", "iot_basic_user");
values.Add("publish", "date\\unixepoch\\");
var payload = JsonSerializer.Serialize(values);


LogManager.Level = LogLevel.None;

var table = new Table()
    .AddColumn("id")
    .AddColumn("Time")
    .AddColumn("Sent PDU (bytes)")
    .AddColumn("Received PDU (bytes)")
    .AddColumn("Sent")
    .AddColumn("Acknowledged")
    .AddColumn("Received")
    .AddColumn("Payload Data")
    .Border(TableBorder.Heavy);

int iterations;
string fileOutput;

Init();
await Start(iterations);
SaveToFile();

void Init()
{
    Console.Clear();
    AnsiConsole.Markup("[red][yellow]COAP Request/Response Packet Analysis Tool[/][/]\n");
    AnsiConsole.Markup("Created by Ryan Young u3188033\n\n");
    
    AnsiConsole.Markup("[bold]IMPORTANT! [/]Before proceeding ensure that COAP_Server \nis currently running and connected!\n\n");
    
    iterations = AnsiConsole.Prompt(new TextPrompt<int>(@"[red][[Required]][/] How many requests? ")
        .AllowEmpty());
    fileOutput = AnsiConsole.Prompt(new TextPrompt<string>(@"[grey][[Optional]][/] What is the full [green]filepath output[/] (def: C:\temp\iot_http_response.json)? ")
        .AllowEmpty());
}

async Task Start(int iterations = 5)
{
    AnsiConsole.Markup("\n" +
                       $"Sending {iterations} requests to [blue]CoAP_Server[/]" +
                       "\n\n" + 
                       "[bold]Tabular Results:[/]\n");

    await AnsiConsole.Live(table).StartAsync(ctx =>
    {
        while (count < iterations)
        {
            
            // Client send a request to the client (Datetime.now)
            // Server receives and handles the requests (sending back their datetime.now)
            // Client gets a datetime.now on the time received.
            
            // Compare total time
            // Compare client time sent, to server time received (subtract)
            // Compare server time sent, to client time received (subtract)
            var startTime = DateTime.Now;

            // Create, configure and send a request to the server
            var request = new Request(Method.GET) {URI = new Uri("coap://localhost/time")};
            request.SetPayload(payload, MediaType.Any);
            request.Send();

            var response = request.WaitForResponse(); // response contains the datetime
            var receivedTime = DateTime.Now;

            var responsejson = JsonSerializer.Deserialize<Dictionary<string, string>>(response.ResponseText);
            var acknowledgeTime = DateTime.Parse(responsejson["data"]);
            
            
            // Formulate packet data information
            
            var time = DateTime.Now.ToString("H:mm:ss.fff");
            var sentPDU = request.Bytes.Length;
            var receivedPDU = response.PayloadString.Length; //todo: fix
            var sentTime = SubtracttoMS(receivedTime, startTime);
            var ackTime = SubtracttoMS(acknowledgeTime, startTime);
            var recTime = SubtracttoMS(receivedTime, acknowledgeTime);

            var rtt = response.RTT; 
            
            var size = response.PayloadSize;

            packets.Add(new PacketData(
                count,
                time, 
                receivedPduSize: receivedPDU,
                sentPduSize: sentPDU,
                payload: payload,
                sentTimeMs: sentTime,
                ackTimeMs: ackTime,
                recTimeMs: recTime
            ));

            table.AddRow(
                count.ToString(), 
                time,
                sentPDU.ToString(),
                receivedPDU.ToString(),
                sentTime.ToString(),
                ackTime.ToString(),
                recTime.ToString(),
                payload);
            
            ctx.Refresh();

            watch.Reset();
            count++;
        }

        return Task.CompletedTask;
    });
}


static double SubtracttoMS(DateTime first, DateTime second)
{
    return first.Subtract(second).TotalMilliseconds;
}

void SaveToFile(string path = @"C:\temp\iot_coap_response.json")
{
    var json = JsonSerializer.Serialize(packets);
    File.WriteAllText(path, json);

    var formatPath = new TextPath(path)
        .LeafColor(Color.Green);
    
    AnsiConsole.WriteLine("\nOperation complete.");
    AnsiConsole.Write("Saved data to: ");
    AnsiConsole.Write(formatPath);
    
    AnsiConsole.Write("\n\nPress any key to exit...");
    Console.ReadKey();
}


