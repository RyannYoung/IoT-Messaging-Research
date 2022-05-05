using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

var mqttFactory = new MqttFactory();

using var mqttClient = mqttFactory.CreateMqttClient();
var mqttClientOptions = new MqttClientOptionsBuilder()
    .WithTcpServer("localhost", 1883)
    .WithClientId("Subscriber")
    .WithCleanSession()
    .Build();

await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

var applicationMessage = new MqttApplicationMessageBuilder()
    .WithTopic("IOT/data")
    .WithPayload("19.5")
    .Build();

await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

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

Console.WriteLine("Press any key to close the connection");
Console.ReadKey();