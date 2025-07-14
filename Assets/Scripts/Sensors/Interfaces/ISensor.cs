using System;

public interface ISensor<TContext> : IDisposable
{
    void UpdateSensor(TContext context);
    SensorUpdateMode DefaultMode { get; }
    void Initialize(TContext context);
}
