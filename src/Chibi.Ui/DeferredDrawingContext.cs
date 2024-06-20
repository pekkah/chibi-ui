using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;
using Meadow.Units;

namespace Chibi.Ui;

public interface IDrawingAction
{
    Rect Bounds { get; }

    void Draw(IDrawingContext context);
}

public class DeferredDrawingContext : IDrawingContext
{
    public void DrawText(Point position, string text, IFont font, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawRectangle(Rect bounds, Color color, bool filled = false)
    {
        throw new System.NotImplementedException();
    }

    public void DrawHorizontalGradient(Rect bounds, Color startColor, Color endColor)
    {
        throw new System.NotImplementedException();
    }

    public void DrawVerticalGradient(Rect bounds, Color startColor, Color endColor)
    {
        throw new System.NotImplementedException();
    }

    public void DrawBuffer(Point position, IPixelBuffer source, Color? transparencyColor)
    {
        throw new System.NotImplementedException();
    }

    public void DrawCircle(Point position, int radius, Color color, bool filled = false)
    {
        throw new System.NotImplementedException();
    }

    public void DrawLine(Point position, int length, int angle, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawLine(Point start, Point end, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawArc(Point center, int radius, Angle startAngle, Angle endAngle, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawHorizontalLine(Point position, int length, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawVerticalLine(Point position, int length, Color color)
    {
        throw new System.NotImplementedException();
    }

    public void DrawTriangle(Point p0, Point p1, Point p2, Color color, bool filled = false)
    {
        throw new System.NotImplementedException();
    }

    public void DrawCircleQuadrant(Point center, int radius, int quadrant, Color color, bool filled = false)
    {
        throw new System.NotImplementedException();
    }

    public void DrawRoundedRectangle(Point position, int width, int height, int cornerRadius, Color color, bool filled = false)
    {
        throw new System.NotImplementedException();
    }

    public void Show()
    {
        throw new System.NotImplementedException();
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}