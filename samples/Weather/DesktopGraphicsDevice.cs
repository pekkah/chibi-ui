using System;
using System.Reflection;
using Meadow;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui.Weather;

public class DesktopGraphicsDevice(IPixelDisplay display) : PixelDisplayDevice(display)
{
    private static readonly ConstructorInfo? CreateSkiaBufferCtor = Assembly
        .GetAssembly(typeof(SilkDisplay))
        ?.GetType("Meadow.Foundation.Displays.SkiaPixelBuffer")
        ?.GetConstructor(BindingFlags.Instance | BindingFlags.Public, [typeof(int), typeof(int)]);

    private readonly IPixelBuffer _backbuffer = CreateSkiaPixelBuffer(display.Width, display.Height);

    private readonly IPixelDisplay _display = display;

    public override IPixelBuffer CreateBuffer(int width, int height)
    {
        if (_display is SilkDisplay silkDisplay)
            return CreateSkiaPixelBuffer(width, height);

        return base.CreateBuffer(width, height);
    }

    private static IPixelBuffer CreateSkiaPixelBuffer(int width, int height)
    {
        if (CreateSkiaBufferCtor is null)
            throw new InvalidOperationException("SkiaPixelBuffer constructor not found");

        return (IPixelBuffer)CreateSkiaBufferCtor.Invoke([width, height]);
    }

    public override void Show()
    {
        FrameBuffer.WriteBuffer(0, 0, _backbuffer);
    }

    public override void Clear(Color? clear)
    {
        if (clear.HasValue)
            _backbuffer.Fill(clear.Value);
        else
            _backbuffer.Clear();
    }


    public override IDrawingContext CreateDrawingContext()
    {
        return new DrawingContext(_backbuffer, ScaleFactor.X1);
    }
}