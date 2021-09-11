using System;
using System.Threading;
using System.Threading.Tasks;
using Clima.Meadow.Pro.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Units;

namespace Clima.Meadow.Pro
{
    /// <summary>
    /// Basically combines all the sensors into one and enables the whole system
    /// to be read at once. Then it can go to sleep in between.
    ///
    /// ## Design considerations:
    ///
    /// we can probably get rid of the StartUpdating and StopUdpating stuff
    /// in favor of managing the lifecycle elsewhere for sleep purposes. but we may
    /// not need to, depending on how we design the sleep APIs
    ///
    /// </summary>
    public class ClimateMonitorAgent
    {
        //==== events
        /// <summary>
        /// Raised when a new climate reading has been taken. 
        /// </summary>
        public event EventHandler<ClimateConditions> ClimateConditionsUpdated = delegate { };

        //==== internals
        IF7MeadowDevice Device => MeadowApp.Device;
        protected object samplingLock = new object();
        protected CancellationTokenSource? SamplingTokenSource { get; set; }
        bool IsSampling { get; set; } = false;

        //==== peripherals
        II2cBus i2cBus;
        SwitchingAnemometer anemometer;
        WindVane windVane;
        Bme280 bme280;

        //==== properties
        /// <summary>
        /// The last read conditions.
        /// </summary>
        public Climate Climate { get; set; }

        //==== singleton stuff
        private static readonly Lazy<ClimateMonitorAgent> instance =
            new Lazy<ClimateMonitorAgent>(() => new ClimateMonitorAgent());
        public static ClimateMonitorAgent Instance {
            get { return instance.Value; }
        }

        // only invoked via the singleton instance 
        private ClimateMonitorAgent() {
            Initialize();
        }

        void Initialize()
        {
            Console.WriteLine("ClimateMonitor initializing.");

            //==== Anemometer
            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            //var anemometerObserver = SwitchingAnemometer.CreateObserver(
            //    handler: HandleAnemometerUpdate,
            //    //result => {
            //    //    Console.WriteLine($"new speed: {result.New:n2}, old: {result.Old:n2}");
            //    //},
            //    null
            //);
            //anemometer.Subscribe(anemometerObserver);
            Console.WriteLine("Anemometer up.");

            //==== WindVane
            windVane = new WindVane(Device, Device.Pins.A00);
            //var windVaneObserver = WindVane.CreateObserver(
            //    handler: result => { Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"); },
            //    filter: null
            //);
            //windVane.Subscribe(windVaneObserver);
            Console.WriteLine("WindVane up.");

            //==== I2C Bus
            i2cBus = Device.CreateI2cBus();
            Console.WriteLine("I2C up.");

            //==== BME280
            bme280 = new Bme280(i2cBus, Bme280.DEFAULT_ADDRESS);
            //var bmeObserver = Bme280.CreateObserver(
            //    handler: HandleBmeUpdate,
            //    filter: result => true);
            //bme280.Subscribe(bmeObserver);
            Console.WriteLine("BME280 up.");
            //ReadConditions().Wait();

            Console.WriteLine("ClimateMonitor initialized.");
        }

        public void StartUpdating(TimeSpan updateInterval)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            // thread safety
            lock (samplingLock) {
                if (IsSampling) return;

                Console.WriteLine("ClimateMonitorAgent.1");

                IsSampling = true;

                SamplingTokenSource = new CancellationTokenSource();
                CancellationToken ct = SamplingTokenSource.Token;

                Climate oldClimate;

                Console.WriteLine("ClimateMonitorAgent.2");


                Task.Factory.StartNew(async () => {
                    while (true) {

                        Console.WriteLine("ClimateMonitorAgent: About to do a reading.");

                        // cleanup
                        if (ct.IsCancellationRequested) {
                            // do task clean up here
                            //observers.ForEach(x => x.OnCompleted());
                            IsSampling = false;
                            break;
                        }

                        // capture history
                        oldClimate = Climate;

                        // read
                        Climate = await Read();

                        // build a new result with the old and new conditions
                        var result = new ClimateConditions(Climate, oldClimate);

                        Console.WriteLine("ClimateMonitorAgent: Reading complete.");

                        // let everyone know
                        RaiseEventsAndNotify(result);

                        // sleep for the appropriate interval
                        await Task.Delay(updateInterval);
                    }
                }, SamplingTokenSource.Token);
            }
        }

