using System;

namespace Chibi.Ui;

public interface ICommand
{
    void Execute();
}

public class InlineCommand(Action action) : ICommand
{
    public void Execute() => action();
}