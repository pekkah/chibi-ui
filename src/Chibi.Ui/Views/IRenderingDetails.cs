using Chibi.Ui.DataBinding;

namespace Chibi.Ui.Views;

public interface IRenderingDetails
{
    public ReactiveProperty<int> Fps { get; }
}