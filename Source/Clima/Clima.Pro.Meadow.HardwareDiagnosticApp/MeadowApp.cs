using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Atmospheric;
using Meadow.Foundation.Sensors.Weather;
using Meadow.Hardware;

namespace MeadowApp
{
    // Change F7MicroV2 to F7Micro for V1.x boards
    public class MeadowApp : App<F7MicroV2, MeadowApp>
    {
        RgbPwmLed onboardLed;
        II2cBus? i2c;
        Bme680? bme680;
        Bme280? bme280;
        WindVane windVane;
        SwitchingAnemometer anemometer;

        public MeadowApp()
        {
            Initialize();
            //StartUpdating();
        }

        void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            Console.WriteLine("Initialize RGB Led");
            onboardLed = new RgbPwmLed(device: Device,
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                3.3f, 3.3f, 3.3f,
                Meadow.Peripherals.Leds.IRgbLed.CommonType.CommonAnode);

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

            // i2c
            try {
                Console.WriteLine("Instantiating I2C bus");
                i2c = Device.CreateI2cBus();
            } catch (Exception e) {
                Console.WriteLine("i2C failed bring-up.");
            }

            Console.WriteLine("Initialize Bme680");
            if(i2c != null) {
                try {
                    bme680 = new Bme680(i2c, (byte)Bme680.Addresses.Address_0x76);
                    Console.WriteLine("Bme680 successully initialized.");
                    var bmeObserver = Bme680.CreateObserver(
                        handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                        filter: result => true);
                    bme680.Subscribe(bmeObserver);
                } catch (Exception e) {
                    Console.WriteLine($"Bme680 failed bring-up: {e.Message}");
                }

                if (bme680 == null) {
                    Console.WriteLine("Trying it as a BME280.");
                    try {
                        bme280 = new Bme280(i2c, (byte)Bme280.Addresses.Address0);
                        Console.WriteLine("Bme280 successully initialized.");
                        var bmeObserver = Bme280.CreateObserver(
                            handler: result => Console.WriteLine($"Temp: {result.New.Temperature.Value.Fahrenheit:n2}, Humidity: {result.New.Humidity.Value.Percent:n2}%"),
                            filter: result => true);
                        bme280.Subscribe(bmeObserver);
                    } catch (Exception e2) {
                        Console.WriteLine($"Bme280 failed bring-up: {e2.Message}");
                    }
                }
            }


            if (bme680 != null || bme280 != null) {
                onboardLed.StartPulse(Color.Green);
                Console.WriteLine("Success. Board is good.");
            } else {
                onboardLed.SetColor(Color.Red);
                Console.WriteLine("Failure. Board is bad.");
            }
        }

        void StartUpdating()
        {
            bme680.StartUpdating();
            windVane.StartUpdating(TimeSpan.FromSeconds(1));
            anemometer.StartUpdating();
        }

    }
}
