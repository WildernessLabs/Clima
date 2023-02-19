using CommonContracts.Models;
using Meadow.Foundation;
using Meadow.Foundation.Web.Maple;
using Meadow.Foundation.Web.Maple.Routing;
using MeadowClimaHackKit.Controller;
using MeadowClimaHackKit.Database;
using System.Collections.Generic;

namespace MeadowClimaHackKit.Connectivity
{
    public class MapleRequestHandler : RequestHandlerBase
    {

        private const int MAX_PAGE_SIZE = 50;

        public MapleRequestHandler() { }

        [HttpGet("/gettemperaturelogs")]
        public IActionResult GetTemperatureLogs()
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetAllTemperatureReadings();
            var data = MapDtoTemperatureModels(logs);

            LedController.Instance.SetColor(Color.Green);
            return new JsonResult(data);
        }

        [HttpGet("/gettemperaturepage/{page}")]
        public IActionResult GetTemperaturePage(int page)
        {
            LedController.Instance.SetColor(Color.Magenta);

            var logs = DatabaseManager.Instance.GetTemperatureReadings(MAX_PAGE_SIZE, page);

            var data = MapDtoTemperatureModels(logs);

            LedController.Instance.SetColor(Color.Green);
            return new JsonResult(data);
        }

        private static List<TemperatureModel> MapDtoTemperatureModels(List<TemperatureTable> logs)
        {
            var data = new List<TemperatureModel>();
            foreach (var log in logs)
            {
                data.Add(new TemperatureModel()
                {
                    Temperature = log.TemperatureCelcius?.ToString("00"),
                    DateTime = log.DateTime.ToString("yyyy-MM-dd hh:mm:ss tt"),
                });
            }

            return data;
        }
    }
}