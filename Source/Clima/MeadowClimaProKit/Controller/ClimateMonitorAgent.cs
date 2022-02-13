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
        //==== singleton
        private static readonly Lazy<ClimateMonitorAgent> instance =
            new Lazy<ClimateMonitorAgent>(() => new ClimateMonitorAgent());
        public static ClimateMonitorAgent Instance => instance.Value;

        //==== internals
        IF7MeadowDevice Device => MeadowApp.Device;
        object samplingLock = new object();
        CancellationTokenSource? SamplingTokenSource;
        bool IsSampling = false;

        //==== peripherals
        II2cBus? i2c;
        Bme680? bme680;
        Bme280? bme280;
        WindVane? windVane;
        SwitchingAnemometer? anemometer;
        SwitchingRainGauge rainGauge;

        //==== properties
        /// <summary>
        /// The last read conditions.
        /// </summary>
        public ClimateReading? Climate { get; set; }

        private ClimateMonitorAgent() { }

        public void Initialize()
        {
            i2c = Device.CreateI2cBus();
            try
            {
                bme680 = new Bme680(i2c, (byte)Bme680.Addresses.Address_0x76);
                Console.WriteLine("Bme680 successully initialized.");
                var bmeObserver = Bme680.CreateObserver(
                    handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                    filter: result => true);
                bme680.Subscribe(bmeObserver);
            }
            catch (Exception e)
            {
                bme680 = null;
                Console.WriteLine($"Bme680 failed bring-up: {e.Message}");
            }

            if (bme680 == null)
            {
                Console.WriteLine("Trying it as a BME280.");
                try
                {
                    bme280 = new Bme280(i2c, (byte)Bme280.Addresses.Address0);
                    Console.WriteLine("Bme280 successully initialized.");
                    var bmeObserver = Bme280.CreateObserver(
                        handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                        filter: result => true);
                    bme280.Subscribe(bmeObserver);
                }
                catch (Exception e2)
                {
                    Console.WriteLine($"Bme280 failed bring-up: {e2.Message}");
                }
            }

            windVane = new WindVane(Device, Device.Pins.A00);
            Console.WriteLine("WindVane up.");

            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            anemometer.UpdateInterval = TimeSpan.FromSeconds(10);
            anemometer.StartUpdating();
            Console.WriteLine("Anemometer up.");

            rainGauge = new SwitchingRainGauge(Device, Device.Pins.D15);
            Console.WriteLine("Rain gauge up.");

            StartUpdating(TimeSpan.FromSeconds(30));
        }

        void StartUpdating(TimeSpan updateInterval)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            lock (samplingLock)
            {
                if (IsSampling) return;

                IsSampling = true;

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                ClimateReading oldClimate;

                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        Console.WriteLine("ClimateMonitorAgent: About to do a reading.");

                        // cleanup
                        if (ct.IsCancellationRequested)
                        {   // do task clean up here
                        //observers.ForEach(x => x.OnCompleted());
                        IsSampling = false;
                        break;
                        }

                        // capture history
                        oldClimate = Climate ?? new ClimateReading();

                        // read
                        Climate = await Read().ConfigureAwait(false);

                        // build a new result with the old and new conditions
                        var result = new ClimateConditions(Climate, oldClimate);

                        Console.WriteLine("ClimateMonitorAgent: Reading complete.");

                        // sleep for the appropriate interval
                        await Task.Delay(updateInterval).ConfigureAwait(false);
                    }
                }, SamplingTokenSource.Token);
            }
        }

        void StopUpdating()
        {
            if (!IsSampling) return;

            lock (samplingLock)
            {
                SamplingTokenSource?.Cancel();

                IsSampling = false;
            }
        }

        public async Task<ClimateReading> Read()
        {
            //==== create the read tasks
            var bmeTask = bme280?.Read();
            var windVaneTask = windVane?.Read(); 
 
            //==== await until all tasks complete 
            await Task.WhenAll(bmeTask, windVaneTask);

            var climate = new ClimateReading()
            {
                DateTime = DateTime.Now,
                Humidity = bmeTask?.Result.Humidity,
                Temperature = bmeTask?.Result.Temperature,
                Pressure = bmeTask?.Result.Pressure,
                WindDirection = windVaneTask?.Result,
                WindSpeed = anemometer?.WindSpeed,
            };

            return climate;
        }
    }
}