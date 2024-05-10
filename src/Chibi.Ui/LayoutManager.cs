namespace Chibi.Ui;

public class LayoutManager
{
    public static bool Measure(UiElement control, Size size, bool isRoot = false)
    {
        if (!control.IsVisible)
            return false;

        // Controls closest to the visual root need to be arranged first. We don't try to store
        // ordered invalidation lists, instead we traverse the tree upwards, measuring the
        // controls closest to the root first. This has been shown by benchmarks to be the
        // fastest and most memory-efficient algorithm.
        if (control.ParentElement is { } parent)
        {
            if (!Measure(parent, size))
                return false;
        }

        // If the control being measured has IsMeasureValid == true here then its measure was
        // handed by an ancestor and can be ignored. The measure may have also caused the
        // control to be removed.
        if (!control.IsMeasureValid)
        {
            if (isRoot)
            {
                control.Measure(size);
            }
            else if (control.PreviousMeasure.HasValue)
            {
                control.Measure(control.PreviousMeasure.Value);
            }
        }

        return true;
    }

    public static ArrangeResult Arrange(UiElement control, Rect finalRect, bool isRoot = false)
    {
        if (!control.IsVisible)
            return ArrangeResult.NotVisible;

        if (control.ParentElement is { } parent)
        {
            if (Arrange(parent, finalRect) is var parentResult && parentResult != ArrangeResult.Arranged)
                return parentResult;
        }

        if (!control.IsMeasureValid)
            return ArrangeResult.AncestorMeasureInvalid;

        if (!control.IsArrangeValid)
        {
            if (isRoot)
                control.Arrange(finalRect);
            else if (control.PreviousArrange != null)
            {
                // Has been observed that PreviousArrange sometimes is null, probably a bug somewhere else.
                // Condition observed: control.VisualParent is Scrollbar, control is Border.
                control.Arrange(control.PreviousArrange.Value);
            }
        }

        return ArrangeResult.Arranged;
    }

    public enum ArrangeResult
    {
        Arranged,
        NotVisible,
        AncestorMeasureInvalid,
    }
}