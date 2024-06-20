using System;
using Meadow;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui;

public class DiffingPixelDisplayDevice : IGraphicsDevice
{
    public DiffingPixelDisplayDevice(IPixelDisplay display, IGraphicsDevice device)
    {
        Display = display;
        Device = device;
        PreviousFrame = CreateBuffer(Display.Width, Display.Height);
    }

    protected IPixelDisplay Display { get; }

    protected IPixelBuffer PreviousFrame { get; set; }

    protected IGraphicsDevice Device { get; set; }


    public IPixelBuffer FrameBuffer => Display.PixelBuffer;

    public IPixelBuffer CreateBuffer(int width, int height)
    {
        return Device.CreateBuffer(width, height);
    }

    public void Show()
    {
        try
        {
            var previousBytes = PreviousFrame.Buffer.AsSpan();
            var currentBytes = FrameBuffer.Buffer.AsSpan();
            int x = 0, top = -1, bottom = 0;

            while (bottom < Display.Height)
            {
                var rowStart = bottom * FrameBuffer.Width * FrameBuffer.BitDepth;
                if (rowStart >= currentBytes.Length || rowStart >= previousBytes.Length)
                {
                    break;
                }

                var row = currentBytes.Slice(rowStart, FrameBuffer.Width * FrameBuffer.BitDepth);
                var previousRow = previousBytes.Slice(rowStart, FrameBuffer.Width * FrameBuffer.BitDepth);

                if (!row.SequenceEqual(previousRow))
                {
                    if (top == -1) top = bottom; // Start of the changed area
                }
                else if (top != -1)
                {
                    // End of the changed area, call Display.Show
                    Display.Show(x, top, FrameBuffer.Width - 1, bottom - 1);
                    top = -1; // Reset the start of the changed area
                }

                bottom++;
            }

            // If there's a changed area at the end of the buffer
            if (top != -1)
            {
                Display.Show(x, top, FrameBuffer.Width - 1, Display.Height - 1);
            }

            // store previous frame for comparison
            PreviousFrame = CreateBuffer(Display.Width, Display.Height, Display.PixelBuffer.Buffer);
        }
        catch (Exception ex)
        {
            Resolver.Log.Error(ex, "Error showing diffing frame");
        }
    }

    public void Clear(Color? clear)
    {
    }

    public IDrawingContext CreateDrawingContext(IPixelBuffer buffer)
    {
        return Device.CreateDrawingContext(buffer);
    }

    public IDrawingContext CreateDrawingContext()
    {
        return Device.CreateDrawingContext();
    }

    public IPixelBuffer CreateBuffer(int width, int height, byte[] bytes)
    {
        return Device.CreateBuffer(width, height, bytes);
    }
}