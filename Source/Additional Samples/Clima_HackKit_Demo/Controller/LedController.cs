﻿using Meadow.Foundation;
using Meadow.Foundation.Leds;
using System;
using Meadow;

namespace Clima_HackKit_Demo.Controller
{
    public class LedController
    {
        private static readonly Lazy<LedController> instance =
            new Lazy<LedController>(() => new LedController());
        public static LedController Instance => instance.Value;

        RgbPwmLed led;

        private LedController()
        {
            Initialize();
        }

        private void Initialize()
        {
            led = new RgbPwmLed(
                MeadowApp.Device.Pins.OnboardLedRed,
                MeadowApp.Device.Pins.OnboardLedGreen,
                MeadowApp.Device.Pins.OnboardLedBlue
            );
        }

        public void SetColor(Color color)
        {
            led.SetColor(color);
        }
    }
}