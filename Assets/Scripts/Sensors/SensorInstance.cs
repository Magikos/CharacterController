using UnityEngine;

public class SensorInstance<TContext>
{
    private ISensor<TContext> _sensor;
    private SensorUpdateMode _currentMode;
    private int _frameInterval;
    private int _lastUpdateFrame;

    public SensorUpdateMode DefaultMode => _sensor.DefaultMode;

    public SensorInstance(ISensor<TContext> sensor)
    {
        _sensor = sensor;
        SetMode(_sensor.DefaultMode);
    }

    public void Initialize(TContext context)
    {
        _sensor.Initialize(context);
        _lastUpdateFrame = Time.frameCount;
    }

    protected bool ShouldUpdate()
    {
        if (_currentMode == SensorUpdateMode.Disabled)
            return false;

        return Time.frameCount - _lastUpdateFrame >= _frameInterval;
    }

    public void Update(TContext context)
    {
        if (!ShouldUpdate()) return;

        _sensor.UpdateSensor(context);
        _lastUpdateFrame = Time.frameCount;
    }

    public void SetMode(SensorUpdateMode mode)
    {
        if (_currentMode == mode) return;

        _currentMode = mode;
        UpdateFrameInterval();
    }

    public void RevertToDefault()
    {
        SetMode(_sensor.DefaultMode);
    }

    private void UpdateFrameInterval()
    {
        _frameInterval = _currentMode switch
        {
            SensorUpdateMode.EveryFrame => 1,
            SensorUpdateMode.Reduced => 2,
            SensorUpdateMode.Minimal => 6,
            SensorUpdateMode.Disabled => int.MaxValue,
            _ => 1
        };
    }
}
