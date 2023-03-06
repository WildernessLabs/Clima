using System;
using System.Threading;
using System.Threading.Tasks;
using MeadowClimaProKit.Models;
using Meadow;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using MeadowClimaProKit.Database;
using Meadow.Hardware;

namespace MeadowClimaProKit
{
    /// <summary>
    /// Basically combines all the sensors into one and enables the whole system
    /// to be read at once. Then it can go to sleep in between.
    ///
    /// ## Design considerations:
    ///
    /// we can probably get rid of the StartUpdating and StopUppating stuff
    /// in favor of managing the lifecycle elsewhere for sleep purposes. but we may
    /// not need to, depending on how we design the sleep APIs
    ///
    /// </summary>
    public class ClimateMonitorAgent
    {
        private static readonly Lazy<ClimateMonitorAgent> instance =
            new Lazy<ClimateMonitorAgent>(() => new ClimateMonitorAgent());
        public static ClimateMonitorAgent Instance => instance.Value;

        public event EventHandler<ClimateConditions> ClimateConditionsUpdated = delegate { };

        IF7MeadowDevice Device => MeadowApp.Device;
        bool IsSampling = false;

        Bme680? bme680;
        WindVane? windVane;
        SwitchingAnemometer? anemometer;
        SwitchingRainGauge? rainGauge;

        public ClimateReading? Climate { get; set; }

        private ClimateMonitorAgent() { }

        public void Initialize()
        {
            bme680 = new Bme680(Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
            Console.WriteLine("Bme680 successully initialized.");

            windVane = new WindVane(MeadowApp.Device.Pins.A00);
            Console.WriteLine("WindVane up.");

            anemometer = new SwitchingAnemometer(MeadowApp.Device.Pins.A01);
            anemometer.StartUpdating();
            Console.WriteLine("Anemometer up.");

            rainGauge = new SwitchingRainGauge(MeadowApp.Device.Pins.D11);
            rainGauge.StartUpdating();
            Console.WriteLine("Rain gauge up.");

            StartUpdating(TimeSpan.FromSeconds(30));
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
                        
                // capture history
                oldClimate = Climate ?? new ClimateReading();

                // read
                Climate = await Read();

                // build a new result with the old and new conditions
                var result = new ClimateConditions(Climate, oldClimate);

                Console.WriteLine("ClimateMonitorAgent: Reading complete.");
                DatabaseManager.Instance.SaveReading(result?.New);

                ClimateConditionsUpdated.Invoke(this, result);

                // sleep for the appropriate interval
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
            var bmeTask = bme680?.Read();
            var windVaneTask = windVane?.Read();
            var anemometerTask = anemometer?.Read();
            var rainFallTask = rainGauge?.Read();

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