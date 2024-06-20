using System;
using System.Runtime.InteropServices;
using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Hardware;
using System.Threading.Tasks;
using Chibi.Ui.Views;
using Chibi.Weather.Shared;
using Meadow.Foundation.Sensors.Hid;
using System.Threading;
using Chibi.Ui;
using Chibi.Ui.Weather.Shared;
using Chibi.Ui.Weather.Shared.Views;
using Meadow.Peripherals.Displays;
using SkiaSharp;

namespace Weather;

public class MeadowApp : App<Desktop>
{
    public WeatherViewManager ViewManager { get; private set; }

    public override Task Initialize()
    {
        Resolver.Log.Info($"Initializing {this.GetType().Name}");
        Resolver.Log.Info($" Platform OS is a {Device.PlatformOS.GetType().Name}");
        Resolver.Log.Info($" Platform: {Device.Information.Platform}");
        Resolver.Log.Info($" OS: {Device.Information.OSVersion}");
        Resolver.Log.Info($" Model: {Device.Information.Model}");
        Resolver.Log.Info($" Processor: {Device.Information.ProcessorType}");

        Display = Device.Display;
        Display?.Resize(240, 320);

        //var graphicsDevice = new DiffingPixelDisplayDevice(Display, new DesktopDisplayDevice(Display));
        var graphicsDevice = new DesktopDisplayDevice(Display);
        ViewManager = new WeatherViewManager(graphicsDevice, 60, Display as ITouchScreen);
        Resolver.Services.Add<INavigationController>(ViewManager);
        Resolver.Services.Add<IRenderingDetails>(ViewManager);
        Resolver.Services.Add(Device.Display);

        if (Device.Display is ITouchScreen touchScreen)
            EnableTouchScreen(touchScreen);

        // AssetManager
        var assetManager = new AssetManager(graphicsDevice);
        Resolver.Services.Add(assetManager);

        // Views
        Resolver.Services.Create<MainView>();

        // Input
        var inputManager = W.CreateInputManager(ViewManager);
        var keyboard = new Keyboard();
        inputManager.MapAction(keyboard.Pins.Left.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Left);
        inputManager.MapAction(keyboard.Pins.Right.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Right);
        inputManager.MapAction(keyboard.Pins.Up.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Up);
        inputManager.MapAction(keyboard.Pins.Down.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Down);
        inputManager.MapAction(keyboard.Pins.Enter.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Enter);
        inputManager.MapAction(keyboard.Pins.Back.CreateDigitalInterruptPort(InterruptMode.EdgeRising), Actions.Back);

        return base.Initialize();
    }

    public IResizablePixelDisplay Display { get; protected set; }

    private void EnableTouchScreen(ITouchScreen touchScreen)
    {
        touchScreen.TouchClick += (sender, point) =>
        {
            ViewManager.OnTouchUp(point);
        };
    }

    public override Task Run()
    {
        ViewManager.Navigate<MainView>();
        var cts = new CancellationTokenSource();
        ViewManager.Start(cts.Token);

        // NOTE: this will not return until the display is closed
        ExecutePlatformDisplayRunner();

        return Task.CompletedTask;
    }

    private void ExecutePlatformDisplayRunner()
    {
        var silkDisplay = Display as SilkDisplay;
        silkDisplay?.Run();
    }
}

public class DesktopDisplayDevice(IPixelDisplay display) : PixelDisplayDevice(display)
{
    public override IPixelBuffer CreateBuffer(int width, int height)
    {
        return new SkiaPixelBuffer(width, height);
    }

    public override IPixelBuffer CreateBuffer(int width, int height, byte[] bytes)
    {
        var buffer = new SkiaPixelBuffer(width, height);
        SKBitmap bitmap = buffer.SKBitmap;

        // Calculate row bytes assuming the bytes are in RGBA8888 format.
        int rowBytes = width * 4;

        // Allocate unmanaged memory for the byte array.
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);

        try
        {
            // Copy the byte array to the allocated unmanaged memory.
            Marshal.Copy(bytes, 0, ptr, bytes.Length);

            // Create an SKPixmap with the unmanaged memory pointer.
            using var pixmap = new SKPixmap(new SKImageInfo(width, height, SKColorType.Rgba8888), ptr, rowBytes);

            // Use the pixmap to install pixels into the bitmap.
            bitmap.InstallPixels(pixmap);
        }
        finally
        {
            // Free the allocated unmanaged memory.
            Marshal.FreeHGlobal(ptr);
        }

        return buffer;
    }


}
