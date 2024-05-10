using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui;

public class TextPresenter : UiElement
{
    public TextPresenter()
    {
        TextProperty = Property(nameof(Text), "");
        FontProperty = Property(nameof(Font), (IFont)new Font12x20());
        ColorProperty = Property(nameof(Color), Color.White);
        VerticalAlignment = VerticalAlignment.Top;
        HorizontalAlignment = HorizontalAlignment.Left;
    }

    public string Text
    {
        get => TextProperty.Value;
        set => TextProperty.Value = value;
    }

    public ReactiveProperty<string> TextProperty { get; }

    public IFont Font
    {
        get => FontProperty.Value;
        set => FontProperty.Value = value;
    }

    public ReactiveProperty<IFont> FontProperty { get; }

    public Color Color
    {
        get => ColorProperty.Value;
        set => ColorProperty.Value = value;
    }

    public ReactiveProperty<Color> ColorProperty { get; }

    protected Size TextSize => DrawingContext.MeasureText(Text, Font);

    public override void Render(IDrawingContext context, Rect bounds)
    {
        context.DrawText(bounds.Position, Text, Font, Color);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return TextSize;
    }
}