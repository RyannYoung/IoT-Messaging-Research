using System.Net;
using HTTP_Server;

var server = new Server { ProcessRequest = Server.ProcessMessage };
server.Start();
