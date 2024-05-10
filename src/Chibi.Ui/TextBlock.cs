using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui;

public class TextBlock : ContentControl<string>
{
    private static readonly IFont Font12X20 = new Font12x20();

    public TextBlock()
    {
        FontProperty = Property(nameof(Font), Font12X20);
        ColorProperty = Property(nameof(Color), Color.White);
        ContentPresenter = new TextPresenter
        {
            ParentElement = this,
            TextProperty =
            {
                ContentProperty.Select(t => t ?? string.Empty)
            },
            FontProperty =
            {
                FontProperty
            },
            ColorProperty =
            {
                ColorProperty
            },
            HeightProperty =
            {
                HeightProperty
            },
            WidthProperty =
            {
                WidthProperty
            }
        };
    }

    public string Text
    {
        get => ContentProperty.Value ?? string.Empty;
        set => ContentProperty.Value = value;
    }

    public ReactiveProperty<string> TextProperty => ContentProperty!;

    public IFont Font
    {
        get => FontProperty.Value;
        set => FontProperty.Value = value;
    }

    public ReactiveProperty<IFont> FontProperty { get; set; }

    public Color Color
    {
        get => ColorProperty.Value;
        set => ColorProperty.Value = value;
    }

    public ReactiveProperty<Color> ColorProperty { get; set; }
}