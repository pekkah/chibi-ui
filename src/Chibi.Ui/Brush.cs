using System.Numerics;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui;

public interface IBrush
{
    void Draw(IDrawingContext context, Rect bounds);
}

public class EmptyBrush : IBrush
{
    public void Draw(IDrawingContext context, Rect bounds)
    {
        // noop
    }
}

public class RectangleBrush : IBrush
{
    public Color Color { get; set; }

    public int CornerRadius { get; set; }

    public bool Filled { get; set; }

    public void Draw(IDrawingContext context, Rect bounds)
    {
        if (CornerRadius > 0)
        {
            context.DrawRoundedRectangle(
                bounds.Position,
                bounds.Width, 
                bounds.Height, 
                CornerRadius,
                Color, 
                Filled);
        }
        else
        {
            context.DrawRectangle(bounds, Color, Filled);
        }
    }
}

public class CircleBrush : IBrush
{
    public Color Color { get; set; }

    public bool Filled { get; set; }

    public int Radius { get; set; }

    public void Draw(IDrawingContext context, Rect bounds)
    {
        var centerPosition = new Point(
            bounds.X + bounds.Width / 2,
            bounds.Y + bounds.Height / 2);

        context.DrawCircle(centerPosition, Radius, Color, Filled);
    }
}