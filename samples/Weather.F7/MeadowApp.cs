using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Leds;
using Meadow.Peripherals.Leds;
using System;
using System.Linq;
using System.Threading.Tasks;
using Chibi.Ui;
using Chibi.Ui.Views;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Chibi.Ui.Weather.F7;
using Chibi.Ui.Weather.Shared.Views;
using Meadow.Foundation.Graphics.MicroLayout;
using System.Threading;
using Meadow.Foundation.Graphics;
using Meadow.Units;

namespace Weather.F7
{
    public class MeadowApp : App<F7FeatherV2>
    {
        private RgbPwmLed _onboardLed;

        public override async Task Initialize()
        {
            Resolver.Log.Info("Initialize...");

            _onboardLed = new RgbPwmLed(
                redPwmPin: Device.Pins.OnboardLedRed,
                greenPwmPin: Device.Pins.OnboardLedGreen,
                bluePwmPin: Device.Pins.OnboardLedBlue,
                CommonType.CommonAnode);

            
            Display = new Ili9341
            (
                spiBus: Device.CreateSpiBus(),
                chipSelectPin: Device.Pins.D13,
                dcPin: Device.Pins.D14,
                resetPin: Device.Pins.D15,
                width: 240,
                height: 320
            );
            Display.SetRotation(RotationType._180Degrees);
            Display.SpiBusSpeed = new Frequency(20, Frequency.UnitType.Megahertz);

            DrawText(Display, "Initializing...");
            TouchScreen = new Xpt2046(
                Device.CreateSpiBus(),
                Device.Pins.D10.CreateDigitalInterruptPort(InterruptMode.EdgeFalling, ResistorMode.InternalPullUp),
                Device.Pins.D11.CreateDigitalOutputPort(true));

            Resolver.Log.Info($"Initializing {this.GetType().Name}");
            Resolver.Log.Info($" Platform OS is a {Device.PlatformOS.GetType().Name}");
            Resolver.Log.Info($" Platform: {Device.Information.Platform}");
            Resolver.Log.Info($" OS: {Device.Information.OSVersion}");
            Resolver.Log.Info($" Model: {Device.Information.Model}");
            Resolver.Log.Info($" Processor: {Device.Information.ProcessorType}");

            var graphicsDevice = new PixelDisplayDevice(Display);
            ViewManager = new ViewManager(graphicsDevice, 60);
            Resolver.Services.Add<INavigationController>(ViewManager);
            Resolver.Services.Add<IRenderingDetails>(ViewManager);
            
            // Views
            Resolver.Services.Create<MainView>();

            // Input
            /*var inputManager = W.CreateInputManager(ViewManager);
            inputManager.MapAction(keyboard.Pins.Left.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Left);
            inputManager.MapAction(keyboard.Pins.Right.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Right);
            inputManager.MapAction(keyboard.Pins.Up.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Up);
            inputManager.MapAction(keyboard.Pins.Down.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Down);
            inputManager.MapAction(keyboard.Pins.Enter.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Enter);
            inputManager.MapAction(keyboard.Pins.Back.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Back);
            */
            await base.Initialize();
        }

        private void DrawText(Ili9341 display, string text)
        {
            var g = new MicroGraphics(display);
            g.Clear();
            g.DrawText(Display.Width / 2 - (g.MeasureText(text).Width / 2), Display.Height  / 2 - (g.MeasureText(text).Height / 2), text, Color.White);
            g.Show();
        }

        public Xpt2046 TouchScreen { get; set; }

        public Ili9341 Display { get; set; }

        public ViewManager ViewManager { get; private set; }

        public override async Task Run()
        {
            Resolver.Log.Info("Run...");
            await WaitForNetwork();

            var ts = new TouchscreenCalibrationService(new DisplayScreen(Display, RotationType._180Degrees, TouchScreen));
            ts.EraseCalibrationData();

            var calData = ts.GetSavedCalibrationData();
            if (calData != null)
            {
                TouchScreen.SetCalibrationData(calData);
            }
            else
            {
                Resolver.Log.Info("Calibrating");

                await ts.Calibrate(true);
            }

            TouchScreen.TouchUp += (s, e) =>
            {
                Resolver.Log.Info($"Touch Up: {e.ScreenX}, {e.ScreenY}");
                ViewManager.HandleTouch(e);
            };

            Resolver.Log.Info("Starting ViewManager");
            DrawText(Display, "Loading...");
            ViewManager.Navigate<MainView>();
            var cts = new CancellationTokenSource();
            ViewManager.Start(cts.Token);

            Resolver.Log.Info("ViewManager started.");
            await CycleColors(TimeSpan.FromMilliseconds(5000));
        }

        private async Task CycleColors(TimeSpan duration)
        {
            Resolver.Log.Info("Cycle colors...");

            while (true)
            {
                await ShowColorPulse(Color.Blue, duration);
                await ShowColorPulse(Color.Cyan, duration);
                await ShowColorPulse(Color.Green, duration);
                await ShowColorPulse(Color.GreenYellow, duration);
                await ShowColorPulse(Color.Yellow, duration);
                await ShowColorPulse(Color.Orange, duration);
                await ShowColorPulse(Color.OrangeRed, duration);
                await ShowColorPulse(Color.Red, duration);
                await ShowColorPulse(Color.MediumVioletRed, duration);
                await ShowColorPulse(Color.Purple, duration);
                await ShowColorPulse(Color.Magenta, duration);
                await ShowColorPulse(Color.Pink, duration);
            }
        }

        private async Task ShowColorPulse(Color color, TimeSpan duration)
        {
            await _onboardLed.StartPulse(color, duration / 2);
            await Task.Delay(duration);
        }

        private async Task WaitForNetwork()
        {
            Resolver.Log.Info("Checking connection...");
            DrawText(Display, "Connecting...");
            if (!Device.NetworkAdapters.Any(n => n.IsConnected))
            {
                while (!Device.NetworkAdapters.Any(n => n.IsConnected))
                {
                    Resolver.Log.Info("Waiting connection...");
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }
            }

            Resolver.Log.Info("Network connected");
            DrawText(Display, "Connected.");
        }
    }
}