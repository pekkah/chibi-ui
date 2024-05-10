using Chibi.Ui;
using Chibi.Weather.Shared.Views;

namespace Chibi.Weather.Shared;

public static partial class W
{
    public static InputManager<Actions> CreateInputManager(ViewManager viewManager)
    {
        var inputManager = new InputManager<Actions>();
        inputManager.AddAction(Actions.Back, viewManager.OnBack);
        inputManager.AddAction(Actions.Up, viewManager.OnUp);
        inputManager.AddAction(Actions.Down, viewManager.OnDown);
        inputManager.AddAction(Actions.Left, viewManager.OnLeft);
        inputManager.AddAction(Actions.Right, viewManager.OnRight);
        inputManager.AddAction(Actions.Enter, viewManager.OnEnter);

        return inputManager;
    }
}