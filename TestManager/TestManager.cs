using System.Reflection;
using System.Text.Json;
using Spectre.Console;

namespace TestManager;

public class TestManager
{
    /// <summary>
    /// NONE OF THIS WORKS
    /// <remarks>IT IS ALL BROKEN</remarks>
    /// </summary>

    private List<PacketData> _packets = new();
    private string _savePath = "";
    private int _iterations;
    string _fileoutput;
    public void RunApp()
    {
        AnsiConsole.Markup("[yellow]Common IOT Messaging Protocol Analysis Tool (v1)[/]\n");
        AnsiConsole.Markup("[yellow]Created by Ryan Young | u3188033[/]\n\n");

        var protocol = PromptProtocol();

        _iterations = AnsiConsole.Prompt(
            new TextPrompt<int>("[red][[Required]][/] How many packets to send?: "));
        _fileoutput = AnsiConsole.Prompt(
            new TextPrompt<string>("[grey][[Optional]][/] Full file path output (def: C:\\temp\\iot_protocol.json): ").AllowEmpty());

        if (!string.IsNullOrEmpty(_fileoutput))
            UpdateSavePath(_fileoutput);
        
        // run the appropriate protocol
        AnsiConsole.Markup($"Press <any key> to begin {protocol} test");
        Console.ReadKey();

        RunProtocol(protocol);

    }

    private void RunProtocol(Protocol protocol)
    {
        switch (protocol)
        {
            case Protocol.Http:
                RunHTTP();
                break;
            case Protocol.Mqtt:
                RunMQTT();
                break;
            case Protocol.Coap:
                RunCOAP();
                break;
            case Protocol.Amqp:
                RunAMQP();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(protocol), protocol, null);
        }
    }

    private void RunAMQP()
    {
        throw new NotImplementedException();
    }

    private void RunCOAP()
    {
        
    }

    private void RunMQTT()
    {
        throw new NotImplementedException();
    }

    private void RunHTTP()
    {
        throw new NotImplementedException();
    }


    enum Protocol
    {
        Http = 1,
        Mqtt = 2,
        Coap = 3,
        Amqp = 4
    }

    private Protocol PromptProtocol()
    {
        // Initialise the type of messaging protocol
        var answer = AnsiConsole.Prompt(
            new TextPrompt<int>("Protocol to test (HTTP=1, MQTT=2, COAP=3, AMQP=4): ")
                .ValidationErrorMessage("Invalid selection")
                .PromptStyle("green")
                .Validate(ans =>
                {
                    return ans switch
                    {
                        <= 0 => ValidationResult.Error("[red]Answer must be greater than zero[/]"),
                        > 4 => ValidationResult.Error("[red]Answer must be less than 4[/]"),
                        _ => ValidationResult.Success()
                    };
                }));

        var selectedProtocol = (Protocol) answer;
        AnsiConsole.Markup($"Selected protocol {selectedProtocol}");
        UpdateSavePath(selectedProtocol);

        return selectedProtocol;
    }

    private void UpdateSavePath(Protocol protocol)
    {
        _savePath = $@"C:\temp\iot_{protocol}.json";
        AnsiConsole.Markup($"\nSet save path to: {_savePath}\n");
    }

    private void UpdateSavePath(string path)
    {
        _savePath = path;
    }

    public Task SaveToFile()
    {
        var json = JsonSerializer.Serialize(_packets);
        File.WriteAllText(_savePath, json);

        var formatPath = new TextPath(_savePath)
            .LeafColor(Color.Green);
        
        AnsiConsole.WriteLine("\nOperation complete.");
        AnsiConsole.Write("Saved data to: ");
        AnsiConsole.Write(formatPath);
        
        return Task.CompletedTask;
    }

}