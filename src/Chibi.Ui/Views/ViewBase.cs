using System;
using System.Diagnostics.CodeAnalysis;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui.Views;

public abstract class ViewBase : UiElement, IViewController, ILayoutRoot, IEmbeddedLayoutRoot
{
    protected ViewBase()
    {
        LayoutManager = new LayoutManager(this);
        ContentProperty = Property(nameof(Content), (UiElement)new TextBlock()
        {
            Text = GetType().Name,
        });

        ContentProperty.Subscribe(OnContentChanged);
    }

    private IDisposable? _contentChanged;

    [MemberNotNull("FocusManager")]
    private void OnContentChanged(PropertyChangedEvent<UiElement> obj)
    {
        if (obj.OldValue is not null)
        {
            obj.OldValue.ParentElement = null;
            obj.OldValue.RootElement = null;
            _contentChanged?.Dispose();
        }

        if (obj.Value is not null)
        {
            obj.Value.ParentElement = this;
            obj.Value.RootElement = this;
            _contentChanged = obj.Value.DesiredSizeProperty.Subscribe(_ =>
            {
                InvalidateLayout();
            });
            FocusManager = new FocusManager(obj.Value);
        }

        InvalidateLayout();
    }

    public ReactiveProperty<UiElement> ContentProperty { get;  }

    public UiElement Content
    {
        get => ContentProperty.Value;
        set => ContentProperty.Value = value;
    }

    public FocusManager FocusManager { get; private set; }

    public LayoutManager LayoutManager { get; private set; }

    public virtual void Render(Renderer renderer)
    {
        LayoutManager.ExecuteQueuedLayoutPass();
        renderer.Render(this);
    }


    public virtual void SetFocus(IFocusable focusable)
    {
        FocusManager.TrySetFocus(focusable);
    }

    public override void Unload()
    {
        Content.Unload();
        TreeHelper.Visit(this, (parent, child) => child.ParentElement = parent);
    }

    public override void Load()
    {
        TreeHelper.Visit(this, (parent, child) =>
        {
            child.ParentElement = parent;
            child.RootElement = this;
        });
        Content.Load();
        LayoutManager.InvalidateMeasure(this);
        LayoutManager.ExecuteInitialLayoutPass();
    }

    public virtual void Loaded()
    {
    }

    public void InvalidateLayout()
    {
        InvalidateMeasure();
        InvalidateArrange();
    }

    public override int GetChildCount()
    {
        return 1;
    }

    public override UiElement GetChild(int index)
    {
        return Content ?? throw new InvalidOperationException();
    }

    public Size AllocatedSize { get; set; }
}