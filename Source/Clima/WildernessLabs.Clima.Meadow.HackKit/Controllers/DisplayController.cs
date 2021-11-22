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

        protected Menu menu;
        protected St7789 display;
        protected BufferRgb888 logo;
        protected MicroGraphics graphics;

        static Color backgroundColor = Color.FromHex("#23abe3");

        protected bool isCelcius = true;
        protected bool isRendering = false;

        private static readonly Lazy<DisplayController> instance =
            new Lazy<DisplayController>(() => new DisplayController());
        public static DisplayController Instance => instance.Value;

        private DisplayController()
        {
            Initialize();
        }

        public void Initialize()
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
                width: 240, height: 240,
                displayColorMode: ColorType.Format16bppRgb565
            );

            // create our graphics surface that we'll draw onto and then blit to the display
            graphics = new MicroGraphics(display) 
            {   
                CurrentFont = new Font12x20(),
                Stroke = 3,
            };
            graphics.DisplayConfig.FontScale = 2;

            //create the menu for TextDisplayMenu
            var menuItems = new MenuItem[]
            {
                new MenuItem("°C", command: "setCelcius"),
                new MenuItem("°F", command: "setFahrenheit"),
            };

            menu = new Menu(graphics, menuItems, false);
            menu.Selected += MenuSelected;

            Console.WriteLine("Clear display");

            // finally, clear the display so it's ready for action
            graphics.Clear(true);

            //and load the logo jpg into a buffer
            LoadJpeg();
        }

        void MenuSelected(object sender, MenuSelectedEventArgs e)
        {
            Console.WriteLine("MenuSelected: " + e.Command);

            isCelcius = (e.Command == "setCelcius");
               
            //hide the menu after a selection
            menu.Disable();
            //and update the display
            Render();
        }
        
        void DrawBackground()
        {
            //clear the buffer to a single color
            graphics.Clear(backgroundColor); 

            //draw the jpeg logo
            graphics.DrawBuffer(
                x: graphics.Width / 2 - logo.Width / 2,
                y: 40,
                buffer: logo);

            //draw the circle
            graphics.DrawCircle(
                centerX: display.Width / 2, 
                centerY: display.Height / 2, 
                radius: (display.Width / 2) - 10, 
                color: Color.Black, 
                filled: false);
        }

        public void UpdateStatusText(string line1, string line2)
        {
            if (isRendering) return;

            //we'll do a partial update
            Rect rect = new Rect(40, 140, 200, 190);
            graphics.DrawRectangle(rect.Left, rect.Top, rect.Width, rect.Height, 
                backgroundColor, true);

            graphics.DrawText(display.Width / 2, 140, line1, Color.Black, 
                alignment: MicroGraphics.TextAlignment.Center);

            graphics.DrawText(display.Width / 2, 170, line2, Color.Black, 
                alignment: MicroGraphics.TextAlignment.Center);

            graphics.Show(rect);
        }

        public void UpdateDisplay(Temperature conditions) 
        {
            this.conditions = conditions;

            if(menu.IsEnabled == false)
            {
                Render();
            }
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

        public void ShowSplashScreen()
        {
            DrawBackground();

            graphics.Show();
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

        /// <summary>
        /// Does the actual rendering. If it's already rendering, it'll bail out,
        /// so render requests don't stack up.
        /// </summary>
        protected void Render()
        {
            //if the menu is enabled, it's responsible for drawing the screen
            if (menu.IsEnabled) { return; }

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
                scaleFactor: MicroGraphics.ScaleFactor.X2,
                alignment: MicroGraphics.TextAlignment.Center);

            graphics.Show();

            isRendering = false;
        }
    }
}