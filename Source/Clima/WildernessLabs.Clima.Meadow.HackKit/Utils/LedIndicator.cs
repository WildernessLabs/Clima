using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace Clima.Meadow.HackKit.Utils
{
    public static class LedIndicator
    {
        static RgbPwmLed led;

        static LedIndicator() { }

        public static void Initialize()
        {
            led = new RgbPwmLed(
                MeadowApp.Device,
                MeadowApp.Device.Pins.OnboardLedRed,
                MeadowApp.Device.Pins.OnboardLedGreen,
                MeadowApp.Device.Pins.OnboardLedBlue
            );
        }

        public static void SetColor(Color color)
        {
            led.SetColor(color);
        }

        public static void StartBlink(Color color)
        {
            led.StartBlink(color);
        }

        public static void StartPulse(Color color)
        {
            led.StartPulse(color);
        }
    }
}