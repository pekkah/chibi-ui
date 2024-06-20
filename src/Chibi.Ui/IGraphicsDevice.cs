using Meadow;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui;

public interface IGraphicsDevice
{
    IPixelBuffer FrameBuffer { get; }

    int Width => FrameBuffer.Width;

    int Height => FrameBuffer.Height;

    void Show();

    void Clear(Color? clear);

    IDrawingContext CreateDrawingContext(IPixelBuffer buffer);

    IDrawingContext CreateDrawingContext();

    IPixelBuffer CreateBuffer(int width, int height);

    IPixelBuffer CreateBuffer(int width, int height, byte[] bytes);
}