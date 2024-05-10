using System;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class UiElement : ObservableObject
{
    private readonly ReactiveProperty<Size> _desiredSizeProperty;

    private readonly ReactiveProperty<bool> _isArrangeValidProperty;

    private readonly ReactiveProperty<bool> _isMeasureValidProperty;

    private bool _measuring;

    public UiElement()
    {
        IsDirtyProperty = Property(nameof(IsDirty), false);
        IsVisibleProperty = Property(nameof(IsVisible), true);
        WidthProperty = Property(nameof(Width), (int?)null);
        HeightProperty = Property(nameof(Height), (int?)null);
        MarginProperty = Property(nameof(Margin), new Thickness(0));
        VerticalAlignmentProperty = Property(nameof(VerticalAlignment), VerticalAlignment.Stretch);
        HorizontalAlignmentProperty = Property(nameof(HorizontalAlignment), HorizontalAlignment.Stretch);
        _desiredSizeProperty = Property(nameof(DesiredSize), default(Size));
        _isMeasureValidProperty = Property(nameof(IsMeasureValid), false);
        _isArrangeValidProperty = Property(nameof(IsArrangeValid), false);
        BackgroundProperty = Property(nameof(Background), default(IBrush));

        IsArrangeValidProperty.Subscribe(isArrangeValid =>
        {
            if (!isArrangeValid) InvalidateRender();
        });
    }

    public ReactiveProperty<bool> IsDirtyProperty { get; }

    public bool IsDirty
    {
        get => IsDirtyProperty.Value;
        set => IsDirtyProperty.Value = value;
    }

    /// <summary>
    ///     Gets the bounds of the control relative to its parent.
    /// </summary>
    public Rect Bounds { get; protected set; }

    /// <summary>
    ///     Gets or sets a value indicating whether this control is visible.
    /// </summary>
    public bool IsVisible
    {
        get => IsVisibleProperty.Value;
        set => IsVisibleProperty.Value = value;
    }

    public ReactiveProperty<bool> IsVisibleProperty { get; }

    internal UiElement? ParentElement { get; set; }

    /// <summary>
    ///     Gets or sets the width of the element.
    /// </summary>
    public int? Width
    {
        get => WidthProperty.Value;
        set => WidthProperty.Value = value;
    }

    public ReactiveProperty<int?> WidthProperty { get; }

    /// <summary>
    ///     Gets or sets the height of the element.
    /// </summary>
    public int? Height
    {
        get => HeightProperty.Value;
        set => HeightProperty.Value = value;
    }

    public ReactiveProperty<int?> HeightProperty { get; }

    /// <summary>
    ///     Gets or sets the minimum width of the element.
    /// </summary>
    public int MinWidth { get; set; }

    /// <summary>
    ///     Gets or sets the maximum width of the element.
    /// </summary>
    public int MaxWidth { get; set; } = int.MaxValue;

    /// <summary>
    ///     Gets or sets the minimum height of the element.
    /// </summary>
    public int MinHeight { get; set; }

    /// <summary>
    ///     Gets or sets the maximum height of the element.
    /// </summary>
    public int MaxHeight { get; set; } = int.MaxValue;

    /// <summary>
    ///     Gets or sets the margin around the element.
    /// </summary>
    public Thickness Margin
    {
        get => MarginProperty.Value;
        set => MarginProperty.Value = value;
    }

    public ReactiveProperty<Thickness> MarginProperty { get; }

    /// <summary>
    ///     Gets or sets the element's preferred horizontal alignment in its parent.
    /// </summary>
    public HorizontalAlignment HorizontalAlignment
    {
        get => HorizontalAlignmentProperty.Value;
        set => HorizontalAlignmentProperty.Value = value;
    }

    public ReactiveProperty<HorizontalAlignment> HorizontalAlignmentProperty { get; }

    /// <summary>
    ///     Gets or sets the element's preferred vertical alignment in its parent.
    /// </summary>
    public VerticalAlignment VerticalAlignment
    {
        get => VerticalAlignmentProperty.Value;
        set => VerticalAlignmentProperty.Value = value;
    }

    public ReactiveProperty<VerticalAlignment> VerticalAlignmentProperty { get; }

    /// <summary>
    ///     Gets the size that this element computed during the measure pass of the layout process.
    /// </summary>
    public Size DesiredSize
    {
        get => _desiredSizeProperty.Value;
        private set => _desiredSizeProperty.Value = value;
    }

    public IObservable<Size> DesiredSizeProperty => _desiredSizeProperty;

    /// <summary>
    ///     Gets a value indicating whether the control's layout measure is valid.
    /// </summary>
    public bool IsMeasureValid
    {
        get => _isMeasureValidProperty.Value;
        private set => _isMeasureValidProperty.Value = value;
    }

    public IObservable<bool> IsMeasureValidProperty => _isMeasureValidProperty;

    /// <summary>
    ///     Gets a value indicating whether the control's layouts arrange is valid.
    /// </summary>
    public bool IsArrangeValid
    {
        get => _isArrangeValidProperty.Value;
        private set => _isArrangeValidProperty.Value = value;
    }

    public IObservable<bool> IsArrangeValidProperty => _isArrangeValidProperty;

    public IBrush? Background
    {
        get => BackgroundProperty.Value;
        set => BackgroundProperty.Value = value;
    }

    public ReactiveProperty<IBrush?> BackgroundProperty { get; }

    /// <summary>
    ///     Gets the available size passed in the previous layout pass, if any.
    /// </summary>
    internal Size? PreviousMeasure { get; private set; }

    /// <summary>
    ///     Gets the layout rect passed in the previous layout pass, if any.
    /// </summary>
    internal Rect? PreviousArrange { get; private set; }

    public string? Name { get; set; }

    public void AffectsRender<T>(IObservable<T> property)
    {
        property.Subscribe(_ => InvalidateRender());
    }

    protected void InvalidateRender()
    {
        IsDirty = true;
    }

    public void InvalidateMeasure()
    {
        if (!_measuring)
        {
            IsMeasureValid = false;
            DesiredSize = default;
            PreviousMeasure = null;
        }
    }

    public void InvalidateArrange()
    {
        IsArrangeValid = false;
        PreviousArrange = null;
    }

    public virtual void Render(IDrawingContext context, Rect bounds)
    {
        Background?.Draw(context, bounds);

        IsDirty = false;
    }

    public virtual int GetChildCount()
    {
        return 0;
    }

    public virtual UiElement GetChild(int index)
    {
        return null!;
    }

    public virtual T AttachChild<T>(T child) where T : UiElement
    {
        child.ParentElement = this;
        return child;
    }

    public virtual void DetachParent()
    {
        UnsubscribePropertySubscribers();

        var childCount = GetChildCount();
        for (var i = 0; i < childCount; i++) GetChild(i).DetachParent();
    }

    /// <summary>
    ///     Carries out a measure of the control.
    /// </summary>
    /// <param name="availableSize">The available size for the control.</param>
    public void Measure(Size availableSize)
    {
        if (!IsMeasureValid || PreviousMeasure != availableSize)
        {
            Size desiredSize;

            IsMeasureValid = true;

            try
            {
                _measuring = true;
                desiredSize = MeasureCore(availableSize);
            }
            catch (Exception)
            {
                IsMeasureValid = false;
                throw;
            }
            finally
            {
                _measuring = false;
            }

            if (IsInvalidSize(desiredSize))
                throw new InvalidOperationException("Invalid size returned for Measure.");

            DesiredSize = desiredSize;
            PreviousMeasure = availableSize;
        }
    }

    /// <summary>
    ///     Arranges the control and its children.
    /// </summary>
    /// <param name="rect">The control's new bounds.</param>
    public void Arrange(Rect rect)
    {
        if (IsInvalidRect(rect)) throw new InvalidOperationException("Invalid Arrange rectangle.");

        if (!IsMeasureValid) Measure(PreviousMeasure ?? rect.Size);

        if (!IsArrangeValid || PreviousArrange != rect)
        {
            IsArrangeValid = true;
            ArrangeCore(rect);
            PreviousArrange = rect;
        }
    }

    /// <summary>
    ///     The default implementation of the control's measure pass.
    /// </summary>
    /// <param name="availableSize">The size available to the control.</param>
    /// <returns>The desired size for the control.</returns>
    /// <remarks>
    ///     This method calls <see cref="MeasureOverride(Size)" /> which is probably the method you
    ///     want to override in order to modify a control's arrangement.
    /// </remarks>
    protected virtual Size MeasureCore(Size availableSize)
    {
        if (IsVisible)
        {
            var margin = Margin;
            var sizeWithMargins = availableSize.Deflate(margin);
            var measured = MeasureOverride(sizeWithMargins);

            var width = measured.Width;
            var height = measured.Height;

            width = Math.Min(width, MaxWidth);
            width = Math.Max(width, MinWidth);

            height = Math.Min(height, MaxHeight);
            height = Math.Max(height, MinHeight);


            width = Math.Min(width, availableSize.Width);
            height = Math.Min(height, availableSize.Height);

            return NonNegative(new Size(width, height).Inflate(margin));
        }

        return default;
    }

    /// <summary>
    ///     Measures the control and its child elements as part of a layout pass.
    /// </summary>
    /// <param name="availableSize">The size available to the control.</param>
    /// <returns>The desired size for the control.</returns>
    protected virtual Size MeasureOverride(Size availableSize)
    {
        var width = Width ?? (
            HorizontalAlignment == HorizontalAlignment.Stretch
                ? availableSize.Width
                : 0);

        var height = Height ?? (
            VerticalAlignment == VerticalAlignment.Stretch
                ? availableSize.Height
                : 0);

        var visualCount = GetChildCount();

        for (var i = 0; i < visualCount; i++)
        {
            var visual = GetChild(i);

            visual.Measure(availableSize);
            width = Math.Max(width, visual.DesiredSize.Width);
            height = Math.Max(height, visual.DesiredSize.Height);
        }

        return new Size(width, height);
    }

    /// <summary>
    ///     The default implementation of the control's arrange pass.
    /// </summary>
    /// <param name="finalRect">The control's new bounds.</param>
    /// <remarks>
    ///     This method calls <see cref="ArrangeOverride(Size)" /> which is probably the method you
    ///     want to override in order to modify a control's arrangement.
    /// </remarks>
    protected virtual void ArrangeCore(Rect finalRect)
    {
        if (IsVisible)
        {
            var margin = Margin;
            var originX = finalRect.X + margin.Left;
            var originY = finalRect.Y + margin.Top;

            var availableSizeMinusMargins = new Size(
                Math.Max(0, finalRect.Width - margin.Left - margin.Right),
                Math.Max(0, finalRect.Height - margin.Top - margin.Bottom));

            var horizontalAlignment = HorizontalAlignment;
            var verticalAlignment = VerticalAlignment;
            var size = availableSizeMinusMargins;

            if (horizontalAlignment != HorizontalAlignment.Stretch)
                size = size.WithWidth(Math.Min(size.Width, DesiredSize.Width - margin.Left - margin.Right));

            if (verticalAlignment != VerticalAlignment.Stretch)
                size = size.WithHeight(Math.Min(size.Height, DesiredSize.Height - margin.Top - margin.Bottom));

            size = ArrangeOverride(size).Constrain(size);

            switch (horizontalAlignment)
            {
                case HorizontalAlignment.Center:
                case HorizontalAlignment.Stretch:
                    originX += (availableSizeMinusMargins.Width - size.Width) / 2;
                    break;
                case HorizontalAlignment.Right:
                    originX += availableSizeMinusMargins.Width - size.Width;
                    break;
            }

            switch (verticalAlignment)
            {
                case VerticalAlignment.Center:
                case VerticalAlignment.Stretch:
                    originY += (availableSizeMinusMargins.Height - size.Height) / 2;
                    break;
                case VerticalAlignment.Bottom:
                    originY += availableSizeMinusMargins.Height - size.Height;
                    break;
            }

            Bounds = new Rect(originX, originY, size.Width, size.Height);
        }
    }

    /// <summary>
    ///     Positions child elements as part of a layout pass.
    /// </summary>
    /// <param name="finalSize">The size available to the control.</param>
    /// <returns>The actual size used.</returns>
    protected virtual Size ArrangeOverride(Size finalSize)
    {
        var arrangeRect = new Rect(finalSize);
        var visualCount = GetChildCount();

        for (var i = 0; i < visualCount; i++)
        {
            var visual = GetChild(i);
            visual.Arrange(arrangeRect);
        }

        return finalSize;
    }

    /// <summary>
    ///     Tests whether any of a <see cref="Rect" />'s properties include negative values,
    ///     a NaN or Infinity.
    /// </summary>
    /// <param name="rect">The rect.</param>
    /// <returns>True if the rect is invalid; otherwise false.</returns>
    private static bool IsInvalidRect(Rect rect)
    {
        return rect.Width < 0 || rect.Height < 0 ||
               double.IsInfinity(rect.X) || double.IsInfinity(rect.Y) ||
               double.IsInfinity(rect.Width) || double.IsInfinity(rect.Height) ||
               double.IsNaN(rect.X) || double.IsNaN(rect.Y) ||
               double.IsNaN(rect.Width) || double.IsNaN(rect.Height);
    }

    /// <summary>
    ///     Tests whether any of a <see cref="Size" />'s properties include negative values,
    ///     a NaN or Infinity.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>True if the size is invalid; otherwise false.</returns>
    private static bool IsInvalidSize(Size size)
    {
        return size.Width < 0 || size.Height < 0 ||
               double.IsInfinity(size.Width) || double.IsInfinity(size.Height) ||
               double.IsNaN(size.Width) || double.IsNaN(size.Height);
    }

    /// <summary>
    ///     Ensures neither component of a <see cref="Size" /> is negative.
    /// </summary>
    /// <param name="size">The size.</param>
    /// <returns>The non-negative size.</returns>
    private static Size NonNegative(Size size)
    {
        return new Size(Math.Max(size.Width, 0), Math.Max(size.Height, 0));
    }

    public virtual void Load()
    {
    }

    public virtual void Unload()
    {
    }
}