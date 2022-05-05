using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTP_Server;

public class Server
{
    private Thread _serverThread = null!;
    private TcpListener _listener;

    public delegate Response ProcessRequestDelegate(string? request);
    public ProcessRequestDelegate ProcessRequest;

    public void Start(int port = 8888)
    {
        if (_serverThread != null) return;
        
        var ipAddress = new IPAddress(0);
        
        _listener = new TcpListener(ipAddress, port);
        _serverThread = new Thread(ServerHandler);
        _serverThread.Start();
    }

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
                    responseBuilder.AppendLine("HTTP/1.1 200 OK");
                    responseBuilder.AppendLine("Content-Type: application/json");
                    responseBuilder.AppendLine($"Content-Length: {response.Data.Length}");
                    responseBuilder.AppendLine();

                    var headerBytes = Encoding.UTF8.GetBytes(responseBuilder.ToString());
                
                    stream.Write(headerBytes, 0, headerBytes.Length);
                    stream.Write(response.Data, 0, response.Data.Length);
                }
            }
            finally
            {
                stream.Close();
                client.Close();
            }
            
        }
        
    }
    
    public static Response ProcessMessage(string request)
    {
        Console.Out.WriteLine($"Request: {request}");
        var response = new Response { MimeType = "application/json" };
        var responseData = Encoding.UTF8.GetBytes(request);
        response.Data = responseData;
        return response;
    }
}