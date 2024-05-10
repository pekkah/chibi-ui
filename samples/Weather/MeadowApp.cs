﻿using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Hardware;
using System.Threading.Tasks;
using Chibi.Ui.Views;
using Chibi.Weather.Shared;
using Chibi.Weather.Shared.Views;
using Meadow.Foundation.Sensors.Hid;
using System.Threading;
using Chibi.Ui.Weather;
using Chibi.Ui.Weather.Shared.Views;

namespace Weather
{
    public class MeadowApp : App<Desktop>
    {
        public ViewManager ViewManager { get; private set; }

        public override Task Initialize()
        {
            Resolver.Log.Info($"Initializing {this.GetType().Name}");
            Resolver.Log.Info($" Platform OS is a {Device.PlatformOS.GetType().Name}");
            Resolver.Log.Info($" Platform: {Device.Information.Platform}");
            Resolver.Log.Info($" OS: {Device.Information.OSVersion}");
            Resolver.Log.Info($" Model: {Device.Information.Model}");
            Resolver.Log.Info($" Processor: {Device.Information.ProcessorType}");

            Device.Display?.Resize(240, 320);

            var graphicsDevice = new DesktopGraphicsDevice(Device.Display);
            ViewManager = new ViewManager(graphicsDevice, 60);
            Resolver.Services.Add<INavigationController>(ViewManager);
            Resolver.Services.Add<IRenderingDetails>(ViewManager);
            Resolver.Services.Add(Device.Display);

            if (Device.Display is ITouchScreen touchScreen)
                EnableTouchScreen(touchScreen);

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

        private void EnableTouchScreen(ITouchScreen touchScreen)
        {
            touchScreen.TouchClick += (sender, point) =>
            {
                ViewManager.HandleTouch(point);
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
            if (Device.Display is SilkDisplay display)
            {
                display.Run();
            }
        }
    }
}