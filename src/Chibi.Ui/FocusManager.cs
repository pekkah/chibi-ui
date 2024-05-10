using System.Collections.Generic;

namespace Chibi.Ui;

public class FocusManager
{
    private readonly Dictionary<string, IFocusable> _namedElements = new();
    private readonly SortedList<int, IFocusable> _focusOrder = new();
    private int _currentFocusIndex;

    public FocusManager(UiElement root)
    {
        FindFocusableElements(root);

        Current?.Focus();
    }

    private void FindFocusableElements(UiElement root)
    {
        var stack = new Stack<UiElement>();
        stack.Push(root);

        while (stack.Count > 0)
        {
            var element = stack.Pop();

            if (element is IFocusable focusable)
            {
                if (!string.IsNullOrEmpty(element.Name))
                {
                    _namedElements[element.Name] = focusable;
                }

                

                Add(focusable);
            }

            var childCount = element.GetChildCount();
            for (var i = 0; i < childCount; i++)
            {
                stack.Push(element.GetChild(i));
            }
        }

        void Add(IFocusable focusable)
        {
            if (!_focusOrder.TryAdd(focusable.TabIndex, focusable))
                for(var i = focusable.TabIndex +1; i < int.MaxValue; i++)
                {
                    if (_focusOrder.TryAdd(i, focusable))
                    {
                        break;
                    }
                }
        }
    }

    public IFocusable? Current => _focusOrder.Count > 0 ? _focusOrder.Values[_currentFocusIndex]: null;

    public void Next()
    {
        if (_focusOrder.Count == 0)
            return;

        Current?.Unfocus();

        _currentFocusIndex = (_currentFocusIndex + 1) % _focusOrder.Count;

        Current?.Focus();
    }

    public void Previous()
    {
        if (_focusOrder.Count == 0)
            return;

        Current?.Unfocus();

        _currentFocusIndex = (_currentFocusIndex - 1 + _focusOrder.Count) % _focusOrder.Count;

        Current?.Focus();
    }

    public void FocusByName(string name)
    {
        if (_namedElements.TryGetValue(name, out var focusable))
        {
            Current?.Unfocus();

            _currentFocusIndex = _focusOrder.IndexOfValue(focusable);

            Current?.Focus();
        }
    }

    public bool TrySetFocus(IFocusable focusable)
    {
        if (focusable is UiElement { Name: not null } element)
        {
            FocusByName(element.Name);
            return true;
        }

        return false;
    }
}