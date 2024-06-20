using System;
using System.Threading;
using System.Threading.Tasks;
using Chibi.Ui;
using Chibi.Ui.Views;
using Chibi.Ui.Weather.Shared.Views;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;

namespace Weather.F7;

public class MeadowApp : App<F7FeatherV2>
{
    private RgbPwmLed _onboardLed;


    public Xpt2046 TouchScreen { get; set; }

    public Ili9341 Display { get; set; }

    public WeatherViewManager ViewManager { get; private set; }

    public override async Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        _onboardLed = new RgbPwmLed(
            Device.Pins.OnboardLedRed,
            Device.Pins.OnboardLedGreen,
            Device.Pins.OnboardLedBlue,
            CommonType.CommonAnode);


        var spiBus = Device.CreateSpiBus();
        Display = new Ili9341
        (
            spiBus,
            Device.Pins.D13,
            Device.Pins.D14,
            Device.Pins.D15,
            240,
            320
        );
        Display.SetRotation(RotationType._180Degrees);
        //Display.SpiBusSpeed = new Frequency(20, Frequency.UnitType.Megahertz);

        DrawText(Display, "Initializing...");
        /*TouchScreen = new Xpt2046(
            spiBus,
            Device.Pins.D10.CreateDigitalInterruptPort(InterruptMode.EdgeBoth, ResistorMode.InternalPullUp),
            Device.Pins.D11.CreateDigitalOutputPort(true));*/

        Resolver.Log.Info($"Initializing {GetType().Name}");
        Resolver.Log.Info($" Platform OS is a {Device.PlatformOS.GetType().Name}");
        Resolver.Log.Info($" Platform: {Device.Information.Platform}");
        Resolver.Log.Info($" OS: {Device.Information.OSVersion}");
        Resolver.Log.Info($" Model: {Device.Information.Model}");
        Resolver.Log.Info($" Processor: {Device.Information.ProcessorType}");

        //var graphicsDevice = new DiffingPixelDisplayDevice(Display, new PixelDisplayDevice(Display));
        var graphicsDevice = new PixelDisplayDevice(Display);
        ViewManager = new WeatherViewManager(graphicsDevice, 30, TouchScreen);
        Resolver.Services.Add<INavigationController>(ViewManager);
        Resolver.Services.Add<IRenderingDetails>(ViewManager);
        Resolver.Services.Add<IRenderingControl>(ViewManager);
        //Resolver.Services.Add<ICalibratableTouchscreen>(TouchScreen);

        // AssetManager
        var assetManager = new AssetManager(graphicsDevice);
        Resolver.Services.Add(assetManager);

        // Views
        Resolver.Services.Create<MainView>();
        //Resolver.Services.Create<CalibrationView>();

        // Input
        /*var inputManager = W.CreateInputManager(WeatherViewManager);
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
        var g = new MicroGraphics(display)
        {
            CurrentFont = new Font12x16()
        };
        g.Clear();
        g.DrawText(Display.Width / 2 - g.MeasureText(text).Width / 2,
            Display.Height / 2 - g.MeasureText(text).Height / 2, text, Color.White);
        g.Show();
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run...");
        await WaitForNetwork();
        GC.Collect();

        /*TouchScreen.TouchUp += (s, e) =>
        {
            Resolver.Log.Info($"Touch Up: {e.ScreenX}, {e.ScreenY}");
            ViewManager.OnTouchUp(e);
        };

        TouchScreen.TouchDown += (s, e) =>
        {
            Resolver.Log.Info($"Touch Down: {e.ScreenX}, {e.ScreenY}");
            //WeatherViewManager.OnTouchUp(e);
        };

        TouchScreen.TouchMoved += (s, e) =>
        {
            Resolver.Log.Info($"Touch Move: {e.ScreenX}, {e.ScreenY}");
            //WeatherViewManager.OnTouchUp(e);
        };

        TouchScreen.TouchClick += (s, e) =>
        {
            Resolver.Log.Info($"Touch Click: {e.ScreenX}, {e.ScreenY}");
            //WeatherViewManager.OnTouchUp(e);
        };
        */
        Resolver.Log.Info("Starting WeatherViewManager");
        DrawText(Display, "Loading...");
        /*  There's an issue with touch screen sharing the same SPI bus with the display. It's not working.
         * The Meadow team is working on a fix. Manual push buttons for navigation will be used for now when needed.
         *
         *
         * ViewManager.Navigate<CalibrationView>();
         */
        ViewManager.Navigate<MainView>();
        var cts = new CancellationTokenSource();
        ViewManager.Start(cts.Token);

        Resolver.Log.Info("WeatherViewManager started.");
        await CycleColors(TimeSpan.FromMilliseconds(5000));
    }

    private async Task CycleColors(TimeSpan duration)
    {
        while (true)
        {
            await ShowColorPulse(Color.Blue, duration);
            await ShowColorPulse(Color.Cyan, duration);
            await ShowColorPulse(Color.Green, duration);
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

        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        if (!wifi.IsConnected)
        {
            await wifi.ConnectToDefaultAccessPoint(TimeSpan.FromMinutes(2), CancellationToken.None);
            GC.Collect();
        }

        if (wifi.IsConnected)
        {
            Resolver.Log.Info("Network connected");
            DrawText(Display, "Connected.");
        }
        else
        {
            Resolver.Log.Info("Failed to connect to default access point.");
            DrawText(Display, "Failed to connect to default access point.");
        }
    }
}