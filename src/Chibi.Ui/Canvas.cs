using Chibi.Ui.DataBinding;
using System;

namespace Chibi.Ui;

public class Canvas : Panel
{
    public static string XPropertyName = $"{typeof(DockPanel).FullName}.X";
    public static string YPropertyName = $"{typeof(Canvas).FullName}.Y";

    public static int? GetX(UiElement control)
    {
        return control.GetProperty<int?>(XPropertyName)?.Value;
    }

    public static Action<ObservableObject> SetX(int? x)
    {
        return element => element.Property(XPropertyName, x);
    }

    public static int? GetY(UiElement control)
    {
        return control.GetProperty<int?>(YPropertyName)?.Value;
    }

    public static Action<ObservableObject> SetY(int? x)
    {
        return element => element.Property(YPropertyName, x);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return base.MeasureOverride(availableSize);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            var x = GetX(child);
            var y = GetY(child);
            if (x.HasValue && y.HasValue)
            {
                child.Arrange(new Rect(x.Value, y.Value, child.DesiredSize.Width, child.DesiredSize.Height));
            }
        }

        return base.ArrangeOverride(finalSize);
    }
}