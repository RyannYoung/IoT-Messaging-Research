using CoAP.Server.Resources;

namespace CoAP;

public class Resource : Server.Resources.Resource
{
    public Resource() : base("IOT")
    {
        Attributes.Title = "GET a friendly greeting!";
    }
    
    // override this method to handle GET requests
    protected override void DoGet(CoapExchange exchange)
    {
        // now we get a request, respond it
        exchange.Respond("Hello World!");
    }
}