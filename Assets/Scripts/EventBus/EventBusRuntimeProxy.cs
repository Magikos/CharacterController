using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using System;

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

    public static void BindEvents(object target)
    {
        if (target == null) return;

        var type = target.GetType();
        var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        foreach (var method in methods)
        {
            foreach (var attr in method.GetCustomAttributes(true))
            {
                if (attr is HandleEventBusAttribute busAttr)
                {
                    var eventType = busAttr.EventType;
                    var parameters = method.GetParameters();
                    if (parameters.Length != 1 || parameters[0].ParameterType != eventType)
                    {
                        Debug.LogError($"[EventBus] Invalid handler '{method.Name}' on {type.Name}. Expected parameter of type {eventType.Name}.");
                        continue;
                    }

                    var delegateType = typeof(Action<>).MakeGenericType(eventType);
                    var handler = Delegate.CreateDelegate(delegateType, target, method);

                    var busType = typeof(EventBus<>).MakeGenericType(eventType);

                    string methodName = busAttr.OneTime
                        ? (busAttr.Deferred ? "ListenOnceDeferred" : "ListenOnce")
                        : (busAttr.Deferred ? "ListenDeferred" : "Listen");

                    var listenMethod = busType.GetMethod(methodName, new[] { delegateType });
                    listenMethod?.Invoke(null, new object[] { handler });
                }
            }
        }
    }
}
