using System;
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
    public class ClimateMonitor
    {
        //==== events
        public event EventHandler<ClimateConditions> ClimateConditionsUpdated = delegate { };

        //==== internals
        IMeadowDevice Device => MeadowApp.Device;

        //==== peripherals
        II2cBus i2cBus;
        SwitchingAnemometer anemometer;
        WindVane windVane;
        Bme280 bme280;

        //==== Properties
        public ClimateConditions ClimateConditions { get; set; }

        //==== singleton stuff
        private static readonly Lazy<ClimateMonitor> instance =
            new Lazy<ClimateMonitor>(() => new ClimateMonitor());
        public static ClimateMonitor Instance {
            get { return instance.Value; }
        }

        // only invoked via the singleton instance 
        private ClimateMonitor() {
            Initialize();

            // test
            bme280.StartUpdating();
            anemometer.StartUpdating(standbyDuration: 20000);
            windVane.StartUpdating(standbyDuration: 20000);

        }

        void Initialize()
        {
            Console.WriteLine("ClimateMonitor initializing.");

            //==== Anemometer
            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            var anemometerObserver = SwitchingAnemometer.CreateObserver(
                handler: result => {
                    Console.WriteLine($"new speed: {result.New:n2}, old: {result.Old:n2}");
                },
                null
            );
            anemometer.Subscribe(anemometerObserver);
            Console.WriteLine("Anemometer up.");

            //==== WindVane
            windVane = new WindVane(Device, Device.Pins.A00);
            var windVaneObserver = WindVane.CreateObserver(
                handler: result => { Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"); },
                filter: null
            );
            windVane.Subscribe(windVaneObserver);
            Console.WriteLine("WindVane up.");

            //==== I2C Bus
            i2cBus = Device.CreateI2cBus();
            Console.WriteLine("I2C up.");

            //==== BME280
            bme280 = new Bme280(i2cBus, Bme280.I2cAddress.Adddress0x76);
            var bmeObserver = Bme280.CreateObserver(
                handler: HandleBmeUpdate,
                filter: result => true);
            bme280.Subscribe(bmeObserver);
            Console.WriteLine("BME280 up.");
            ReadConditions().Wait();

            Console.WriteLine("ClimateMonitor initialized.");
        }

        protected async Task ReadConditions()
        {
            var conditions = await bme280.Read();
            Console.WriteLine("Initial Readings:");
            Console.WriteLine($"  Temperature: {conditions.Temperature?.Celsius}°C");
            Console.WriteLine($"  Pressure: {conditions.Pressure?.Pascal}hPa");
            Console.WriteLine($"  Relative Humidity: {conditions.Humidity}%");
        }

        protected void HandleBmeUpdate(IChangeResult<(Temperature? Temperature, RelativeHumidity? Humidity, Pressure? Pressure)> result)
        {
            Console.WriteLine($"Temp: {result.New.Temperature?.Fahrenheit:n2}, Humidity: {result.New.Humidity?.Percent:n2}%");

            // Null check
            ClimateConditions ??= new ClimateConditions();

            // temp
            if(result.New.Temperature is { } temp) {
                ClimateConditions.Old ??= new Climate();
                if(ClimateConditions.New != null) {
                    ClimateConditions.Old.Temperature = ClimateConditions.New.Temperature;
                }
                ClimateConditions.New ??= new Climate();
                ClimateConditions.New.Temperature = temp;
            }

            // humidity
            if (result.New.Humidity is { } humidity) {
                ClimateConditions.Old ??= new Climate();
                if (ClimateConditions.New != null) {
                    ClimateConditions.Old.Humidity = ClimateConditions.New.Humidity;
                }
                ClimateConditions.New ??= new Climate();
                ClimateConditions.New.Humidity = humidity;
            }

            // pressure
            if (result.New.Pressure is { } pressure) {
                ClimateConditions.Old ??= new Climate();
                if (ClimateConditions.New != null) {
                    ClimateConditions.Old.Pressure = ClimateConditions.New.Pressure;
                }
                ClimateConditions.New ??= new Climate();
                ClimateConditions.New.Pressure = pressure;
            }

            this.RaiseClimateConditionsUpdated(ClimateConditions);
        }

        protected void RaiseClimateConditionsUpdated(ClimateConditions conditions)
        {
            this.ClimateConditionsUpdated?.Invoke(this, ClimateConditions);
        }
    }
}
