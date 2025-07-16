using System;

public interface ISensor<in TContext> : IDisposable
{
    SensorUpdateMode DefaultMode { get; }
    void UpdateSensor(TContext context);
    void Initialize(TContext context);
}
