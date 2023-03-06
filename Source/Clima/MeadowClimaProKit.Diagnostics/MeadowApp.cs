using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowClimaProKit.Diagnostics
{
    public class MeadowApp : App<F7FeatherV2>
    {
        bool IsSampling = false;
        object samplingLock = new object();
        CancellationTokenSource? SamplingTokenSource;

        Bme680? bme680;
        WindVane? windVane;
        RgbPwmLed? onboardLed;
        SwitchingRainGauge? rainGauge;
        SwitchingAnemometer? anemometer;
        IAnalogInputPort? solarVoltageInput;
        DiagnosticStatus? diagnosticStatus = new DiagnosticStatus();

        public override Task Initialize()
        {
            Console.WriteLine("Initialize started...");

            onboardLed = new RgbPwmLed(
                Device.Pins.OnboardLedRed,
                Device.Pins.OnboardLedGreen,
                Device.Pins.OnboardLedBlue);

            onboardLed.StartPulse(Color.Red);

            bme680 = new Bme680(Device.CreateI2cBus(), (byte)Bme680.Addresses.Address_0x76);
            Console.WriteLine("Bme680 successully initialized.");

            windVane = new WindVane(Device.Pins.A00);
            Console.WriteLine("WindVane up.");

            anemometer = new SwitchingAnemometer(Device.Pins.A01);
            anemometer.StartUpdating();
            Console.WriteLine("Anemometer up.");

            solarVoltageInput = Device.CreateAnalogInputPort(Device.Pins.A02);
            Console.WriteLine("Solar voltage input up.");

            rainGauge = new SwitchingRainGauge(Device.Pins.D11);
            rainGauge.StartUpdating();
            Console.WriteLine("Rain gauge up.");

            onboardLed.StartPulse(Color.Green);
            Console.WriteLine("Initialize completed!");

            StartUpdating(TimeSpan.FromSeconds(5));

            onboardLed.StartPulse(Color.Green);

            return Task.CompletedTask;
        }

        public async Task Read()
        {
            var bmeTask = bme680?.Read();
            var windVaneTask = windVane?.Read();
            var anemometerTask = anemometer?.Read();
            var rainFallTask = rainGauge?.Read();

            await Task.WhenAll(bmeTask, anemometerTask, windVaneTask, rainFallTask);

            Console.WriteLine($"");
            Console.WriteLine($"Date:       {DateTime.Now}");
            Console.WriteLine($"BME680:     {bmeTask?.Result.Temperature.Value.Fahrenheit:N2}°F");
            Console.WriteLine($"BME680:     {bmeTask?.Result.Pressure.Value.Pascal:N2}hPa");
            Console.WriteLine($"BME680:     {bmeTask?.Result.Humidity.Value.Percent:N2}%");
            Console.WriteLine($"RainGauge:  {rainFallTask?.Result}mm");
            Console.WriteLine($"WindVane:   {windVaneTask?.Result.Compass16PointCardinalName}");
            Console.WriteLine($"Anemometer: {anemometerTask?.Result.KilometersPerHour:N2}Kmph");
        }

        void StartUpdating(TimeSpan updateInterval)
        {
            Console.WriteLine("ClimateMonitorAgent.StartUpdating()");

            lock (samplingLock)
            {
                if (IsSampling)
                    return;
                IsSampling = true;

                SamplingTokenSource = new CancellationTokenSource();
                var ct = SamplingTokenSource.Token;

                Task.Run(async () =>
                {
                    while (true)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            IsSampling = false;
                            break;
                        }

                        await Read().ConfigureAwait(false);

                        await Task.Delay(updateInterval).ConfigureAwait(false);
                    }
                }, SamplingTokenSource.Token);
            }
        }

        public async override Task Run()
        {
            var solarVoltage = await solarVoltageInput.Read();
            Console.WriteLine($"Solar Voltage: {solarVoltage:n2}V");

            if (solarVoltage.Volts > 2)
            {
                diagnosticStatus.SolarWorking = true;
            }

            // write out our test status.
            if(diagnosticStatus.AllWorking)
            {
                Console.WriteLine("Success. Board is good.");
            }
            else
            {
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