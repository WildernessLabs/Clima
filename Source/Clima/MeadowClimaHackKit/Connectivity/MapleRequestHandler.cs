using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using MeadowClimaHackKit.Controllers;
using MeadowClimaHackKit.Database;
using System.Collections.Generic;

namespace WildernessLabs.MeadowClimaHackKit.MapleRequestHandlers
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/gettemperature")]
        public IActionResult GetTemperature()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetTemperatureReadings();

            var data = new List<TemperatureModel>();
            foreach (var log in logs)
            {
                data.Add(new TemperatureModel()
                {
                    Temperature = log.TemperatureCelcius?.ToString("00"),
                    DateTime = log.DateTime
                });
            }

            LedController.Instance.SetColor(Color.Green);

            return new JsonResult(data);
        }
    }
}