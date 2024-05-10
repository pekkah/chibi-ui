using System;

namespace Chibi.Ui.DataBinding;

public static class ObservableExtensions
{
    public static IDisposable BindTo<T>(this IObserver<T> observer, IObservable<T> observable)
    {
        return observable.Subscribe(observer);
    }
}