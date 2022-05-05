// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MQTTnet;
using MQTTnet.Protocol;
using MQTTnet.Server;


        
var mqttFactory = new MqttFactory();

var mqttServerOptions = new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .Build();

new MqttServerOptionsBuilder()
    .WithDefaultEndpoint()
    .WithDefaultEndpointPort(1234)
    .Build();

using var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
await mqttServer.StartAsync();

Console.WriteLine("Press Enter to exit.");
Console.ReadLine();

// Stop and dispose the MQTT server if it is no longer needed!
await mqttServer.StopAsync();