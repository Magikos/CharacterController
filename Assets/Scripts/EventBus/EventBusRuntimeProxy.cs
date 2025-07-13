
using System;
using System.Collections.Generic;

internal static class EventBusRuntimeProxy
{
    private static readonly List<Action> _deferredProcessors = new();

    public static void RegisterProcessor(Action processor)
    {
        if (!_deferredProcessors.Contains(processor))
            _deferredProcessors.Add(processor);
    }

    public static void ProcessAll()
    {
        foreach (var processor in _deferredProcessors)
            processor?.Invoke();
    }
}
