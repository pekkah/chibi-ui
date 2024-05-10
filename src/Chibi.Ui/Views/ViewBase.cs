using System;
using System.Diagnostics.CodeAnalysis;

namespace Chibi.Ui.Views;

public abstract class ViewBase : IViewController
{
    protected ViewBase()
    {
        Root = new TextBlock()
        {
            Text = GetType().Name,
        };
    }

    private UiElement _root;

    public UiElement Root
    {
        get => _root ?? throw new InvalidOperationException("Root not set");

        [MemberNotNull("FocusManager")]
        [MemberNotNull(nameof(_root))]
        set
        {
            _root = value;
            FocusManager = new FocusManager(_root);
        }
    }

    public FocusManager FocusManager { get; private set; }

    public virtual void Render(Renderer renderer)
    {
        renderer.Render(Root, renderer.DeviceBounds);
    }


    public virtual void SetFocus(IFocusable focusable)
    {
        FocusManager.TrySetFocus(focusable);
    }

    public virtual void Unload()
    {
        Root.Unload();
    }

    public virtual void Load()
    {
        Root.Load();
    }
}