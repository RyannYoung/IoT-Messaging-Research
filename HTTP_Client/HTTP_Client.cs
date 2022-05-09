﻿using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using HTTP_Client;
using Spectre.Console;

// Recording values
var httpClient = new HttpClient();
var watch = new Stopwatch();
var packets = new List<PacketData>();
var count = 0;

// Mock data values
var values = new Dictionary<string, string>();
values.Add("value", DateTime.UnixEpoch.ToString());
values.Add("mockAPIKey", "thisisamockapikey1234567890!@#$%^&*()");
values.Add("user", "iot_basic_user");
values.Add("publish", "date\\unixepoch\\");

var content = new FormUrlEncodedContent(values);

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

            var startTime = DateTime.Now; // time of when the request was sent to server
            
            // Send and wait for the response
            content = new FormUrlEncodedContent(values);
            var response = await httpClient.PostAsync("http://localhost:8888/", content);
            var receivedTime = DateTime.Now; // time of when the response was received to client
            var responseString = await response.Content.ReadAsStringAsync();
            var responsejson = JsonSerializer.Deserialize<Dictionary<string, string>>(responseString);
            

            var acknowledgeTime= DateTime.Parse(responsejson["data"]);

            
            // Formulate packet data information
            var time = DateTime.Now.ToString("H:mm:ss.fff");
            var sentPduByteArray = await content.ReadAsByteArrayAsync();
            var sentPDU = sentPduByteArray.Length;
            var receivedPduByteArray = await response.Content.ReadAsByteArrayAsync();
            var receivedPDU = receivedPduByteArray.Length;//todo: fix
            var sentTime = SubtracttoMS(receivedTime, startTime);
            var ackTime = SubtracttoMS(acknowledgeTime, startTime);
            var recTime = SubtracttoMS(receivedTime, acknowledgeTime);
            var payload = JsonSerializer.Serialize(values);

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

            // Print to console;
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
        
            // Reset
            watch.Reset();
            count++;
        }
    });
    
}

static double SubtracttoMS(DateTime first, DateTime second)
{
    return first.Subtract(second).TotalMilliseconds;
}

void SaveToFile(string path = @"C:\temp\iot_http_response.json")
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