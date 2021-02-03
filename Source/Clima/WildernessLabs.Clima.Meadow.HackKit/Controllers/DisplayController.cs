using System;
using System.IO;
using System.Reflection;
using Meadow.Foundation;
using Meadow.Foundation.Displays.Tft;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Atmospheric;
using SimpleJpegDecoder;

namespace Clima.Meadow.HackKit.Controllers
{
    public class DisplayController
    {
        // internals
        protected St7789 display;
        protected GraphicsLibrary graphics;
        protected AtmosphericConditions conditions;

        // rendering state and lock
        protected bool isRendering = false;
        protected object renderLock = new object();

        public DisplayController()
        {
            InitializeDisplay();
        }

        /// <summary>
        /// intializes the physical display peripheral, as well as the backing
        /// graphics library.
        /// </summary>
        protected void InitializeDisplay()
        {
            // our display needs mode3
            var config = new SpiClockConfiguration(6000, SpiClockConfiguration.Mode.Mode3);

            // new up the actual display on the SPI bus
            display = new St7789
            (
                device: MeadowApp.Device,
                spiBus: MeadowApp.Device.CreateSpiBus(MeadowApp.Device.Pins.SCK, MeadowApp.Device.Pins.MOSI, MeadowApp.Device.Pins.MISO, config),
                chipSelectPin: null,
                dcPin: MeadowApp.Device.Pins.D01,
                resetPin: MeadowApp.Device.Pins.D00,
                width: 240, height: 240
            );

            // create our graphics surface that we'll draw onto and then blit
            // to the display with.
            graphics = new GraphicsLibrary(display) {   // my display is upside down
                // Rotation = GraphicsLibrary.RotationType._180Degrees,
                CurrentFont = new Font12x20(),
            };

            Console.WriteLine("Clear display");

            // finally, clear the display so it's ready for action
            graphics.Clear(true);

            //Render();
        }

        public void ShowSplashScreen() 
        {
            graphics.Clear();

            graphics.Stroke = 1;
            graphics.DrawRectangle(0, 0, (int)display.Width, (int)display.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)display.Width - 10, (int)display.Height - 10, Color.White);

            graphics.DrawCircle((int)display.Width / 2, (int)display.Height / 2, (int)(display.Width / 2) - 10, Color.FromHex("#23abe3"), true);

            DisplayJPG();

            graphics.Show();
        }

        public void ShowTextLine1(string message) 
        {
            graphics.DrawRectangle(48, 130, 144, 71, Color.FromHex("#23abe3"), true);

            graphics.CurrentFont = new Font12x16();            
            graphics.DrawText(((int)display.Width - message.Length * 12) / 2, 139, message, Color.Black);

            graphics.Show();
        }

        public void ShowTextLine2(string message)
        {
            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(((int)display.Width - message.Length * 12) / 2, 169, message, Color.Black);

            graphics.Show();
        }

        public void UpdateDisplay(AtmosphericConditions conditions) {
            this.conditions = conditions;
            this.Render();  
        }

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            Console.WriteLine($"Render() - is rendering: {isRendering}");

            lock (renderLock) {   // if we're already rendering, bail out.
                if (isRendering) {
                    Console.WriteLine("Already in a rendering loop, bailing out.");
                    return;
                }

                isRendering = true;
            }

            graphics.Clear(true);

            graphics.Stroke = 1;
            graphics.DrawRectangle(0, 0, (int)display.Width, (int)display.Height, Color.White);
            graphics.DrawRectangle(5, 5, (int)display.Width - 10, (int)display.Height - 10, Color.White);

            graphics.DrawCircle((int)display.Width / 2, (int)display.Height / 2, (int)(display.Width / 2) - 10, Color.FromHex("#23abe3"), true);

            DisplayJPG();

            string text = $"{conditions.Temperature?.ToString("##.#")}°C";

            graphics.CurrentFont = new Font12x20();
            graphics.DrawText(
                x: (int)(display.Width - text.Length * 24) / 2, 
                y: 140, 
                text: text, 
                color: Color.Black, 
                scaleFactor: GraphicsLibrary.ScaleFactor.X2);

            graphics.Rotation = GraphicsLibrary.RotationType._270Degrees;

            graphics.Show();

            Console.WriteLine("Show complete");

            isRendering = false;

        }

        protected void DisplayJPG()
        {
            var jpgData = LoadResource("meadow.jpg");
            var decoder = new JpegDecoder();
            var jpg = decoder.DecodeJpeg(jpgData);

            int x = 0;
            int y = 0;
            byte r, g, b;

            for (int i = 0; i < jpg.Length; i += 3)
            {
                r = jpg[i];
                g = jpg[i + 1];
                b = jpg[i + 2];

                graphics.DrawPixel(x + 55, y + 40, Color.FromRgb(r, g, b));

                x++;
                if (x % decoder.Width == 0)
                {
                    y++;
                    x = 0;
                }
            }

            display.Show();
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"Clima.Meadow.HackKit.{filename}";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var ms = new MemoryStream())
                {
                    stream.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
