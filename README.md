# IoT-Messaging-Research
A performance comparison of popular messaging protocols used within IoT

> This application was developed as part of a research paper for the University of Canberra's - Internet of Thing's (IoT) unit

<img src="https://raw.githubusercontent.com/RyannYoung/IoT-Messaging-Research/master/Assets/output.gif?raw=true"/>

## Description
This Github repo contains multiple applications related to the configuration, and running of both server, client (incl. publisher/subscriber) applications all written in C# using a variety of popular libraries

## How to use
- Download the application, and run each of the projects in the temrinal. You may be required to config the selected ports, or include administrator privledges to allow running servers on your system. This will require you to manually edit the ports within the source code. 

### HTTP
- Run the HTTP Server project, you may need to configure the port <br/>
- Run the HTTP Client project, this should directly connect you to the server and then ask the run-time decisions

### CoAP
- Run the CoAP Server project to initialise the server <br/>
- Run the the CoAP clien to connect to the CoAP server.

### MQTT
- The simplest way to setup the client, and publisher would be to download MQTT Mosquitto, run the executable then
- Run MQTT Publisher. Note: This version acts as both the subscriber, and the publisher

### AMQP
- The simplest way would be to visit AMQP Lite .NET's Github Page and download the TestBroker.exe
- Run the test broker with the provided command
- Run the AMQP Publisher. Note: This client acts as both the subscriber, and the publisher

## Where is the data exported to
After completion of each program, all the data will be exported to <br/> 
`C:\temp`

## Special Thanks
Special thanks to the developers of the following libraries
[AMQPNET Lite](https://github.com/Azure/amqpnetlite)</br>
[CoAPNet](https://github.com/smeshlink/CoAP.NET)<br/>
[MQTT .Net](https://github.com/dotnet/MQTTnet)<br/>
<br/>
This code is free for use and reference

