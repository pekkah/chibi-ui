using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Chibi.Ui.DataBinding;

namespace Chibi.Ui;

public class Panel : UiElement
{
    public Panel()
    {
        ChildrenProperty = Property(nameof(Children), new ObservableCollection<UiElement>());
        ChildrenProperty.Subscribe(OnChildrenChanged);
    }

    public ReactiveProperty<ObservableCollection<UiElement>> ChildrenProperty { get; }

    public ObservableCollection<UiElement> Children
    {
        get => ChildrenProperty.Value;
        set => ChildrenProperty.Value = value;
    }

    private readonly List<IDisposable> _childDesiredSizeChanged = new();

    private void OnChildrenChanged(PropertyChangedEvent<ObservableCollection<UiElement>> children)
    {
        children.OldValue.CollectionChanged -= OnChildrenCollectionChanged;
        children.Value.CollectionChanged += OnChildrenCollectionChanged;

        OnChildrenCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, children.Value, children.OldValue));
    }

    protected virtual void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.OldItems != null)
        {
            foreach (var oldItem in e.OldItems.OfType<UiElement>())
            {
                oldItem.ParentElement = null;
                oldItem.RootElement = null;
            }
            _childDesiredSizeChanged.ForEach(x => x.Dispose());
            _childDesiredSizeChanged.Clear();
        }

        if (e.NewItems != null)
            foreach (var newItem in e.NewItems.OfType<UiElement>())
            {
                newItem.ParentElement = this;
                newItem.RootElement = RootElement;
                _childDesiredSizeChanged.Add(newItem.DesiredSizeProperty.Subscribe(_ =>
                {
                    InvalidateMeasure();
                    InvalidateArrange();
                }));
            }

        InvalidateMeasure();
        InvalidateArrange();
    }

    public override int GetChildCount()
    {
        return Children.Count;
    }

    public override UiElement GetChild(int index)
    {
        return Children[index];
    }
}