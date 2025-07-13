public interface ISensor<TContext>
{
    void UpdateSensor(TContext context);
    SensorUpdateMode DefaultMode { get; }
    void Initialize(TContext context);
}
