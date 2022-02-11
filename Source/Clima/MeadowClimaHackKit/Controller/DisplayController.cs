using Meadow.Foundation;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Hardware;
using Meadow.Units;
using SimpleJpegDecoder;
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowClimaHackKit.Controller
{
    public class DisplayController
    {
        CancellationTokenSource token;

        protected Temperature conditions;

        protected St7789 display;
        protected BufferRgb888 logo;
        protected BufferRgb888 wifiConnecting;
        protected BufferRgb888 wifiConnected;
        protected MicroGraphics graphics;

        static Color backgroundColor = Color.FromHex("#23ABE3");

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
            var config = new SpiClockConfiguration(new Frequency(48000, Frequency.UnitType.Kilohertz), SpiClockConfiguration.Mode.Mode3);
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

            // finally, clear the display so it's ready for action
            graphics.Clear(true);

            //and load the logo jpg into a buffer
            //LoadJpeg("meadow.jpg");
            logo = LoadJpeg("meadow.jpg");
            wifiConnected = LoadJpeg("wifi_connected.jpg");
            wifiConnecting = LoadJpeg("wifi_connecting.jpg");
        }

        BufferRgb888 LoadJpeg(string fileName)
        {
            var jpgData = LoadResource(fileName);
            var decoder = new JpegDecoder();
            decoder.DecodeJpeg(jpgData);
            return new BufferRgb888(decoder.Width, decoder.Height, decoder.GetImageData());
        }

        void DrawBackground()
        {
            //clear the buffer to a single color
            graphics.Clear(backgroundColor);

            //draw the jpeg logo
            graphics.DrawBuffer(
                x: graphics.Width / 2 - logo.Width / 2,
                y: 34,
                buffer: logo);

            //draw the circle
            graphics.DrawCircle(
                centerX: display.Width / 2,
                centerY: display.Height / 2,
                radius: (display.Width / 2) - 10,
                color: Color.Black,
                filled: false);
        }

        public void UpdateDisplay(Temperature conditions)
        {
            this.conditions = conditions;

            Render();
        }

        public void ShowSplashScreen()
        {
            DrawBackground();

            graphics.Show();
        }

        

        protected byte[] LoadResource(string filename)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = $"MeadowClimaHackKit.{filename}";

            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        protected void Render()
        {
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
                tempText = $"{conditions.Celsius:##.#}°C";
            }
            else
            {
                tempText = $"{conditions.Fahrenheit:##.#}°F";
            }

            graphics.DrawText(
                x: display.Width / 2,
                y: 140,
                text: tempText,
                color: Color.Black,
                scaleFactor: ScaleFactor.X2,
                alignment: TextAlignment.Center);

            graphics.Show();

            isRendering = false;
        }

        public async Task StartWifiConnectingAnimation() 
        {
            token = new CancellationTokenSource();

            while (!token.IsCancellationRequested)
            {                
                graphics.DrawBuffer(
                    x: graphics.Width / 2 - wifiConnecting.Width / 2,
                    y: 134,
                    buffer: wifiConnecting);
                graphics.Show();

                await Task.Delay(500);

                graphics.DrawBuffer(
                    x: graphics.Width / 2 - wifiConnected.Width / 2,
                    y: 134,
                    buffer: wifiConnected);
                graphics.Show();

                await Task.Delay(500);
            }
        }

        public void StopWifiConnectingAnimation() 
        {
            token.Cancel();
        }
    }
}
