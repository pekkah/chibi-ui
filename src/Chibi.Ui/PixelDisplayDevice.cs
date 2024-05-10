using System;
using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui;

public class PixelDisplayDevice(IPixelDisplay display) : IGraphicsDevice
{
    public virtual IPixelBuffer CreateBuffer(int width, int height)
    {
        // would be nice if the display had a method to create a matching buffer
        return display.ColorMode switch
        {
            ColorMode.Format1bpp => new Buffer1bpp(width, height),
            ColorMode.Format16bppRgb565 => new BufferRgb565(width, height),
            ColorMode.Format24bppRgb888 => new BufferRgb888(width, height),
            ColorMode.Format4bppGray => new BufferGray4(width, height),
            ColorMode.Format2bpp => throw new NotSupportedException(),
            ColorMode.Format4bppIndexed => new BufferIndexed4(width, height),
            ColorMode.Format8bppGray => new BufferGray8(width, height),
            ColorMode.Format8bppRgb332 => new BufferRgb332(),
            ColorMode.Format12bppRgb444 => new BufferRgb444(width, height),
            ColorMode.Format16bppRgb555 => throw new NotSupportedException(),
            ColorMode.Format18bppRgb666 => throw new NotSupportedException(),
            ColorMode.Format24bppGrb888 => throw new NotSupportedException(),
            ColorMode.Format32bppRgba8888 => new BufferRgba8888(width, height),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public IPixelBuffer FrameBuffer => display.PixelBuffer;

    public virtual void Show()
    {
        display.Show();
    }

    public virtual void Clear(Color? clear)
    {
        if (clear.HasValue)
        {
            display.Fill(clear.Value);
            return;
        }

        display.Clear();
    }

    public virtual IDrawingContext CreateDrawingContext(IPixelBuffer buffer)
    {
        return new DrawingContext(buffer, ScaleFactor.X1);
    }

    public virtual IDrawingContext CreateDrawingContext()
    {
        return CreateDrawingContext(FrameBuffer);
    }
}