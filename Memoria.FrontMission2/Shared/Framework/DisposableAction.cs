using System;

namespace Memoria.FrontMission2.Shared.Framework;

public class DisposableAction : IDisposable
{
    private readonly Action _action;

    public DisposableAction(Action action)
    {
        _action = action;
    }

    public void Dispose()
    {
        _action();
    }
}