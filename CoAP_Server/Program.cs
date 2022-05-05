using System;
using System.Net;
using CoAP.Server;

namespace CoAP
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var server = new CoapServer();
            server.Add(new Resource());
            server.Add(new TimeResource("time"));
            server.Start();
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}