using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Units;
using Meadow.Foundation.Sensors.Atmospheric;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;
        //IDigitalInputPort anemometer;
        SwitchingAnemometer anemometer;
        WindVane windVane;
        Bme280 bme280;
        II2cBus i2cBus;

        public MeadowApp()
        {
            Initialize();

            bme280.StartUpdating();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // RGB onboard LED
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);

            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            // classic event
            //anemometer.WindSpeedUpdated += (object sender, SwitchingAnemometer.AnemometerChangeResult e) => {
            //    Console.WriteLine($"new speed: {e.New}, old: {e.Old}");
            //};
            // iobservable
            SwitchingAnemometer.CreateObserver(
                handler: result => {
                    Console.WriteLine($"new speed: {result.New}, old: {result.Old}");
                },
                // only notify if it's change more than 0.1kmh:
                //filter: result => {
                //    Console.WriteLine($"delta: {result.Delta}");
                //    return result.Delta > 0.1;
                //    }
                null
            );

            anemometer.StartUpdating();

            //==== to test the analog port for the wind vane, uncomment this section
            //// init the windvane
            //windVaneAnalog = Device.CreateAnalogInputPort(Device.Pins.A00);
            //windVaneAnalog.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
            //    handler: result => {
            //        //Console.WriteLine($"WindVane voltage: {result.New}");
            //    },
            //    null
            //    ));
            //// sample every half a second, and do automatic oversampling.
            //windVaneAnalog.StartSampling(standbyDuration: 1000);

            ////==== to test the windvane driver use this
            //windVane = new WindVane(Device, Device.Pins.A00);
            //windVane.Subscribe(new FilterableChangeObserver<WindVane.WindVaneChangeResult, Azimuth>(
            //    handler: result => { Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"); },
            //    filter: null
            //));

            //// get initial reading, just to test the API
            //Azimuth azi = windVane.Read().Result;
            //Console.WriteLine($"Initial azimuth: {azi.Compass16PointCardinalName}");

            //windVane.StartUpdating();

            //==== I2C Bus
            i2cBus = Device.CreateI2cBus();
            Console.WriteLine("I2C up.");

            //==== BME280
            bme280 = new Bme280(i2cBus);
            var bmeObserver = Bme280.CreateObserver(
                handler: result => { Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"); },
                filter: result => true);
            bme280.Subscribe(bmeObserver);
            Console.WriteLine("BME280 up.");

            //ReadConditions().Wait();

            // done.
            Console.WriteLine("Initialization complete.");
        }

        void OutputWindSpeed(Speed windspeed)
        {
            // `0.0` - `10kmh`
            int r = (int)windspeed.KilometersPerHour.Map(0f, 10f, 0f, 255f);
            int b = (int)windspeed.KilometersPerHour.Map(0f, 10f, 255f, 0f);

            var wspeedColor = Color.FromRgb(r, 0, b);
            onboardLed.SetColor(wspeedColor);
        }
    }
}