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
        HeaderFontColorProperty = Property(nameof(HeaderFontColor), Color.Black);
        ValueFontColorProperty = Property(nameof(ValueFontColor), Color.Black);
        SpacingProperty = Property(nameof(Spacing), 5);
        HeaderFontProperty = Property(nameof(HeaderFont), (IFont)new Font12x16());
        ValueFontProperty = Property(nameof(ValueFont), (IFont)new Font12x16());

        Padding = new Thickness(2);
        Margin = new Thickness(2);

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
                    ContentProperty =
                    {
                        HeaderProperty
                    },
                    FontProperty =
                    {
                        HeaderFontProperty
                    },
                    ColorProperty =
                    {
                        HeaderFontColorProperty
                    }
                },
                new TextBlock
                {
                    ContentProperty =
                    {
                        ValueProperty
                    },
                    FontProperty =
                    {
                        ValueFontProperty
                    },
                    ColorProperty =
                    {
                        ValueFontColorProperty
                    }
                }
            ]
        };
    }

    public ReactiveProperty<IFont> ValueFontProperty { get; }

    public IFont ValueFont
    {
        get => ValueFontProperty.Value;
        set => ValueFontProperty.Value = value;
    }

    public ReactiveProperty<IFont> HeaderFontProperty { get; }

    public IFont HeaderFont
    {
        get => HeaderFontProperty.Value;
        set => HeaderFontProperty.Value = value;
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
        get => ValueFontColorProperty.Value;
        set => ValueFontColorProperty.Value = value;
    }

    public ReactiveProperty<Color> ValueFontColorProperty { get; }
}