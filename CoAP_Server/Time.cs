using System;
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

            _timer = new Timer(Timed, null, 0, 2000);
        }

        private void Timed(Object o)
        {
            _now = DateTime.Now;
            Changed();
        }

        protected override void DoGet(CoapExchange exchange)
        {
            exchange.Respond(StatusCode.Content, _now.ToString(), MediaType.TextPlain);
        }
    }
}