using System;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class HandleEventBusAttribute : Attribute
{
    public Type EventType { get; }
    public bool Deferred { get; set; } = false;
    public bool OneTime { get; set; } = false;

    public HandleEventBusAttribute(Type eventType)
    {
        EventType = eventType;
    }
}
