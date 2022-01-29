using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple.Server;
using Meadow.Foundation.Web.Maple.Server.Routing;
using MeadowClimaProKit.Controller;
using MeadowClimaProKit.DataAccessLayer;
using System.Collections.Generic;

namespace MeadowClimaProKit.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {
        public MapleRequestHandler() { }

        [HttpGet("/getweatherlogs")]
        public IActionResult GetWeatherLogs()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetAllClimateReadings();

            var data = new List<WeatherModel>();
            foreach (var log in logs)
            {
                data.Add(new WeatherModel()
                {
                    ID = log.ID,
                    TimeOfReading = log.DateTime,
                    TempC = log.TemperatureValue,
                    BarometricPressureMillibarHg = log.PressureValue,
                    RelativeHumdity = log.HumidityValue
                });
            }

            LedController.Instance.SetColor(Color.Green);

            return new JsonResult(data);
        }
    }
}