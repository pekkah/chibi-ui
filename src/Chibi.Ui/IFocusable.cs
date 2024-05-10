namespace Chibi.Ui;

public interface IFocusable
{
    public int TabIndex { get; }

    void Focus();

    void Unfocus();
}