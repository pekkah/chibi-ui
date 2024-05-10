using Chibi.Ui;
using Chibi.Ui.Views;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;

namespace Chibi.Weather.Shared.Views;

public class ViewManager(IGraphicsDevice graphicsDevice, int maxFps) : ViewManagerBase<WeatherViewBase>(graphicsDevice, maxFps)
{
    public void HandleTouch(TouchPoint point)
    {
        var targetElement = Renderer.HitTest(CurrentView.Root, new Point(point.ScreenX, point.ScreenY));

        if (targetElement == null)
            return;

        var clickable = TreeHelper.FindReverse(targetElement, e => e is IClickable) as IClickable;

        switch (clickable)
        {
            case null:
                return;
            case IFocusable focusable:
                CurrentView.SetFocus(focusable);
                break;
        }

        clickable.Click();
    }

    public virtual void OnBack()
    {
        CurrentView.OnBack();
    }

    public virtual void OnUp()
    {
        CurrentView.OnUp();
    }

    public virtual void OnDown()
    {
        CurrentView.OnDown();
    }

    public virtual void OnLeft()
    {
        CurrentView.OnLeft();
    }

    public virtual void OnRight()
    {
        CurrentView.OnRight();
    }

    public virtual void OnEnter()
    {
        CurrentView.OnEnter();
    }
}