using System;
using System.Threading;
using System.Threading.Tasks;
using Chibi.Ui;
using Chibi.Ui.Views;
using Chibi.Ui.Weather.F7;
using Chibi.Ui.Weather.Shared.Views;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Hardware;
using Meadow.Peripherals.Displays;
using Meadow.Peripherals.Leds;
using Meadow.Units;

namespace Weather.F7;

public class MeadowApp : App<F7FeatherV2>
{
    private RgbPwmLed _onboardLed;

    private TaskCompletionSource<object> _wifiConnected = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public MeadowApp()
    {
        Device.NetworkAdapters.NetworkConnected += (sender, args) => { _wifiConnected.TrySetResult(null); };

        Device.NetworkAdapters.NetworkDisconnected += (sender, args) =>
        {
            _wifiConnected = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
        };
    }

    public Xpt2046 TouchScreen { get; set; }

    public Ili9341 Display { get; set; }

    public ViewManager ViewManager { get; private set; }

    public override async Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();
        //wifi.SetAntenna(AntennaType.External, true);

        if (wifi.IsConnected)
        {
            _wifiConnected.SetResult(null);
        }

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
        TouchScreen = new Xpt2046(
            spiBus,
            Device.Pins.D10.CreateDigitalInterruptPort(InterruptMode.EdgeFalling, ResistorMode.InternalPullUp),
            Device.Pins.D11.CreateDigitalOutputPort(true));

        Resolver.Log.Info($"Initializing {GetType().Name}");
        Resolver.Log.Info($" Platform OS is a {Device.PlatformOS.GetType().Name}");
        Resolver.Log.Info($" Platform: {Device.Information.Platform}");
        Resolver.Log.Info($" OS: {Device.Information.OSVersion}");
        Resolver.Log.Info($" Model: {Device.Information.Model}");
        Resolver.Log.Info($" Processor: {Device.Information.ProcessorType}");

        var graphicsDevice = new PixelDisplayDevice(Display);
        ViewManager = new ViewManager(graphicsDevice, 1);
        Resolver.Services.Add<INavigationController>(ViewManager);
        Resolver.Services.Add<IRenderingDetails>(ViewManager);
        Resolver.Services.Add<IRenderingControl>(ViewManager);
        Resolver.Services.Add<ICalibratableTouchscreen>(TouchScreen);

        // Views
        Resolver.Services.Create<MainView>();
        Resolver.Services.Create<CalibrationView>();

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
        GC.Collect();

        TouchScreen.TouchUp += (s, e) =>
        {
            Resolver.Log.Info($"Touch Up: {e.ScreenX}, {e.ScreenY}");
            ViewManager.HandleTouch(e);
        };

        TouchScreen.TouchDown += (s, e) =>
        {
            Resolver.Log.Info($"Touch Down: {e.ScreenX}, {e.ScreenY}");
            //ViewManager.HandleTouch(e);
        };

        TouchScreen.TouchMoved += (s, e) =>
        {
            Resolver.Log.Info($"Touch Move: {e.ScreenX}, {e.ScreenY}");
            //ViewManager.HandleTouch(e);
        };

        TouchScreen.TouchClick += (s, e) =>
        {
            Resolver.Log.Info($"Touch Click: {e.ScreenX}, {e.ScreenY}");
            //ViewManager.HandleTouch(e);
        };

        GC.Collect();
        Resolver.Log.Info("Starting ViewManager");
        DrawText(Display, "Loading...");
        ViewManager.Navigate<CalibrationView>();
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

        var wifi = Device.NetworkAdapters.Primary<IWiFiNetworkAdapter>();

        if (!wifi.IsConnected)
        {
            await _wifiConnected.Task;
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