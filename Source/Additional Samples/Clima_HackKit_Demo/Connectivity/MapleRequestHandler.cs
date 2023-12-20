using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using Clima_HackKit_Demo.Controller;
using Clima_HackKit_Demo.Database;
using System.Collections.Generic;
using Meadow;

namespace Clima_HackKit_Demo.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/gettemperaturelogs")]
        public IActionResult GetTemperatureLogs()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetTemperatureReadings();

            var data = new List<TemperatureModel>();
            foreach (var log in logs)
            {
                data.Add(new TemperatureModel()
                {
                    Temperature = log.TemperatureCelcius?.ToString("00"),
                    DateTime = log.DateTime.ToString("yyyy-MM-dd hh:mm:ss tt"),
                });
            }

            LedController.Instance.SetColor(Color.Green);
            return new JsonResult(data);
        }
    }
}