﻿using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace HTTP_Server;

public class Server
{
    // Setup params
    private Thread _serverThread = null!;
    private TcpListener _listener;

    // Callbacks to activate when a response is received
    public delegate Response ProcessRequestDelegate(string? request);
    public ProcessRequestDelegate ProcessRequest;

    
    /// <summary>
    /// Start the application, this sets up a server and runs the given IP
    /// </summary>
    /// <param name="port">The port utilised by the server</param>
    public void Start(int port = 6912)
    {
        if (_serverThread != null) return;
        
        var ipAddress = new IPAddress(0);
        
        _listener = new TcpListener(ipAddress, port);
        _serverThread = new Thread(ServerHandler);
        _serverThread.Start();
    }

    /// <summary>
    /// Reads the given request from the network stream
    /// </summary>
    /// <param name="stream">The network stream obtaining the request data</param>
    /// <returns></returns>
    private static string? ReadRequest(NetworkStream stream)
    {
        var contents = new MemoryStream();
        var buffer = new byte[2048];
        
        do
        {
            var size = stream.Read(buffer, 0, buffer.Length);
            if (size == 0)
            {
                return null;
            }
            contents.Write(buffer,0,size);
        } while (stream.DataAvailable);

        return Encoding.UTF8.GetString(contents.ToArray());
    }

    /// <summary>
    /// Run-time for the server
    /// </summary>
    private void ServerHandler(object o)
    {
        _listener.Start();
        Console.Out.WriteLine($"Starting server on... http://localhost:8888/");

        while (true)
        {
            var client = _listener.AcceptTcpClient();
            var stream = client.GetStream();

            try
            {
                var request = ReadRequest(stream);

                if (ProcessRequest != null)
                {
                    var response = ProcessRequest(request);
                    var responseBuilder = new StringBuilder();
                    responseBuilder.AppendLine("HTTP/1.1 200 OK" +
                                               "\nContent-Type: application/json" +
                                               "\nContent-Length: {response.Data.Length}\n");

                    var headerBytes = Encoding.UTF8.GetBytes(responseBuilder.ToString());
                
                    stream.Write(headerBytes, 0, headerBytes.Length);
                    stream.Write(response.Data, 0, response.Data.Length);
                }
            }
            finally
            {
                stream.Close();
            }
            
        }
        
    }
    
    /// <summary>
    /// Process the request
    /// </summary>
    /// <param name="request">the given request in string form</param>
    /// <returns></returns>
    public static Response ProcessMessage(string request)
    {
        
        Console.Out.WriteLine($"Request: {request}");
        var response = new Response { MimeType = "application/json" };

        // Construct the response payload
        var responsePayload = new Dictionary<string, string>();
        responsePayload.Add("data", DateTime.Now.ToString("o"));
        responsePayload.Add("type", "response");
        responsePayload.Add("target-user", "sample-user");
        
        // Serialise into json format
        var json = JsonSerializer.Serialize(responsePayload);

        // Encode and return the response object.
        var responseData = Encoding.UTF8.GetBytes(json);
        response.Data = responseData;
        return response;
    }
}