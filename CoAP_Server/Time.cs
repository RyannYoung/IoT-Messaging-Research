using System;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using CoAP.Server.Resources;

namespace CoAP
{
    class TimeResource : Server.Resources.Resource
    {
        private Timer _timer;
        private DateTime _now;

        public TimeResource(String name) : base(name)
        {
            Attributes.Title = "GET the current time";
            Attributes.AddResourceType("CurrentTime");
            Observable = true;

            _timer = new Timer(Timed, null, 0, 0);
        }

        private void Timed(Object o)
        {
            _now = DateTime.Now;
            Changed();
        }

        protected override void DoGet(CoapExchange exchange)
        {
            Dictionary<string, string> responseData = new Dictionary<string, string>();
            responseData.Add("data", DateTime.Now.ToString("o"));
            responseData.Add("type", "response");
            responseData.Add("target-user", "sample-user");

            var json = JsonSerializer.Serialize(responseData);

            Console.WriteLine(exchange.Request);
            exchange.Respond(StatusCode.Content, json, MediaType.ApplicationJson);
        }
    }
}