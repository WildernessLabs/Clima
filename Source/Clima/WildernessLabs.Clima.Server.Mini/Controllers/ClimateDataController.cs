using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clima.Contracts.Models;

namespace Clima.Server.Mini.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClimateDataController : ControllerBase
    {
        static Dictionary<long, ClimateReading> ClimateReadings = new Dictionary<long, ClimateReading>();

        static ClimateDataController()
        {            
            ClimateReadings.Add(1, new ClimateReading() { ID = 1, TimeOfReading = new DateTime(2021, 02, 01, 14, 35, 25), TempC = 20, BarometricPressureMillibarHg = 200, RelativeHumdity = 0.5m });
            ClimateReadings.Add(2, new ClimateReading() { ID = 2, TimeOfReading = new DateTime(2021, 01, 31, 12, 45, 25), TempC = 22, BarometricPressureMillibarHg = 198, RelativeHumdity = 0.5m });
            ClimateReadings.Add(3, new ClimateReading() { ID = 3, TimeOfReading = new DateTime(2021, 01, 24, 07, 21, 25), TempC = 23, BarometricPressureMillibarHg = 201, RelativeHumdity = 0.5m });
            ClimateReadings.Add(4, new ClimateReading() { ID = 4, TimeOfReading = new DateTime(2021, 01, 22, 10, 33, 25), TempC = 20, BarometricPressureMillibarHg = 202, RelativeHumdity = 0.5m });
            ClimateReadings.Add(5, new ClimateReading() { ID = 5, TimeOfReading = new DateTime(2021, 01, 18, 12, 52, 25), TempC = 18, BarometricPressureMillibarHg = 197, RelativeHumdity = 0.5m });
            ClimateReadings.Add(6, new ClimateReading() { ID = 6, TimeOfReading = new DateTime(2021, 01, 17, 15, 18, 25), TempC = 19, BarometricPressureMillibarHg = 198, RelativeHumdity = 0.5m });
            ClimateReadings.Add(7, new ClimateReading() { ID = 7, TimeOfReading = new DateTime(2021, 01, 13, 20, 49, 25), TempC = 19, BarometricPressureMillibarHg = 203, RelativeHumdity = 0.5m });
            ClimateReadings.Add(8, new ClimateReading() { ID = 8, TimeOfReading = new DateTime(2021, 01, 08, 16, 26, 25), TempC = 20, BarometricPressureMillibarHg = 201, RelativeHumdity = 0.5m });
            ClimateReadings.Add(9, new ClimateReading() { ID = 9, TimeOfReading = new DateTime(2021, 01, 05, 19, 56, 25), TempC = 21, BarometricPressureMillibarHg = 199, RelativeHumdity = 0.5m });
        }

        public ClimateDataController() { }

        [HttpGet]
        public IActionResult Get()
        {
            ClimateReading[] readings = new ClimateReading[ClimateReadings.Keys.Count];
            ClimateReadings.Values.CopyTo(readings, 0);

            return new JsonResult(readings);
        }

        [HttpPost]
        public async Task<ActionResult<ClimateReading>> PostClimateReading(ClimateReading item)
        {
            // better way to handle nulls in C# 8 than what i'm doing here?
            if (item.ID.HasValue && ClimateReadings.ContainsKey(item.ID.Value)) {
                Conflict("Already exists");
            }

            // generate an ID
            item.ID = ClimateReadings.Count + 1;
            // save the item
            ClimateReadings.Add((long)(item?.ID), item);
            return CreatedAtAction(nameof(GetClimateReading), new { id = item.ID }, item);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClimateReading>> GetClimateReading(long id)
        {
            if (ClimateReadings.ContainsKey(id)) {
                return ClimateReadings[id];
            } else {
                return NotFound();
            }
        }
    }
}