        /// <summary>
        /// Stops sampling the sensor.
        /// </summary>
        public virtual void StopUpdating()
        {
            lock (samplingLock) {
                if (!IsSampling) return;

                SamplingTokenSource?.Cancel();

                // state muh-cheen
                IsSampling = false;
            }
        }

        protected virtual void RaiseEventsAndNotify(ClimateConditions changeResult)
        {
            ClimateConditionsUpdated.Invoke(this, changeResult);
        }

        public virtual async Task<Climate> Read()
        {
            Console.WriteLine("ClimateMonitorAgent.Read()");

            // TODO: does it crash here??? or is it crashing somewhere else?

            // setup our initial climate stuff
            var climate = new Climate();

            Console.WriteLine("ClimateMonitorAgent.Read.1");
            climate.DateTime = DateTime.Now;

            Console.WriteLine("ClimateMonitorAgent.Read.2");

            //==== create all the read tasks
            //---- Bme280
            var bmeTask = new Task(async () => {
                Console.WriteLine("Reading from BME280");
                var bmeRead = await bme280.Read();

                Console.WriteLine("BME280 read finished.");

                climate.Humidity = bmeRead.Humidity;
                climate.Temperature = bmeRead.Temperature;
                climate.Pressure = bmeRead.Pressure;
            });
            //---- WindVane
            var windVaneTask = new Task(async () => {
                Console.WriteLine("Reading from windvane");
                var reading = await windVane.Read();

                Console.WriteLine("windvane read finished.");

                climate.WindDirection = reading;
            });

            Console.WriteLine("ClimateMonitorAgent.Read.3");

            //==== run them all 
            await Task.WhenAll(
                bmeTask,
                windVaneTask
                );

            Console.WriteLine("ClimateMonitorAgent: All reads finished.");

            return climate;
        }



        //protected void HandleAnemometerUpdate(IChangeResult<Speed> result)
        //{
        //    Console.WriteLine($"new speed: {result.New:n2}, old: {result.Old:n2}");
        //    // Null check
        //    ClimateConditions ??= new ClimateConditions();

        //    // save the last wind speed to the old
        //    if(ClimateConditions.Old is null) { ClimateConditions.Old = new Climate(); }
        //    ClimateConditions.Old.Windspeed = ClimateConditions.New?.Windspeed;

        //    // save the new
        //    //ClimateConditions.New

        //}

        //protected void HandleBmeUpdate(IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> result)
        //{
        //    Console.WriteLine($"Temp: {result.New.Temperature?.Fahrenheit:n2}, Humidity: {result.New.Humidity?.Percent:n2}%");

        //    // Null check
        //    ClimateConditions ??= new ClimateConditions();

        //    // temp
        //    if(result.New.Temperature is { } temp) {
        //        ClimateConditions.Old ??= new Climate();
        //        if(ClimateConditions.New != null) {
        //            ClimateConditions.Old.Temperature = ClimateConditions.New.Temperature;
        //        }
        //        ClimateConditions.New ??= new Climate();
        //        ClimateConditions.New.Temperature = temp;
        //    }

        //    // humidity
        //    if (result.New.Humidity is { } humidity) {
        //        ClimateConditions.Old ??= new Climate();
        //        if (ClimateConditions.New != null) {
        //            ClimateConditions.Old.Humidity = ClimateConditions.New.Humidity;
        //        }
        //        ClimateConditions.New ??= new Climate();
        //        ClimateConditions.New.Humidity = humidity;
        //    }

        //    // pressure
        //    if (result.New.Pressure is { } pressure) {
        //        ClimateConditions.Old ??= new Climate();
        //        if (ClimateConditions.New != null) {
        //            ClimateConditions.Old.Pressure = ClimateConditions.New.Pressure;
        //        }
        //        ClimateConditions.New ??= new Climate();
        //        ClimateConditions.New.Pressure = pressure;
        //    }

        //    this.RaiseClimateConditionsUpdated(ClimateConditions);
        //}

        //protected void RaiseClimateConditionsUpdated(ClimateConditions conditions)
        //{
        //    this.ClimateConditionsUpdated?.Invoke(this, ClimateConditions);
        //}
    }
}
