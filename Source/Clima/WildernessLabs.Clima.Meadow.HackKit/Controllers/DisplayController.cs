using Meadow.Foundation;
using Meadow.Foundation.Displays.TextDisplayMenu;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Hardware;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;

namespace Clima.Meadow.HackKit.Controllers
{
    public class DisplayController
    {
        protected Temperature conditions;

        protected St7789 display;
        protected GraphicsLibrary graphics;
        protected Menu menu;
        protected BufferRgb888 logo;

        protected bool isCelcius = true;
        protected bool isRendering = false;

        public DisplayController()
        {
            Initialize();
        }

        /// <summary>
        /// intializes the physical display peripheral, as well as the backing
        /// graphics library.
        /// </summary>
        protected void Initialize()
        {
            // our display needs mode3
            var config = new SpiClockConfiguration(24000, SpiClockConfiguration.Mode.Mode3);
            var spiBus = MeadowApp.Device.CreateSpiBus(MeadowApp.Device.Pins.SCK, MeadowApp.Device.Pins.MOSI, MeadowApp.Device.Pins.MISO, config);
            // new up the actual display on the SPI bus
            display = new St7789
            (
                device: MeadowApp.Device,
                spiBus: spiBus,
                chipSelectPin: null,
                dcPin: MeadowApp.Device.Pins.D01,
                resetPin: MeadowApp.Device.Pins.D00,
                width: 240, height: 240
            );

            // create our graphics surface that we'll draw onto and then blit to the display
            graphics = new GraphicsLibrary(display) 
            {   
                CurrentFont = new Font12x20(),
            };
            graphics.DisplayConfig.FontScale = 2;

            //create the menu for TextDisplayMenu
            var menuItems = new MenuItem[]
            {
                new MenuItem("Celcius", command: "setCelcius"),
                new MenuItem("Fahrenheit", command: "setFahrenheit"),
            };

            menu = new Menu(graphics, menuItems, false);
            menu.Selected += Menu_Selected;

            Console.WriteLine("Clear display");

            // finally, clear the display so it's ready for action
            graphics.Clear(true);

            //and load the logo jpg into a buffer
            LoadJpeg();
        }

        private void Menu_Selected(object sender, MenuSelectedEventArgs e)
        {
            Console.WriteLine("MenuSelected: " + e.Command);

            isCelcius = (e.Command == "setCelcius");
               
            //hide the menu after a selection
            menu.Disable();
            //and update the display
            Render();
        }

        public void ShowSplashScreen() 
        {
            DrawBackground();

            graphics.Show();
        }

        protected void DrawBackground()
        {
            graphics.Clear(Color.FromHex("#23abe3"));

            graphics.DrawBuffer(
                x: graphics.Width / 2 - logo.Width / 2,
                y: 40,
                buffer: logo);

            graphics.Stroke = 2;
            graphics.DrawCircle(display.Width / 2, display.Height / 2, (display.Width / 2) - 10, Color.Black, false);
        }

        public void ShowTextLine1(string message) 
        {
            //drawing a rect to erase previous text ....
            graphics.DrawRectangle(48, 130, 144, 71, Color.FromHex("#23abe3"), true);

            graphics.DrawText(display.Width / 2, 139, message, Color.Black, alignment: GraphicsLibrary.TextAlignment.Center);

            graphics.Show();
        }

        public void ShowTextLine2(string message)
        {
            graphics.DrawText(display.Width / 2, 169, message, Color.Black, alignment: GraphicsLibrary.TextAlignment.Center);

            graphics.Show();
        }

        public void UpdateDisplay(Temperature conditions) 
        {
            this.conditions = conditions;

            if(menu.IsEnabled == false)
            {
                Render();
            }
        }

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            if (menu.IsEnabled)
            {
                return;
            }

            // if we're already rendering, bail out.
            if (isRendering)
            {
                Console.WriteLine("Already in a rendering loop, bailing out.");
                return;
            }

            isRendering = true;

            DrawBackground();

            string tempText;
            if (isCelcius)
            {
                tempText = $"{conditions.Celsius.ToString("##.#")}°C";
            }
            else
            {
                tempText = $"{conditions.Fahrenheit.ToString("##.#")}°F";
            }

            graphics.DrawText(
                x: display.Width / 2,
                y: 140,
                text: tempText,
                color: Color.Black,
                scaleFactor: GraphicsLibrary.ScaleFactor.X2,
                alignment: GraphicsLibrary.TextAlignment.Center);

            graphics.Show();

            isRendering = false;
        }

        public void MenuUp()
        {
            menu.Previous();
        }

        public void MenuDown()
        {
            menu.Next();
        }

        public void MenuSelect()
        {
            if(menu.IsEnabled == false)
            {
                menu.Enable();
            }
            else
            {
                menu.Select();
            }
        }

        protected void LoadJpeg()
        {
            var jpgData = LoadResource("meadow.jpg");
            var decoder = new JpegDecoder();
            decoder.DecodeJpeg(jpgData);

            logo = new BufferRgb888(decoder.Width, decoder.Height, decoder.GetImageData());
        }

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"WildernessLabs.Clima.Meadow.HackKit.{filename}";

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