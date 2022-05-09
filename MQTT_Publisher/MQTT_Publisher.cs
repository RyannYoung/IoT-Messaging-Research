using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MQTT_Subscriber;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;
using Spectre.Console;


// RUN THE APPLICATION THROUGH MOSQUITTO

// The following code sends a request to the MQTT server.
// This application acts as both the publisher and the subscriber
// publisher > server > subscriber

// Recording values
var packets = new List<PacketData>();
var count = 0;

// mock data
var mockData = new Dictionary<string, string>();
mockData.Add("value", DateTime.UnixEpoch.ToString());
mockData.Add("mockAPIKey", "thisisamockapikey1234567890!@#$%^&*()");
mockData.Add("user", "iot_basic_user");
mockData.Add("publish", "date\\unixepoch\\");
mockData.Add("mac", "00:00:5e:00:53:af");
var payload = JsonSerializer.Serialize(mockData);

// Table config (for console output)
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

// runtime vars
int iterations;
string fileOutput;

bool canSendMessage = true;

DateTime startTime = default;
DateTime receivedTime;


//MQTT Setup
var mqttFactory = new MqttFactory();
MqttApplicationMessage applicationMessage = null;
using var mqttClient = mqttFactory.CreateMqttClient();
mqttClient.ConnectedAsync += e =>
{
    Console.WriteLine("Connected");
    return Task.CompletedTask;
};

mqttClient.DisconnectedAsync += e =>
{
    Console.WriteLine("Disconnected");
    return Task.CompletedTask;
};

mqttClient.ApplicationMessageReceivedAsync += e =>
{
    
    receivedTime = DateTime.Now;
    
    // formulate packet data information
    var time = DateTime.Now.ToString("H:mm:ss.fff");
    var sentPDU = applicationMessage.Payload.Length;
    var receivedPDU = e.ApplicationMessage.Payload.Length;
    var sentTime = SubtracttoMS(receivedTime, startTime);
    
    packets.Add(new PacketData(
        count,
        time,
        receivedPduSize: receivedPDU,
        sentPduSize: sentPDU,
        payload: payload,
        sentTimeMs: sentTime,
        ackTimeMs: -1,
        recTimeMs: -1
    ));

    canSendMessage = true;

    return Task.CompletedTask;
};


Init();
await Start(iterations);
SaveToFile();

void Init()
{
    Console.Clear();
    AnsiConsole.Markup("[red][yellow]MQTT Response Analysis Tool[/][/]\n");
    AnsiConsole.Markup("Created by Ryan Young u3188033\n\n");
    
    AnsiConsole.Markup("[bold]IMPORTANT! [/]Before proceeding ensure that an MQTT_SERVER (MOSQUITTO) \nis currently running and connected!\n\n");
    
    iterations = AnsiConsole.Prompt(new TextPrompt<int>(@"[red][[Required]][/] How many requests? ")
        .AllowEmpty());
    fileOutput = AnsiConsole.Prompt(new TextPrompt<string>(@"[grey][[Optional]][/] What is the full [green]filepath output[/] (def: C:\temp\iot_http_response.json)? ")
        .AllowEmpty());
}

async Task Start(int iterations = 5)
{
    await AnsiConsole.Live(table).StartAsync(async ctx =>
    {
        // connect
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1883)
            .WithClientId("IOT_DATA_TEST")
            .WithCleanSession()
            .Build();
        await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        // subscribe
        var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
            .WithTopicFilter(filter => { filter.WithTopic("IOT/data"); })
            .Build();
        await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

        // publish
        while (count < iterations)
        {
            startTime = DateTime.Now;

            if (canSendMessage)
            {
                canSendMessage = false;
                await PublishMessage();
            }

            while (!canSendMessage)
            {
                await Task.Delay(1);
            }

            var time = DateTime.Now.ToString("H:mm:ss.fff");
            var currPack = packets[count];

            table.AddRow(
                count.ToString(),
                time,
                currPack.sentPDUSize.ToString(),
                currPack.receivedPDUSize.ToString(),
                currPack.sentTimeMS.ToString(),
                "-1",
                "-1",
                payload);
            
            ctx.Refresh();

            
            
            // formulate packet information
            count++;
        }
    });
}

async Task PublishMessage()
{
    applicationMessage = new MqttApplicationMessageBuilder()
        .WithTopic("IOT/data")
        .WithPayload("100500700")
        .Build();
    
        await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
}

Console.WriteLine("Press any key to close the connection");
Console.ReadKey();

static TObject DumpToConsole<TObject>(TObject @object)
{
    var output = "NULL";
    if (@object != null)
    {
        output = JsonSerializer.Serialize(@object, new JsonSerializerOptions
        {
            WriteIndented = true
        });
    }
        
    Console.WriteLine($"[{@object?.GetType().Name}]:\r\n{output}");
    return @object;
}

void SaveToFile(string path = @"C:\temp\iot_mqtt_response.json")
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

static double SubtracttoMS(DateTime first, DateTime second)
{
    return first.Subtract(second).TotalMilliseconds;
}