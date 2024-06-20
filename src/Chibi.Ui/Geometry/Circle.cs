using Chibi.Ui.DataBinding;
using Meadow;

namespace Chibi.Ui.Geometry;

public class Circle : UiElement
{
    public Circle()
    {
        ColorProperty = Property(nameof(Color), Color.Black);
        FilledProperty = Property(nameof(Filled), true);
        Width = 6;
        Height = 6;
    }


    public ReactiveProperty<bool> FilledProperty { get; }

    public bool Filled
    {
        get => FilledProperty.Value;
        set => FilledProperty.Value = value;
    }

    public ReactiveProperty<Color> ColorProperty { get; }

    public Color Color
    {
        get => ColorProperty.Value;
        set => ColorProperty.Value = value;
    }

    public override void Render(IDrawingContext context, Rect bounds)
    {
        base.Render(context, bounds);
        context.DrawCircle(bounds.Center, bounds.Width / 2, Color, Filled);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var size = base.MeasureOverride(availableSize);
        return size;
    }
}