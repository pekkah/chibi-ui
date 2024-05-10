using System;

namespace Chibi.Ui.Helpers;

public static class LayoutHelper
{
    public static Size MeasureChild(UiElement? control, Size availableSize, Thickness padding, Thickness borderThickness)
    {
        if (control != null)
        {
            control.Measure(availableSize.Deflate(padding + borderThickness));
            return control.DesiredSize.Inflate(padding + borderThickness);
        }

        return new Size().Inflate(padding + borderThickness);
    }

    public static Size MeasureChild(UiElement? control, Size availableSize, Thickness padding)
    {
        if (control != null)
        {
            control.Measure(availableSize.Deflate(padding));
            return control.DesiredSize.Inflate(padding);
        }

        return new Size(padding.Left + padding.Right, padding.Bottom + padding.Top);
    }

    public static Size ArrangeChild(UiElement? child, Size availableSize, Thickness padding, Thickness borderThickness)
    {
        return ArrangeChildInternal(child, availableSize, padding + borderThickness);
    }

    public static Size ArrangeChild(UiElement? child, Size availableSize, Thickness padding)
    {
        return ArrangeChildInternal(child, availableSize, padding);
    }

    private static Size ArrangeChildInternal(UiElement? child, Size availableSize, Thickness padding)
    {
        child?.Arrange(new Rect(availableSize).Deflate(padding));

        return availableSize;
    }
}

/// <summary>
/// Calculates the min and max height for a control. Ported from WPF.
/// </summary>
public readonly struct MinMax
{
    public MinMax(UiElement e)
    {
        MaxHeight = e.MaxHeight;
        MinHeight = e.MinHeight;
        int? l = e.Height;

        int height = l ?? int.MaxValue;
        MaxHeight = Math.Max(Math.Min(height, MaxHeight), MinHeight);

        height = l ?? 0;
        MinHeight = Math.Max(Math.Min(MaxHeight, height), MinHeight);

        MaxWidth = e.MaxWidth;
        MinWidth = e.MinWidth;
        l = e.Width;

        int width = l ?? int.MaxValue;
        MaxWidth = Math.Max(Math.Min(width, MaxWidth), MinWidth);

        width = l ?? 0;
        MinWidth = Math.Max(Math.Min(MaxWidth, width), MinWidth);
    }

    public int MinWidth { get; }
    public int MaxWidth { get; }
    public int MinHeight { get; }
    public int MaxHeight { get; }
}