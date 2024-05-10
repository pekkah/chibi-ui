using System;

namespace Chibi.Ui.DataBinding;

internal class CombinedSubject<T>(IObserver<T> observer, IObservable<T> observable) : IObservable<T>, IObserver<T>
{
    public void OnCompleted() => observer.OnCompleted();

    public void OnError(Exception error) => observer.OnError(error);

    public void OnNext(T value) => observer.OnNext(value);

    public IDisposable Subscribe(IObserver<T> observer) => observable.Subscribe(observer);
}
