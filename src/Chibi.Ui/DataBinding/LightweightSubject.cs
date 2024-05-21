using System;

namespace Chibi.Ui.DataBinding;

public class LightweightSubject<T> : LightweightObservableBase<T>
{
    protected override void Initialize()
    {
    }

    protected override void Deinitialize()
    {
    }

    public void Next(T value)
    {
        base.PublishNext(value);
    }

    public void Complete()
    {
        base.PublishCompleted();
    }

    public void Error(Exception error)
    {
        base.PublishError(error);
    }
}