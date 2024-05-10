using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;
using Meadow.Units;

namespace Chibi.Ui;

public class DrawingContext : IDrawingContext
{
    protected readonly IPixelBuffer Buffer;
    protected readonly MicroGraphics Graphics;
    protected readonly ScaleFactor Scale;

    public DrawingContext(IPixelBuffer buffer, ScaleFactor scale)
    {
        Scale = scale;
        Buffer = buffer;
        Graphics = new MicroGraphics(Buffer, true);
    }

    public void DrawText(
        Point position,
        string text,
        IFont font,
        Color color)
    {
        Graphics
            .DrawText(
                position.X, position.Y,
                text,
                color,
                Scale,
                Meadow.Foundation.Graphics.HorizontalAlignment.Left,
                Meadow.Foundation.Graphics.VerticalAlignment.Top,
                font);
    }

    public void DrawRectangle(Rect bounds, Color color, bool filled = false)
    {
        Graphics
            .DrawRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, color, filled);
    }

    public void DrawHorizontalGradient(Rect bounds, Color startColor, Color endColor)
    {
        Graphics
            .DrawHorizontalGradient(
                bounds.X, bounds.Y,
                bounds.Width, bounds.Height,
                startColor,
                endColor);
    }

    public void DrawVerticalGradient(Rect bounds, Color startColor, Color endColor)
    {
        Graphics
            .DrawVerticalGradient(
                bounds.X, bounds.Y,
                bounds.Width, bounds.Height,
                startColor,
                endColor);
    }

    public void DrawBuffer(Point position, IPixelBuffer source, Color? transparencyColor)
    {
        if (transparencyColor.HasValue)
            Graphics
                .DrawBufferWithTransparencyColor(position.X, position.Y,
                    source,
                    transparencyColor.Value);
        else
            Graphics
                .DrawBuffer(position.X, position.Y, source);
    }

    public void DrawCircle(Point position, int radius, Color color, bool filled = false)
    {
        Graphics
            .DrawCircle(position.X, position.Y,
                radius,
                color,
                filled,
                true);
    }

    public void DrawLine(Point position, int length, int angle, Color color)
    {
        Graphics
            .DrawLine(position.X, position.Y, length,
                angle,
                color);
    }

    public void DrawLine(Point start, Point end, Color color)
    {
        Graphics
            .DrawLine(
                start.X, start.Y,
                end.X, end.Y,
                color);
    }

    public void DrawArc(Point center, int radius, Angle startAngle, Angle endAngle, Color color)
    {
        Graphics
            .DrawArc(center.X, center.Y, radius, startAngle, endAngle, color);
    }

    public void DrawHorizontalLine(Point position, int length, Color color)
    {
        Graphics.DrawHorizontalLine(position.X, position.Y, length, color);
    }

    public void DrawVerticalLine(Point position, int length, Color color)
    {
        Graphics.DrawVerticalLine(position.X, position.Y, length, color);
    }

    public void DrawTriangle(Point p0, Point p1, Point p2, Color color, bool filled = false)
    {
        Graphics.DrawTriangle(
            p0.X, p0.Y,
            p1.X, p1.Y,
            p2.X, p2.Y,
            color,
            filled);
    }

    public void DrawCircleQuadrant(Point center, int radius, int quadrant, Color color, bool filled = false)
    {
        Graphics.DrawCircleQuadrant(center.X, center.Y, radius, quadrant, color, filled);
    }

    public void DrawRoundedRectangle(Point position, int width, int height, int cornerRadius, Color color,
        bool filled = false)
    {
        Graphics.DrawRoundedRectangle(position.X, position.Y, width, height, cornerRadius, color, filled);
    }

    public virtual void Show()
    {
        Graphics.Show();
    }

    public virtual void Clear()
    {
        Graphics.Clear();
    }

    /// <summary>
    ///     Get the size in pixels of a string for a given font and scale factor
    /// </summary>
    /// <param name="text">The string to measure</param>
    /// <param name="font">The font used to calculate the text size</param>
    /// <param name="scaleFactor">Scale factor used to calculate the size</param>
    public static Size MeasureText(string text, IFont font, ScaleFactor scaleFactor = ScaleFactor.X1)
    {
        return new Size(text.Length * (int)scaleFactor * font.Width, (int)scaleFactor * font.Height);
    }
}