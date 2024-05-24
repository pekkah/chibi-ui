namespace Chibi.Ui;

public interface IClickable
{
    void Click(HitTestResult? point = default);
}