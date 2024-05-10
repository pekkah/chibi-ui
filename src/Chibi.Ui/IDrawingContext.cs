using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Peripherals.Displays;
using Meadow.Units;

namespace Chibi.Ui;

public interface IDrawingContext
{
    void DrawText(
        Point position,
        string text,
        IFont font,
        Color color);

    void DrawRectangle(Rect bounds, Color color, bool filled = false);
    void DrawHorizontalGradient(Rect bounds, Color startColor, Color endColor);
    void DrawVerticalGradient(Rect bounds, Color startColor, Color endColor);
    void DrawBuffer(Point position, IPixelBuffer source, Color? transparencyColor);
    void DrawCircle(Point position, int radius, Color color, bool filled = false);
    void DrawLine(Point position, int length, int angle, Color color);
    void DrawLine(Point start, Point end, Color color);
    void DrawArc(Point center, int radius, Angle startAngle, Angle endAngle, Color color);
    void DrawHorizontalLine(Point position, int length, Color color);
    void DrawVerticalLine(Point position, int length, Color color);
    void DrawTriangle(Point p0, Point p1, Point p2, Color color, bool filled = false);
    void DrawCircleQuadrant(Point center, int radius, int quadrant, Color color, bool filled = false);

    void DrawRoundedRectangle(Point position, int width, int height, int cornerRadius, Color color,
        bool filled = false);

    void Show();
    void Clear();
}