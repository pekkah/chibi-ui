// This source file is adapted from the Windows Presentation Foundation project. 
// (https://github.com/dotnet/wpf/) 
// 
// Licensed to The Avalonia Project under MIT License, courtesy of The .NET Foundation.

using System;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public enum Dock
{
    Left = 0,
    Bottom,
    Right,
    Top
}

/// <summary>
///     A panel which arranges its children at the top, bottom, left, right or center.
/// </summary>
public class DockPanel : Panel
{ 
    public DockPanel()
    {
        LastChildFillProperty = Property(nameof(LastChildFill), true);
    }

    /// <summary>
    /// Defines the <see cref="LastChildFill"/> property.
    /// </summary>
    public ReactiveProperty<bool> LastChildFillProperty { get; }

    public bool LastChildFill
    {
        get => LastChildFillProperty.Value;
        set => LastChildFillProperty.Value = value;
    }

    public static string DockPropertyName = $"{typeof(DockPanel).FullName}.Dock";

    public static Dock GetDock(UiElement control)
    {
        return control.GetProperty<Dock>(DockPropertyName)?.Value ?? Dock.Top;
    }

    public static Action<ObservableObject> SetDock(Dock value)
    {
        return element => element.Property(DockPropertyName, value);
    }

    /// <summary>
    /// Updates DesiredSize of the DockPanel.  Called by parent Control.  This is the first pass of layout.
    /// </summary>
    /// <remarks>
    /// Children are measured based on their sizing properties and <see cref="Dock" />.  
    /// Each child is allowed to consume all of the space on the side on which it is docked; Left/Right docked
    /// children are granted all vertical space for their entire width, and Top/Bottom docked children are
    /// granted all horizontal space for their entire height.
    /// </remarks>
    /// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
    /// <returns>The Panel's desired size.</returns>
    protected override Size MeasureOverride(Size constraint)
    {
        var children = Children;

        int parentWidth = 0;   // Our current required width due to children thus far.
        int parentHeight = 0;   // Our current required height due to children thus far.
        int accumulatedWidth = 0;   // Total width consumed by children.
        int accumulatedHeight = 0;   // Total height consumed by children.

        for (int i = 0, count = children.Count; i < count; ++i)
        {
            var child = children[i];
            Size childConstraint;             // Contains the suggested input constraint for this child.
            Size childDesiredSize;            // Contains the return size from child measure.

            // Child constraint is the remaining size; this is total size minus size consumed by previous children.
            childConstraint = new Size(Math.Max(0, constraint.Width - accumulatedWidth),
                Math.Max(0, constraint.Height - accumulatedHeight));

            // Measure child.
            child.Measure(childConstraint);
            childDesiredSize = child.DesiredSize;

            // Now, we adjust:
            // 1. Size consumed by children (accumulatedSize).  This will be used when computing subsequent
            //    children to determine how much space is remaining for them.
            // 2. Parent size implied by this child (parentSize) when added to the current children (accumulatedSize).
            //    This is different from the size above in one respect: A Dock.Left child implies a height, but does
            //    not actually consume any height for subsequent children.
            // If we accumulate size in a given dimension, the next child (or the end conditions after the child loop)
            // will deal with computing our minimum size (parentSize) due to that accumulation.
            // Therefore, we only need to compute our minimum size (parentSize) in dimensions that this child does
            //   not accumulate: Width for Top/Bottom, Height for Left/Right.
            switch (GetDock(child))
            {
                case Dock.Left:
                case Dock.Right:
                    parentHeight = Math.Max(parentHeight, accumulatedHeight + childDesiredSize.Height);
                    accumulatedWidth += childDesiredSize.Width;
                    break;

                case Dock.Top:
                case Dock.Bottom:
                    parentWidth = Math.Max(parentWidth, accumulatedWidth + childDesiredSize.Width);
                    accumulatedHeight += childDesiredSize.Height;
                    break;
            }
        }

        // Make sure the final accumulated size is reflected in parentSize.
        parentWidth = Math.Max(parentWidth, accumulatedWidth);
        parentHeight = Math.Max(parentHeight, accumulatedHeight);

        return (new Size(parentWidth, parentHeight));
    }

    /// <summary>
    /// DockPanel computes a position and final size for each of its children based upon their
    /// <see cref="Dock" /> enum and sizing properties.
    /// </summary>
    /// <param name="arrangeSize">Size that DockPanel will assume to position children.</param>
    protected override Size ArrangeOverride(Size arrangeSize)
    {
        var children = Children;
        int totalChildrenCount = children.Count;
        int nonFillChildrenCount = totalChildrenCount - (LastChildFill ? 1 : 0);

        int accumulatedLeft = 0;
        int accumulatedTop = 0;
        int accumulatedRight = 0;
        int accumulatedBottom = 0;

        for (int i = 0; i < totalChildrenCount; ++i)
        {
            var child = children[i];

            Size childDesiredSize = child.DesiredSize;
            Rect rcChild = new Rect(
                accumulatedLeft,
                accumulatedTop,
                Math.Max(0, arrangeSize.Width - (accumulatedLeft + accumulatedRight)),
                Math.Max(0, arrangeSize.Height - (accumulatedTop + accumulatedBottom)));

            if (i < nonFillChildrenCount)
            {
                switch (GetDock(child))
                {
                    case Dock.Left:
                        accumulatedLeft += childDesiredSize.Width;
                        rcChild = rcChild.WithWidth(childDesiredSize.Width);
                        break;

                    case Dock.Right:
                        accumulatedRight += childDesiredSize.Width;
                        rcChild = rcChild.WithX(Math.Max(0, arrangeSize.Width - accumulatedRight));
                        rcChild = rcChild.WithWidth(childDesiredSize.Width);
                        break;

                    case Dock.Top:
                        accumulatedTop += childDesiredSize.Height;
                        rcChild = rcChild.WithHeight(childDesiredSize.Height);
                        break;

                    case Dock.Bottom:
                        accumulatedBottom += childDesiredSize.Height;
                        rcChild = rcChild.WithY(Math.Max(0, arrangeSize.Height - accumulatedBottom));
                        rcChild = rcChild.WithHeight(childDesiredSize.Height);
                        break;
                }
            }

            child.Arrange(rcChild);
        }

        return (arrangeSize);
    }
}