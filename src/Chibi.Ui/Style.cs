using System;
using System.Collections;
using System.Collections.Generic;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class Style<T>: ObservableObject, IEnumerable where T : UiElement
{
    private List<Func<T, Action>> _setters = new();
    private List<Action> _unsetters = new();

    public void Apply(T element)
    {
        foreach (var setter in _setters)
        {
            var unset = setter(element);
            _unsetters.Add(unset);
        }
    }

    public void Add<TV>(Func<T, ReactiveProperty<TV>> getProperty, TV value)
    {
        Func<T, Action> apply = (T target) =>
        {
            var property = getProperty(target);
            var previousValue = property.Value;
            property.Value = value;
            return  () => property.Value = previousValue;
        };

        _setters.Add(apply);
    }

    public void Remove(T element)
    {
        foreach (var unset in _unsetters)
        {
            unset();
        }
    }

    public IEnumerator GetEnumerator()
    {
        yield break;
    }
}