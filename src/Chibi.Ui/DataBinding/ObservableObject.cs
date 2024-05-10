using System;
using System.Collections.Generic;

namespace Chibi.Ui.DataBinding;

public abstract class
    ObservableObject : LightweightObservableBase<(string PropertyName, object? Value, object? OldValue)>
{
    private readonly Dictionary<string, IReactiveProperty> _properties = new();

    /// <summary>
    ///     Run an action on this object
    ///
    /// <remarks>
    ///     Main use is to attach properties to the object during construction.
    /// </remarks>
    /// </summary>
    public IEnumerable<Action<ObservableObject>> Attach
    {
        set
        {
            foreach (var action in value) action(this);
        }
    }

    /// <summary>
    ///     Create and bind or get a property to/of this object.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <param name="initialValue"></param>
    /// <returns></returns>
    public ReactiveProperty<T> Property<T>(string name, T initialValue)
    {
        if (_properties.TryGetValue(name, out var p))
            return (ReactiveProperty<T>)p;

        var property = new ReactiveProperty<T>(name, initialValue);
        property.BindParent(this);
        _properties[name] = property;
        return property;
    }

    public ReactiveProperty<TValue>? GetProperty<TValue>(string name)
    {
        if (_properties.TryGetValue(name, out var property))
            return property as ReactiveProperty<TValue>;

        return null;
    }

    /// <summary>
    ///     Unsubscribe all subscribers from all properties.
    ///     <remarks>
    ///         The parent will still be bound to the properties and
    ///         will receive updates.
    ///     </remarks>
    /// </summary>
    public void UnsubscribePropertySubscribers()
    {
        foreach (var (name, property) in _properties)
            property.UnbindAll();
    }

    protected override void Initialize()
    {
    }

    protected override void Deinitialize()
    {
    }

    public void OnCompleted()
    {
    }

    public void OnError(Exception error)
    {
        throw error;
    }

    public void OnNext<T>(PropertyChangedEvent<T> ev)
    {
        PublishNext((ev.Name, ev.Value, ev.OldValue));
    }
}