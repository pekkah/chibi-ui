using System;
using System.Collections.Generic;
using Chibi.Ui.DataBinding;
using Meadow;
using Meadow.Hardware;

namespace Chibi.Ui;

public class InputManager<TActions>: IDisposable
{
    private readonly Dictionary<TActions, Action> _actions = new();

    private readonly List<IDisposable> _disposables = new();

    public void AddAction(TActions action, Action doAction)
    {
        _actions.Add(action, doAction);
    }

    public IDisposable MapAction(IObservable<IChangeResult<DigitalState>> port, TActions action)
    {
        var unsubscribe = port.Subscribe(r =>
        {
            if (r.New.State != true) return;

            if (_actions.TryGetValue(action, out var doAction)) doAction();
        });

        _disposables.Add(unsubscribe);
        return unsubscribe;
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }
}