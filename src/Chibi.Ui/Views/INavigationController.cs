namespace Chibi.Ui.Views;

public interface INavigationController
{
    void Navigate<T>() where T: IViewController;
}