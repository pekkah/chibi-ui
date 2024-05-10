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

    private void OnChildrenChanged(PropertyChangedEvent<ObservableCollection<UiElement>> children)
    {
        children.OldValue.CollectionChanged -= OnChildrenCollectionChanged;
        children.Value.CollectionChanged += OnChildrenCollectionChanged;
    }

    private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var oldItem in e.OldItems.OfType<UiElement>()) oldItem.DetachParent();

        foreach (var newItem in e.NewItems.OfType<UiElement>()) AttachChild(newItem);
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