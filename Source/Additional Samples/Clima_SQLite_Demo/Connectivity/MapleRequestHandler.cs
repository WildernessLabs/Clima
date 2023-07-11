using CommonContracts.Models;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using Clima_SQLite_Demo.Database;
using System.Collections.Generic;

namespace Clima_SQLite_Demo.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/getclimalogs")]
        public IActionResult GetClimateLogs()
        {
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

            return new JsonResult(data);
        }
    }
}