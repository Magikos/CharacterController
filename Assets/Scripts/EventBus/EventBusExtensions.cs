public static class EventBusExtensions
{
    public static T BindEvents<T>(this T instance) where T : IAutoEventBind
    {
        EventBusRuntimeProxy.BindEvents(instance);
        return instance;
    }
}
