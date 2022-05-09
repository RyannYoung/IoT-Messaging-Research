using CoAP;
using CoAP.Log;
using CoAP.Server;

namespace Coap_Communication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogManager.Level = LogLevel.None;
            
            var server = new CoapServer();
            server.Add(new Resource());
            server.Add(new TimeResource("time"));
            server.Start();
            
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}