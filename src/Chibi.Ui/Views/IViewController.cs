namespace Chibi.Ui.Views;

public interface IViewController
{
    public UiElement Root { get; }

    void Render(Renderer renderer);

    void SetFocus(IFocusable focusable);

    void Unload();

    void Load();
}