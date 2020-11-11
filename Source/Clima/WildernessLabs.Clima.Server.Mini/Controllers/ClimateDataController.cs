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
            var item = new ClimateReading() {
                ID = 1,
                //TimeOfReading = DateTime.Now,
                TempC = 22,
                BarometricPressureMillibarHg = 200,
                RelativeHumdity = 0.5m
            };
            ClimateReadings.Add((long)item.ID, item);
        }

        public ClimateDataController()
        {
        }

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