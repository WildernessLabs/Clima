using MeadowHackKit;
using MeadowHackKit.Utils;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using System;

namespace WildernessLabs.MeadowHackKit.MapleRequestHandlers
{
    public class TemperatureRequestHandler : RequestHandlerBase
    {
        public TemperatureRequestHandler() { }

        [HttpGet]
        public void GetTemperature()
        {
            LedController.Instance.SetColor(Color.Magenta); 

            var data = new 
            {
                Temperature = MeadowApp.Current.CurrentReading.Temperature.Celsius.ToString("##.#"),
                DateTime = DateTime.Now.ToString("yyyy-mm-dd hh:mm:ss tt")
            };

            Context.Response.ContentType = ContentTypes.Application_Json;
            Context.Response.StatusCode = 200;
            Send(data).Wait();

            LedController.Instance.SetColor(Color.Green);
        }
    }
}