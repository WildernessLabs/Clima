using Meadow.Foundation;
using Meadow.Peripherals.Leds;
using Meadow.Foundation.Leds;
using Meadow.Hardware;

namespace Clima.Meadow.HackKit.Utils
{
    public static class LedIndicator
    {
        static RgbPwmLed led;

        static LedIndicator() { }

        public static void Initialize(IIODevice device, IPin redPin, IPin greenPin, IPin bluePin) 
        {
            led = new RgbPwmLed(device, redPin, greenPin, bluePin, 3.3f, 3.3f, 3.3f, IRgbLed.CommonType.CommonAnode);
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
