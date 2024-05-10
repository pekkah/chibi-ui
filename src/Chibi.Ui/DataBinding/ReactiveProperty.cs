using System;
using System.Collections;
using System.Collections.Generic;

namespace Chibi.Ui.DataBinding;

public interface IReactiveProperty
{
    void UnbindAll();

    void BindParent(ObservableObject parent);
}

public class ReactiveProperty<TValue>(string name, TValue initialValue) 
    : LightweightObservableBase<PropertyChangedEvent<TValue>>, 
        IObserver<PropertyChangedEvent<TValue>>,
        IObserver<TValue>,
        IObservable<TValue>,
        IEnumerable, 
        IReactiveProperty
{

    private IDisposable? _parentBinding;
    private List<IDisposable> _bindings = new();

    public TValue Value
    {
        get => initialValue;
        set
        {
            var oldValue = initialValue;
            initialValue = value;
            PublishNext(new PropertyChangedEvent<TValue>(name, initialValue, oldValue));
        }
    }

    public void UnbindAll()
    {
        foreach (var binding in _bindings)
        {
            binding.Dispose();
        }
    }

    public void BindParent(ObservableObject parent)
    {
        _parentBinding = base.Subscribe(new AnonymousObserver<PropertyChangedEvent<TValue>>(
            parent.OnNext,
            parent.OnError,
            parent.OnCompleted
            ));
    }

    protected override void Subscribed(IObserver<PropertyChangedEvent<TValue>> observer, bool first)
    {
        observer.OnNext(new PropertyChangedEvent<TValue>(name, initialValue, initialValue));
    }

    protected override void Initialize()
    {
    }

    protected override void Deinitialize()
    {
    }

    public void Add(IObservable<TValue> observable)
    {
        observable.Subscribe(this);
    }

    public IEnumerator GetEnumerator()
    {
        yield break;
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
        throw error;
    }

    public void OnNext(TValue value)
    {
        Value = value;
    }

    public void OnNext(PropertyChangedEvent<TValue> value)
    {
        Value = value.Value;
    }

    public override IDisposable Subscribe(IObserver<PropertyChangedEvent<TValue>> observer)
    {
        var binding = base.Subscribe(observer);
        _bindings.Add(binding);

        return binding;
    }

    public IDisposable Subscribe(IObserver<TValue> observer)
    {
        var binding = Subscribe(new AnonymousObserver<PropertyChangedEvent<TValue>>(
            next =>
            {
                observer.OnNext(next.Value);
            },
            observer.OnError,
            observer.OnCompleted));

        return binding;
    }

    public IDisposable Subscribe(Action<PropertyChangedEvent<TValue>> onNext)
    {
        return Subscribe(new AnonymousObserver<PropertyChangedEvent<TValue>>(onNext));
    }

    public IObservable<TResult> Select<TResult>(Func<TValue, TResult> selector)
    {
        return ((IObservable<TValue>)this).Select(selector);
    }
}

public class PropertyChangedEvent<T>(string name ,T value, T oldValue)
{
    public string Name { get; } = name;

    public T Value { get; } = value;

    public T OldValue { get; } = oldValue;
}