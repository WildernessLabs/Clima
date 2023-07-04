using Meadow.Devices;
using Clima_SQLite_Demo.Database;
using Clima_SQLite_Demo.Models;
using System;
using System.Threading.Tasks;

namespace Clima_SQLite_Demo
{
    public class ClimateMonitorAgent
    {
        private static readonly Lazy<ClimateMonitorAgent> instance =
            new Lazy<ClimateMonitorAgent>(() => new ClimateMonitorAgent());
        public static ClimateMonitorAgent Instance => instance.Value;

        public event EventHandler<ClimateConditions> ClimateConditionsUpdated = delegate { };

        IClimaHardware clima;

        bool IsSampling = false;

        public ClimateReading? Climate { get; set; }

        private ClimateMonitorAgent() { }

        public async Task Initialize()
        {
            clima = Meadow.Devices.Clima.Create();

            await StartUpdating(TimeSpan.FromSeconds(30));
        }

        async Task StartUpdating(TimeSpan updateInterval)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            if (IsSampling)
                return;
            IsSampling = true;

            ClimateReading oldClimate;

            while (IsSampling)
            {
                Console.WriteLine("ClimateMonitorAgent: About to do a reading.");

                oldClimate = Climate ?? new ClimateReading();

                Climate = await Read();

                var result = new ClimateConditions(Climate, oldClimate);

                Console.WriteLine("ClimateMonitorAgent: Reading complete.");
                DatabaseManager.Instance.SaveReading(result?.New);

                ClimateConditionsUpdated.Invoke(this, result);

                await Task.Delay(updateInterval).ConfigureAwait(false);
            }
        }

        void StopUpdating()
        {
            if (!IsSampling)
                return;

            IsSampling = false;
        }

        public async Task<ClimateReading> Read()
        {
            var bmeTask = clima.AtmosphericSensor?.Read();
            var windVaneTask = clima.WindVane?.Read();
            var anemometerTask = clima.Anemometer?.Read();
            var rainFallTask = clima.RainGauge?.Read();

            await Task.WhenAll(bmeTask, anemometerTask, windVaneTask, rainFallTask);

            var climate = new ClimateReading()
            {
                DateTime = DateTime.Now,
                Temperature = bmeTask?.Result.Temperature,
                Pressure = bmeTask?.Result.Pressure,
                Humidity = bmeTask?.Result.Humidity,
                RainFall = rainFallTask?.Result,
                WindDirection = windVaneTask?.Result.Compass16PointCardinalName,
                WindSpeed = anemometerTask?.Result,
            };

            return climate;
        }
    }
}