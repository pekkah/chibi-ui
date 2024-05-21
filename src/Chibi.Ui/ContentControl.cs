using System;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class ContentControl<TContent>: UiElement
{
    public ContentControl()
    {
        PaddingProperty = Property(nameof(Padding), default(Thickness));
        HorizontalContentAlignmentProperty = Property(nameof(HorizontalContentAlignment), HorizontalAlignment.Stretch);
        VerticalContentAlignmentProperty = Property(nameof(VerticalContentAlignment), VerticalAlignment.Stretch);
        ContentProperty = Property(nameof(Content), default(TContent));
        ContentPresenterProperty = Property(nameof(ContentPresenter), default(UiElement));
        ContentProperty.Subscribe(OnContentChanged);
    }

    private IDisposable? _contentDesiredSizeChanged;
    private void OnContentChanged(PropertyChangedEvent<TContent?> ev)
    {
        if (ev.OldValue is UiElement oldElement)
        {
            oldElement.ParentElement = null;
            oldElement.RootElement = null;
            _contentDesiredSizeChanged?.Dispose();
        }

        if (ev.Value is UiElement element)
        {
            element.ParentElement = this;
            element.RootElement = RootElement;
            ContentPresenter = element;
            _contentDesiredSizeChanged = element.DesiredSizeProperty.Subscribe(_ => InvalidateMeasure());
        }


        InvalidateMeasure();
    }

    public ReactiveProperty<Thickness> PaddingProperty { get; }

    public Thickness Padding
    {
        get => PaddingProperty.Value;
        set => PaddingProperty.Value = value;
    }

    public ReactiveProperty<HorizontalAlignment> HorizontalContentAlignmentProperty { get; }

    public HorizontalAlignment HorizontalContentAlignment
    {
        get => HorizontalContentAlignmentProperty.Value;
        set => HorizontalContentAlignmentProperty.Value = value;
    }

    public ReactiveProperty<VerticalAlignment> VerticalContentAlignmentProperty { get; }

    public VerticalAlignment VerticalContentAlignment
    {
        get => VerticalContentAlignmentProperty.Value;
        set => VerticalContentAlignmentProperty.Value = value;
    }

    public ReactiveProperty<TContent?> ContentProperty { get; }

    public TContent? Content
    {
        get => ContentProperty.Value;
        set => ContentProperty.Value = value;
    }

    public ReactiveProperty<UiElement?> ContentPresenterProperty { get; }

    public UiElement? ContentPresenter
    {
        get => ContentPresenterProperty.Value;
        set
        {
            if (ContentPresenter != null)
                ContentPresenter.ParentElement = null;
            
            ContentPresenterProperty.Value = value;
        }
    }

    public override int GetChildCount()
    {
        return ContentPresenter != null ? 1 : 0;
    }
    
    public override UiElement GetChild(int index)
    {
        if (index != 0) throw new IndexOutOfRangeException();

        return ContentPresenter!;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        var width = Width ?? 0;
        var height = Height ?? 0;
        var padding = Padding;

        var sizeForChild = availableSize.Deflate(padding);

        if (ContentPresenter == null) return sizeForChild;
        ContentPresenter.Measure(sizeForChild);

        return new Size(
            Math.Max(width, ContentPresenter.DesiredSize.Width),
            Math.Max(height, ContentPresenter.DesiredSize.Height)
            );
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (ContentPresenter == null) return finalSize;

        var padding = Padding;
        var horizontalContentAlignment = HorizontalContentAlignment;
        var verticalContentAlignment = VerticalContentAlignment;
        var availableSize = finalSize;
        var sizeForChild = availableSize;
        var originX = 0;
        var originY = 0;

        if (horizontalContentAlignment != HorizontalAlignment.Stretch)
        {
            sizeForChild = sizeForChild.WithWidth(Math.Min(sizeForChild.Width, ContentPresenter.DesiredSize.Width));
        }

        if (verticalContentAlignment != VerticalAlignment.Stretch)
        {
            sizeForChild = sizeForChild.WithHeight(Math.Min(sizeForChild.Height, ContentPresenter.DesiredSize.Height));
        }

        switch (horizontalContentAlignment)
        {
            case HorizontalAlignment.Center:
                originX += (availableSize.Width - sizeForChild.Width) / 2;
                break;
            case HorizontalAlignment.Right:
                originX += availableSize.Width - sizeForChild.Width;
                break;
        }

        switch (verticalContentAlignment)
        {
            case VerticalAlignment.Center:
                originY += (availableSize.Height - sizeForChild.Height) / 2;
                break;
            case VerticalAlignment.Bottom:
                originY += availableSize.Height - sizeForChild.Height;
                break;
        }

        var boundsForChild =
            new Rect(originX, originY, sizeForChild.Width, sizeForChild.Height).Deflate(padding);

        ContentPresenter.Arrange(boundsForChild);

        return finalSize;
    }
}