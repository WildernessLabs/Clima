using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;
        IAnalogInputPort anemometer;
        IAnalogInputPort windVane;

        public MeadowApp()
        {
            Initialize();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // RGB onboard LED
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

            // init anemometer
            anemometer = Device.CreateAnalogInputPort(Device.Pins.A01);
            anemometer.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
                OutputWindSpeed,
                null //filter: result => (result.Delta > 0.01)
                ));
            // sample every half a second, and do automatic oversampling.
            anemometer.StartSampling(standbyDuration: 500);

            // init the windvane
            windVane = Device.CreateAnalogInputPort(Device.Pins.A00);
            windVane.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
                handler: result => {
                    Console.WriteLine($"WindVane voltage: {result.New}");
                },
                null
                ));
            // sample every half a second, and do automatic oversampling.
            windVane.StartSampling(standbyDuration: 500);

            Console.WriteLine("Initialization complete.");
        }

        void OutputWindSpeed(FloatChangeResult result)
        {
            //Console.WriteLine($"Anemometer voltage: {result.New}");

            // `0.0` - `0.2`
            int r = (int)Map(result.New, 0f, 0.2f, 0f, 255f);
            int b = (int)Map(result.New, 0f, 0.2f, 255f, 0f);

            //Console.WriteLine($"r: {r}, b: {b}");

            var wspeedColor = Color.FromRgb(r, 0, b);
            ShowColor(wspeedColor);
        }


        float Map( float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }

        void ShowColor(Color color)
        {
            onboardLed.SetColor(color);
        }
    }
}