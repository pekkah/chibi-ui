using System;
using Chibi.Ui.DataBinding;
using Meadow;

namespace Chibi.Ui;

public class Button : ContentControl<string>, IFocusable, IClickable
{
    private readonly ReactiveProperty<bool> _isFocusedProperty;

    public Button()
    {
        _isFocusedProperty = Property(nameof(IsFocused), false);
        FontColorProperty = Property(nameof(FontColor), Color.Black);
        TabIndexProperty = Property(nameof(TabIndex), 0);
        FocusStyleProperty = Property(nameof(FocusStyle), new Style<Button>
        {
            {
                _ => _.BackgroundProperty, new RectangleBrush
                {
                    Color = new Color(45, 212, 191),
                    Filled = true,
                    CornerRadius = 3
                }
            }
        });

        HorizontalContentAlignment = HorizontalAlignment.Center;
        VerticalContentAlignment = VerticalAlignment.Center;

        Background = new RectangleBrush
        {
            Color = new Color(45, 212, 191),
            Filled = false,
            CornerRadius = 3
        };

        ContentPresenter = new TextBlock
        {
            ParentElement = this,
            TextProperty =
            {
                TextProperty.Select(t => t ?? string.Empty)
            },
            ColorProperty =
            {
                FontColorProperty
            },
            VerticalContentAlignment = VerticalAlignment.Top,
            HorizontalContentAlignment = HorizontalAlignment.Left
        };

        _isFocusedProperty.Subscribe(ev =>
        {
            var focused = ev.Value;
            if (focused)
                FocusStyle.Apply(this);
            else
                FocusStyle.Remove(this);
        });
    }

    public Style<Button> FocusStyle
    {
        get => FocusStyleProperty.Value;
        set => FocusStyleProperty.Value = value;
    }

    public ReactiveProperty<Style<Button>> FocusStyleProperty { get; }

    public ReactiveProperty<int> TabIndexProperty { get; }

    public string Text
    {
        get => Content ?? string.Empty;
        set => Content = value;
    }

    public ReactiveProperty<string?> TextProperty => ContentProperty;

    public ReactiveProperty<Color> FontColorProperty { get; }

    public Color FontColor
    {
        get => FontColorProperty.Value;
        set => FontColorProperty.Value = value;
    }

    public bool IsFocused => _isFocusedProperty.Value;

    public IObservable<bool> IsFocusedProperty => _isFocusedProperty;

    public ICommand? Command { get; set; }

    public void Click()
    {
        Command?.Execute();
    }

    public int TabIndex
    {
        get => TabIndexProperty.Value;
        set => TabIndexProperty.Value = value;
    }


    public void Focus()
    {
        _isFocusedProperty.Value = true;
    }

    public void Unfocus()
    {
        _isFocusedProperty.Value = false;
    }
}