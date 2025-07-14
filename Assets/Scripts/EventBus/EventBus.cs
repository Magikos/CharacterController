using System;
using System.Collections.Generic;

public static class EventBus<TEvent> where TEvent : struct, IGameEvent
{
    private class Subscriber
    {
        public WeakReference Target;
        public Action<TEvent> Handler;
        public bool OneTime;
        public bool Deferred;

        public bool IsDead =>
            Target != null && (!Target.IsAlive || Target.Target == null);
    }

    private static readonly List<Subscriber> _subscribers = new();
    private static readonly Queue<Action> _deferredQueue = new();
    private static bool _registeredDeferredProcessor = false;

    public static void Raise(TEvent context)
    {
        for (int i = _subscribers.Count - 1; i >= 0; i--)
        {
            var sub = _subscribers[i];

            if (sub.IsDead)
            {
                _subscribers.RemoveAt(i);
                continue;
            }

            void Invoke() => sub.Handler?.Invoke(context);

            if (sub.Deferred)
                _deferredQueue.Enqueue(Invoke);
            else
                Invoke();

            if (sub.OneTime)
                _subscribers.RemoveAt(i);
        }
    }

    public static void Listen(Action<TEvent> handler, Func<TEvent, bool> filter = null)
        => AddSubscriber(handler, filter, oneTime: false, deferred: false);

    public static void ListenDeferred(Action<TEvent> handler, Func<TEvent, bool> filter = null)
        => AddSubscriber(handler, filter, oneTime: false, deferred: true);

    public static void ListenOnce(Action<TEvent> handler, Func<TEvent, bool> filter = null)
        => AddSubscriber(handler, filter, oneTime: true, deferred: false);

    public static void ListenOnceDeferred(Action<TEvent> handler, Func<TEvent, bool> filter = null)
        => AddSubscriber(handler, filter, oneTime: true, deferred: true);

    private static void AddSubscriber(Action<TEvent> handler, Func<TEvent, bool> filter, bool oneTime, bool deferred)
    {
        Action<TEvent> wrapped = filter == null ? handler : (ctx =>
        {
            if (filter(ctx)) handler(ctx);
        });

        var target = handler.Target != null ? new WeakReference(handler.Target) : null;

        _subscribers.Add(new Subscriber
        {
            Target = target,
            Handler = wrapped,
            OneTime = oneTime,
            Deferred = deferred
        });

        if (deferred)
            EnsureDeferredProcessorRegistered();
    }

    private static void EnsureDeferredProcessorRegistered()
    {
        if (_registeredDeferredProcessor) return;

        EventBusRuntimeProxy.RegisterProcessor(ProcessDeferred);
        _registeredDeferredProcessor = true;
    }

    public static void ClearAll() => _subscribers.Clear();

    internal static void ProcessDeferred()
    {
        while (_deferredQueue.Count > 0)
            _deferredQueue.Dequeue()?.Invoke();
    }

}
