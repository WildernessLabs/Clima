using Clima.Meadow.HackKit;
using Clima.Meadow.HackKit.Utils;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;

namespace WildernessLabs.Clima.Meadow.HackKit.MapleRequestHandlers
{
    public class TemperatureRequestHandler : RequestHandlerBase
    {
        public TemperatureRequestHandler() { }

        [HttpGet]
        public void GetTemperature()
        {
            LedIndicator.SetColor(Color.Magenta); 

            var data = new 
            {
                Temperature = MeadowApp.Current.CurrentReading.Temperature.Celsius.ToString("##.#"),
                DateTime = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss tt")
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            Context.Response.StatusCode = 200;
            Send(data).Wait();

            LedIndicator.SetColor(Color.Green);
        }
    }
}