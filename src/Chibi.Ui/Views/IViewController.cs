namespace Chibi.Ui.Views;

public interface IViewController
{
    UiElement Content { get; }

    FocusManager FocusManager { get; }

    LayoutManager LayoutManager { get; }

    void Render(Renderer renderer);

    void SetFocus(IFocusable focusable);

    void Unload();

    void Load();

    void Loaded();

    void InvalidateLayout();

    Size AllocatedSize { get; set; }
}