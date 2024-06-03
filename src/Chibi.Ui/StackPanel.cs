using System;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class StackPanel : Panel
{
    public StackPanel()
    {
        OrientationProperty = Property(nameof(Orientation), Orientation.Vertical);
        SpacingProperty = Property(nameof(Spacing), 0);
        AffectsArrange<int>(SpacingProperty);
        AffectsMeasure<int>(SpacingProperty);
        AffectsArrange<Orientation>(OrientationProperty);
        AffectsMeasure<Orientation>(OrientationProperty);
    }

    public ReactiveProperty<int> SpacingProperty { get; set; }

    /// <summary>
    /// Gets or sets the size of the spacing to place between child controls.
    /// </summary>
    public int Spacing
    {
        get => SpacingProperty.Value;
        set => SpacingProperty.Value = value;
    }

    /// <summary>
    /// Gets or sets the orientation in which child controls will be layed out.
    /// </summary>
    public Orientation Orientation
    {
        get => OrientationProperty.Value;
        set => OrientationProperty.Value = value;
    }

    public ReactiveProperty<Orientation> OrientationProperty { get; }

    /// <summary>
    /// General StackPanel layout behavior is to grow unbounded in the "stacking" direction (Size To Content).
    /// Children in this dimension are encouraged to be as large as they like.  In the other dimension,
    /// StackPanel will assume the maximum size of its children.
    /// </summary>
    /// <param name="availableSize">Constraint</param>
    /// <returns>Desired size</returns>
    protected override Size MeasureOverride(Size availableSize)
    {
        Size stackDesiredSize = new Size();
        var children = Children;
        Size layoutSlotSize = availableSize;
        bool fHorizontal = (Orientation == Orientation.Horizontal);
        int spacing = Spacing;
        bool hasVisibleChild = false;

        //
        // Initialize child sizing and iterator data
        // Allow children as much size as they want along the stack.
        //
        if (fHorizontal)
        {
            layoutSlotSize = layoutSlotSize.WithWidth(int.MaxValue);
        }
        else
        {
            layoutSlotSize = layoutSlotSize.WithHeight(int.MaxValue);
        }

        //
        //  Iterate through children.
        //  While we still supported virtualization, this was hidden in a child iterator (see source history).
        //
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            // Get next child.
            var child = children[i];

            bool isVisible = child.IsVisible;

            if (isVisible && !hasVisibleChild)
            {
                hasVisibleChild = true;
            }

            // Measure the child.
            child.Measure(layoutSlotSize);
            Size childDesiredSize = child.DesiredSize;

            // Accumulate child size.
            if (fHorizontal)
            {
                stackDesiredSize = stackDesiredSize.WithWidth(stackDesiredSize.Width + (isVisible ? spacing : 0) + childDesiredSize.Width);
                stackDesiredSize = stackDesiredSize.WithHeight(Math.Max(stackDesiredSize.Height, childDesiredSize.Height));
            }
            else
            {
                stackDesiredSize = stackDesiredSize.WithWidth(Math.Max(stackDesiredSize.Width, childDesiredSize.Width));
                stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height + (isVisible ? spacing : 0) + childDesiredSize.Height);
            }
        }

        if (fHorizontal)
        {
            stackDesiredSize = stackDesiredSize.WithWidth(stackDesiredSize.Width - (hasVisibleChild ? spacing : 0));
        }
        else
        {
            stackDesiredSize = stackDesiredSize.WithHeight(stackDesiredSize.Height - (hasVisibleChild ? spacing : 0));
        }

        return stackDesiredSize;
    }

    /// <summary>
    /// Content arrangement.
    /// </summary>
    /// <param name="finalSize">Arrange size</param>
    protected override Size ArrangeOverride(Size finalSize)
    {
        var children = Children;
        bool fHorizontal = (Orientation == Orientation.Horizontal);
        Rect rcChild = new Rect(finalSize);
        int previousChildSize = 0;
        var spacing = Spacing;

        //
        // Arrange and Position Children.
        //
        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];

            if (!child.IsVisible)
            {
                continue;
            }

            if (fHorizontal)
            {
                rcChild = rcChild.WithX(rcChild.X + previousChildSize);
                previousChildSize = child.DesiredSize.Width;
                rcChild = rcChild.WithWidth(previousChildSize);
                rcChild = rcChild.WithHeight(Math.Max(finalSize.Height, child.DesiredSize.Height));
                previousChildSize += spacing;
            }
            else
            {
                rcChild = rcChild.WithY(rcChild.Y + previousChildSize);
                previousChildSize = child.DesiredSize.Height;
                rcChild = rcChild.WithHeight(previousChildSize);
                rcChild = rcChild.WithWidth(Math.Max(finalSize.Width, child.DesiredSize.Width));
                previousChildSize += spacing;
            }

            ArrangeChild(child, rcChild, finalSize, Orientation);
        }

        return finalSize;
    }

    internal virtual void ArrangeChild(
        UiElement child,
        Rect rect,
        Size panelSize,
        Orientation orientation)
    {
        child.Arrange(rect);
    }
}