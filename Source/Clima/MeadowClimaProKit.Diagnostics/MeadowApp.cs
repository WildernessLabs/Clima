using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace MeadowClimaProKit.Diagnostics
{
    public class MeadowApp : App<F7FeatherV2>
    {
        Bme680? bme680;
        WindVane? windVane;
        RgbPwmLed? onboardLed;
        SwitchingAnemometer? anemometer;
        SwitchingRainGauge? rainGauge;
        IAnalogInputPort? solarVoltageInput;
        DiagnosticStatus? diagnosticStatus;

        public override Task Initialize()
        {
            diagnosticStatus = new DiagnosticStatus();

            Console.WriteLine("Initializing the solar voltage input");
            solarVoltageInput = Device.CreateAnalogInputPort(Device.Pins.A02);
            var solarVoltageObserver = IAnalogInputPort.CreateObserver(
                handler: result => Console.WriteLine($"Solar Voltage: {result.New}"),
                filter: null
            );
            solarVoltageInput.Subscribe(solarVoltageObserver);

            Console.WriteLine("Initialize RGB Led");
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue);
            onboardLed.StartPulse(Color.Red);

            Console.WriteLine("Initialize SwitchingRainGauge");
            rainGauge = new SwitchingRainGauge(Device, Device.Pins.D11);
            var rainGaugeObserver = SwitchingRainGauge.CreateObserver(
                handler: result => Console.WriteLine($"Rain depth: {result.New.Millimeters}mm"),
                filter: null
            );
            rainGauge.Subscribe(rainGaugeObserver);

            Console.WriteLine("Initialize SwitchingAnemometer");
            anemometer = new SwitchingAnemometer(Device, Device.Pins.A01);
            var anemometerObserver = SwitchingAnemometer.CreateObserver(
                handler: result => Console.WriteLine($"new speed: {result.New}, old: {result.Old}"),
                filter: null
            );
            anemometer.Subscribe(anemometerObserver);

            Console.WriteLine("Initialize WindVane");
            windVane = new WindVane(Device, Device.Pins.A00);
            var observer = WindVane.CreateObserver(
                handler: result => Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"),
                filter: null
            );
            windVane.Subscribe(observer);

            Console.WriteLine("Initialize BME680");
            bme680 = new Bme680(Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
            var bmeObserver = Bme680.CreateObserver(
                handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                filter: result => true);
            bme680.Subscribe(bmeObserver);

            onboardLed.StartPulse(Color.Green);

            return Task.CompletedTask;
        }

        void OutputWindSpeed(Speed windspeed)
        {
            // `0.0` - `10kmh`
            int r = (int)windspeed.KilometersPerHour.Map(0f, 10f, 0f, 255f);
            int b = (int)windspeed.KilometersPerHour.Map(0f, 10f, 255f, 0f);

            var wspeedColor = Color.FromRgb(r, 0, b);
            onboardLed.SetColor(wspeedColor);
        }

        public async override Task Run()
        {
            rainGauge?.StartUpdating();
            anemometer?.StartUpdating();
            windVane?.StartUpdating(TimeSpan.FromSeconds(1));
            bme680?.StartUpdating();

            var solarVoltage = await solarVoltageInput.Read();
            Console.WriteLine($"Solar Voltage: {solarVoltage:n2}V");

            if (solarVoltage.Volts > 2)
            {
                diagnosticStatus.SolarWorking = true;
            }

            // write out our test status.
            if(diagnosticStatus.AllWorking)
            {
                onboardLed.StartPulse(WildernessLabsColors.PearGreen);
                Console.WriteLine("Success. Board is good.");
            }
            else
            {
                onboardLed.StartPulse(WildernessLabsColors.ChileanFire);
                Console.WriteLine("Failure. Board is not good.");
                if (!diagnosticStatus.SolarWorking)
                {
                    Console.WriteLine("Solar voltage incorrect.");
                }
            }

            return;
        }
    }
}