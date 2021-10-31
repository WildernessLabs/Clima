using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Units;
using System;

namespace ClimaHardwareDiagnostics
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        Bme280 bme280;
        WindVane windVane;
        RgbPwmLed onboardLed;
        SwitchingAnemometer anemometer;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            var anemometerObserver = SwitchingAnemometer.CreateObserver(
                handler: result => { Console.WriteLine($"new speed: {result.New}, old: {result.Old}"); },
                filter: null
            );
            anemometer.Subscribe(anemometerObserver);
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
            windVane = new WindVane(Device, Device.Pins.A00);
            var observer = WindVane.CreateObserver(
                handler: result => { Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"); },
                filter: null
            );
            windVane.Subscribe(observer);
            windVane.StartUpdating(TimeSpan.FromSeconds(1));

            bme280 = new Bme280(Device.CreateI2cBus());
            var bmeObserver = Bme280.CreateObserver(
                handler: result => { Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"); },
                filter: result => true);
            bme280.Subscribe(bmeObserver);
            bme280.StartUpdating();

            onboardLed.SetColor(Color.Green);
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