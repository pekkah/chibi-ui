using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Foundation.Graphics;

namespace Chibi.Ui;

public class HeaderContentControl : UserControl
{
    public HeaderContentControl()
    {
        HeaderProperty = Property(nameof(Header), "");
        ValueProperty = Property(nameof(Value), "");
        OrientationProperty = Property(nameof(Orientation), Orientation.Vertical);
        HeaderFontColorProperty = Property(nameof(HeaderFontColor), new Color(244,113,181));
        ValueFontColorProperty = Property(nameof(ValueFontColor), new Color(208, 201, 203));
        SpacingProperty = Property(nameof(Spacing), 5);

        Padding = new Thickness(5);
        Margin = new Thickness(3);
        Height = 35;
        Background = new RectangleBrush()
        {
            Color = new Color(30, 41, 59),
            Filled = true,
            CornerRadius = 10,
        };

        Content = new StackPanel
        {
            OrientationProperty =
            {
                OrientationProperty
            },
            SpacingProperty =
            {
                SpacingProperty
            },
            Children =
            [
                new TextBlock
                {
                    Padding = new Thickness(5),
                    ContentProperty =
                    {
                        HeaderProperty
                    },
                    Font = new Font12x16(),
                    ColorProperty =
                    {
                        HeaderFontColorProperty
                    }
                },
                new TextBlock
                {
                    Padding = new Thickness(5),
                    ContentProperty =
                    {
                        ValueProperty
                    },
                    Font = new Font12x16(),
                    ColorProperty =
                    {
                        ValueFontColorProperty
                    }
                }
            ]
        };
    }

    public ReactiveProperty<int> SpacingProperty { get; }

    public int Spacing
    {
        get => SpacingProperty.Value;
        set => SpacingProperty.Value = value;
    }

    public ReactiveProperty<string> HeaderProperty { get; }

    public string Header
    {
        get => HeaderProperty.Value;
        set => HeaderProperty.Value = value;
    }

    public ReactiveProperty<string> ValueProperty { get; }

    public string Value
    {
        get => ValueProperty.Value;
        set => ValueProperty.Value = value;
    }

    public Orientation Orientation
    {
        get => OrientationProperty.Value;
        set => OrientationProperty.Value = value;
    }

    public ReactiveProperty<Orientation> OrientationProperty { get; }


    public Color HeaderFontColor
    {
        get => HeaderFontColorProperty.Value;
        set => HeaderFontColorProperty.Value = value;
    }

    public ReactiveProperty<Color> HeaderFontColorProperty { get; }

    public Color ValueFontColor
    {
        get => HeaderFontColorProperty.Value;
        set => HeaderFontColorProperty.Value = value;
    }

    public ReactiveProperty<Color> ValueFontColorProperty { get; }
}