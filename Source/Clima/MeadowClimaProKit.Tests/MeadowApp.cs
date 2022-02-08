using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Units;
using System;

namespace MeadowClimaProKit.Tests
{
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        Bme680 bme680;
        WindVane windVane;
        RgbPwmLed onboardLed;
        SwitchingAnemometer anemometer;
        SwitchingRainGauge rainGauge;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize RGB Led");
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.SetColor(Color.Red);

            Console.WriteLine("Initialize SwitchingRainGauge");
            rainGauge = new SwitchingRainGauge(Device, Device.Pins.D15);
            var rainGaugeObserver = SwitchingRainGauge.CreateObserver(
                handler: result => Console.WriteLine($"Rain depth: {result.New.Millimeters}mm"),
                filter: null
            );
            rainGauge.Subscribe(rainGaugeObserver);
            rainGauge.StartUpdating();

            Console.WriteLine("Initialize SwitchingAnemometer");
            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            var anemometerObserver = SwitchingAnemometer.CreateObserver(
                handler: result => Console.WriteLine($"new speed: {result.New}, old: {result.Old}"),
                filter: null
            );
            anemometer.Subscribe(anemometerObserver);
            anemometer.StartUpdating();

            Console.WriteLine("Initialize WindVane");
            windVane = new WindVane(Device, Device.Pins.A00);
            var observer = WindVane.CreateObserver(
                handler: result => Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"),
                filter: null
            );
            windVane.Subscribe(observer);
            windVane.StartUpdating(TimeSpan.FromSeconds(1));

            Console.WriteLine("Initialize Bme680");
            bme680 = new Bme680(Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
            var bmeObserver = Bme680.CreateObserver(
                handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                filter: result => true);
            bme680.Subscribe(bmeObserver);
            bme680.StartUpdating();

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