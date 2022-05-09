using MQTTnet;
using MQTTnet.Client;

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost")
    .Build();

mqttClient.ApplicationMessageReceivedAsync += e =>
{
    Console.WriteLine("Received application message.");
    Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
    return Task.CompletedTask;
};

await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder()
    .WithTopicFilter(f => { f.WithTopic("IOT/data"); })
    .Build();

await mqttClient.SubscribeAsync(mqttSubscribeOptions, CancellationToken.None);

Console.WriteLine("MQTT client subscribed to topic.");

Console.WriteLine("Press enter to exit.");
Console.ReadLine();