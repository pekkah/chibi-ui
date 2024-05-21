using System;
using System.Diagnostics.CodeAnalysis;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class IfElement : UiElement
{
    public IfElement()
    {
        IfProperty = Property(nameof(If), false);
        TrueElementProperty = Property(nameof(TrueElement), (UiElement?)null);
        FalseElementProperty = Property(nameof(FalseElement), (UiElement?)null);

        TrueElementProperty.Subscribe(_ => Invalidate());
        FalseElementProperty.Subscribe(_ => Invalidate());
        IfProperty.Subscribe(_ => Invalidate());
    }

    [MemberNotNull("Content")]
    private void Invalidate()
    {
        Content = If ? TrueElement : FalseElement;
        IsVisible = Content != null;
        InvalidateMeasure();
        InvalidateArrange();
    }

    protected UiElement? Content { get; set; }

    public ReactiveProperty<UiElement?> FalseElementProperty { get; }

    public UiElement? FalseElement
    {
        get => FalseElementProperty.Value;
        set => FalseElementProperty.Value = value;
    }

    public ReactiveProperty<UiElement?> TrueElementProperty { get; }

    public UiElement? TrueElement
    {
        get => TrueElementProperty.Value;
        set => TrueElementProperty.Value = value;
    }

    public ReactiveProperty<bool> IfProperty { get;  }

    public bool If
    {
        get => IfProperty.Value;
        set => IfProperty.Value = value;
    }

    public override UiElement GetChild(int index)
    {
        return Content ?? throw new InvalidOperationException();
    }

    public override int GetChildCount()
    {
        return Content != null ? 1 : 0;
    }

    protected override Size MeasureCore(Size availableSize)
    {
        Content?.Measure(availableSize);
        return Content?.DesiredSize ?? new Size();
    }

    protected override void ArrangeCore(Rect finalRect)
    {
        if (Content == null)
        {
            Bounds = new Rect();
        }
        else
        {

            Content?.Arrange(finalRect);
            Bounds = finalRect;
        }
    }
}