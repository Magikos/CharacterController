public struct StaminaChangedEvent : IGameEvent
{
    public float OldValue { get; }
    public float NewValue { get; }

    public StaminaChangedEvent(float oldValue, float newValue)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}