using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using MeadowClimaProKit.Controller;
using MeadowClimaProKit.Database;
using System.Collections.Generic;

namespace MeadowClimaProKit.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/getclimalogs")]
        public IActionResult GetClimateLogs()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetAllClimateReadings();

            var data = new List<ClimateModel>();
            foreach (var log in logs)
            {
                data.Insert(0, new ClimateModel()
                {
                    Date = log.DateTime.ToString(),
                    Temperature = log.Temperature.ToString(),
                    Pressure = log.Pressure.ToString(),
                    Humidity = log.Humidity.ToString(),
                    WindDirection = log.WindDirection.ToString(),
                    WindSpeed = log.WindSpeed.ToString()
                });
            }

            LedController.Instance.SetColor(Color.Green);
            return new JsonResult(data);
        }
    }
}