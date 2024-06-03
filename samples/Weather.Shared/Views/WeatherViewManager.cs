using Chibi.Ui.Views;
using Meadow.Hardware;

namespace Chibi.Ui.Weather.Shared.Views;

public class WeatherViewManager(
    IGraphicsDevice graphicsDevice, 
    int maxFps,
    ITouchScreen touchScreen) 
    : ViewManagerBase<WeatherViewBase>(graphicsDevice, maxFps, Theme.ScreenBackground)
{
    public void OnTouchUp(TouchPoint point)
    {
        var hitTestResult = Renderer.HitTest(
            CurrentView.Content, 
            point);

        if (hitTestResult == null)
            return;

        var targetElement = hitTestResult.Element;
        var clickable = TreeHelper.FindReverse(targetElement, e => e is IClickable) as IClickable;

        switch (clickable)
        {
            case null:
                return;
            case IFocusable focusable:
                CurrentView.SetFocus(focusable);
                break;
        }

        clickable.Click(hitTestResult);
    }

    public override void Draw()
    {
        if (touchScreen.IsTouched)
        {
            Pause();
        }
        else
        {
            Resume();
        }
        base.Draw();
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