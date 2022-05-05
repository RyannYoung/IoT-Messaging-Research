using System;
using System.Net;
using System.Threading.Channels;
using CoAP;
using CoAP.Server;


// Initial setup of request
var request = new Request(Method.GET) { URI = new Uri("coap://localhost/time") };

// Response Loop
var responsesRtt = new List<double>();

// Wait for the response

request.Send();
var response = request.WaitForResponse();
responsesRtt.Add(response.RTT);

var averageRtt = responsesRtt.Average();
Console.WriteLine($"Response: {response.PayloadSize},{averageRtt}");


