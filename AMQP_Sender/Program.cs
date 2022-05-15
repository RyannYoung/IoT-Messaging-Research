using System.Runtime.CompilerServices;
using System.Text.Json;
using Amqp;
using AMQP_Client;
using Spectre.Console;

// Setup params
var address = new Address("amqp://guest:guest@localhost:5672");
var connection = await Connection.Factory.CreateAsync(address);
var session = new Session(connection);

// You'll need an AMQP broker to run this evaluation, a sample can be found on the AMQP Github page
// TestAmqpBroker.exe amqp://localhost:5672 /creds:guest:guest /queues:q1

// Recording params
var packets = new List<PacketData>();
var count = 0;

// mock data to send
var mockData = new Dictionary<string, string>();
mockData.Add("value", DateTime.UnixEpoch.ToString());
mockData.Add("mockAPIKey", "thisisamockapikey1234567890!@#$%^&*()");
mockData.Add("user", "iot_basic_user");
mockData.Add("mac", "00:00:5e:00:53:af");
mockData.Add("publish", "date\\unixepoch\\");
var payload = JsonSerializer.Serialize(mockData);

// table configuration for console
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

// runtime varialbes
int iterations;
string fileOutput;

// AMQP setup
SenderLink sender = null;
ReceiverLink receiver = null;
Message sentMessage;
Message receivedMessage = null;

Init();
await Start();
SaveToFile();

void Init()
{
    Console.Clear();
    AnsiConsole.Markup("[red][yellow]AMQP Request/Response Packet Analysis Tool[/][/]\n");
    AnsiConsole.Markup("Created by Ryan Young u3188033\n\n");
    
    AnsiConsole.Markup("[bold]IMPORTANT! [/]Before proceeding ensure that you have an AMQP Server currently running \nis currently running and connected!\n");
    AnsiConsole.Markup("\nFor support with this see: http://azure.github.io/amqpnetlite/articles/hello_amqp.html");
    AnsiConsole.Markup("\nRun through terminal windows: TestAmqpBroker.exe amqp://localhost:5672 /creds:guest:guest /queues:q1\n\n");
    
    iterations = AnsiConsole.Prompt(new TextPrompt<int>(@"[red][[Required]][/] How many requests? ")
        .AllowEmpty());
    fileOutput = AnsiConsole.Prompt(new TextPrompt<string>(@"[grey][[Optional]][/] What is the full [green]filepath output[/] (def: C:\temp\iot_http_response.json)? ")
        .AllowEmpty());
}



async Task Start()
{
    AnsiConsole.Markup("\n" +
                       $"Sending {iterations} requests to [blue]AMQP_Server[/]" +
                       "\n\n" + 
                       "[bold]Tabular Results:[/]\n");

    await AnsiConsole.Live(table).StartAsync(async ctx =>
    {
        sentMessage = new Message(payload);
        sender = new SenderLink(session, "sender-link", "q1");
        receiver = new ReceiverLink(session, "receiver-link", "q1");
        while (count < iterations)
        {
            var startTime = DateTime.Now;
            await SendMessage(); // send a message
            await ReceiveMessage(); // receive a message
            var receivedTime = DateTime.Now;

            Console.WriteLine();

            // formulate packet data information
            var time = DateTime.Now.ToString("H:mm:ss.fff");
            var sentPDU = sentMessage.GetEstimatedMessageSize();
            var receivedPDU = receivedMessage.GetEstimatedMessageSize();
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

            table.AddRow(
                count.ToString(),
                time,
                sentPDU.ToString(),
                receivedPDU.ToString(),
                sentTime.ToString(),
                "-1",
                "-1",
                payload);
            
            ctx.Refresh();
            
            count++;
        }
    });

}

async Task SendMessage()
{
    // Send a message
    await sender.SendAsync(sentMessage);
}

async Task ReceiveMessage()
{
    // Receive a message
    receivedMessage = await receiver.ReceiveAsync();
    receiver.Accept(receivedMessage);
}

void SaveToFile(string path = @"C:\temp\iot_ampq_response.json")
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