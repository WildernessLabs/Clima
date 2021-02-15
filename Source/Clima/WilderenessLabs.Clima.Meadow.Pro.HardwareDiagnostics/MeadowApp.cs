using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Units;
using WilderenessLabs.Clima.Meadow;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;
        //IAnalogInputPort windVane;
        //IDigitalInputPort anemometer;
        Anemometer anemometer;

        WindVane windVane;

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

            //anemometer = Device.CreateDigitalInputPort(Device.Pins.A01, InterruptMode.EdgeFalling, ResistorMode.InternalPullUp, 20, 20);
            //anemometer.Subscribe(new FilterableChangeObserver<DigitalInputPortEventArgs, DateTime>(
            //    result => {
            //        Console.WriteLine($"wind event delta: {result.Delta}");
            //    },
            //    filter: null
            //    ));

            anemometer = new Anemometer(Device, Device.Pins.A01);
            // classic event
            //anemometer.SpeedUpdated += (object sender, Anemometer.AnemometerChangeResult e) => {
            //    Console.WriteLine($"new speed: {e.New}, old: {e.Old}");
            //};
            // iobservable
            anemometer.Subscribe(new FilterableChangeObserver<Anemometer.AnemometerChangeResult, float>(
                handler: result => {
                    Console.WriteLine($"new speed: {result.New}, old: {result.Old}");
                },
                // only notify if it's change more than 0.1kmh:
                //filter: result => {
                //    Console.WriteLine($"delta: {result.Delta}");
                //    return result.Delta > 0.1;
                //    }
                null
            ));

            //// init the windvane
            //windVane = Device.CreateAnalogInputPort(Device.Pins.A00);
            //windVane.Subscribe(new FilterableChangeObserver<FloatChangeResult, float>(
            //    handler: result => {
            //        //Console.WriteLine($"WindVane voltage: {result.New}");
            //    },
            //    null
            //    ));
            //// sample every half a second, and do automatic oversampling.
            //windVane.StartSampling(standbyDuration: 1000);

            windVane = new WindVane(Device, Device.Pins.A00);
            windVane.Subscribe(new FilterableChangeObserver<WindVane.WindVaneChangeResult, Azimuth>(
                handler: result => { Console.WriteLine($"Wind Direction: {result.New.Compass16PointCardinalName}"); },
                filter: null
            ));


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