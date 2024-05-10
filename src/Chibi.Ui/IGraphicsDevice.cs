using Meadow;
using Meadow.Peripherals.Displays;

namespace Chibi.Ui;

public interface IGraphicsDevice
{
    IPixelBuffer FrameBuffer { get; }

    public IPixelBuffer CreateBuffer(int width, int height);

    void Show();

    void Clear(Color? clear);

    IDrawingContext CreateDrawingContext(IPixelBuffer buffer);

    IDrawingContext CreateDrawingContext();

    int Width => FrameBuffer.Width;

    int Height => FrameBuffer.Height;
}