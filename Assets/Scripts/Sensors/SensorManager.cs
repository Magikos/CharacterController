using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SensorManager<TContext> : ISensorManager<TContext>
{
    private Dictionary<Type, SensorInstance<TContext>> _sensors = new Dictionary<Type, SensorInstance<TContext>>();
    private List<SensorTransition<TContext>> _transitions = new List<SensorTransition<TContext>>();
    private Transform _transform;
    private bool _initialized = false;

    public SensorManager(Transform transform = null)
    {
        _transform = transform;
    }

    // For MonoBehaviour sensors
    public SensorManager<TContext> WithMonoSensor<T>() where T : MonoBehaviour, ISensor<TContext>
    {
        if (_transform == null)
            throw new InvalidOperationException("Transform required for MonoBehaviour sensors");

        var sensor = _transform.GetComponent<T>();
        if (sensor == null)
        {
            throw new InvalidOperationException($"Component {typeof(T).Name} not found on {_transform.name}");
        }

        return WithSensor(sensor);
    }

    // For regular class sensors
    public SensorManager<TContext> WithSensor(IEnumerable<ISensor<TContext>> sensors) => WithSensor(sensors.ToArray());
    public SensorManager<TContext> WithSensor(params ISensor<TContext>[] sensors)
    {
        _sensors = sensors.ToDictionary(s => s.GetType(), s => new SensorInstance<TContext>(s));
        return this;
    }

    public SensorManager<TContext> WithTransition(IEnumerable<SensorTransition<TContext>> transitions) => WithTransition(transitions.ToArray());
    public SensorManager<TContext> WithTransition(params SensorTransition<TContext>[] transitions)
    {
        _transitions.AddRange(transitions);
        return this;
    }

    public void Initialize(TContext context)
    {
        if (_initialized) return;

        // Initialize all sensors
        foreach (var sensorInstance in _sensors)
        {
            sensorInstance.Value.Initialize(context);
        }

        _initialized = true;
    }

    public void UpdateSensors(TContext context)
    {
        if (!_initialized) throw new InvalidOperationException("SensorManager must be initialized before updating sensors");

        foreach (var pair in _sensors)
        {
            var sensorType = pair.Key;
            var sensor = pair.Value;
            var mode = _transitions
                .Where(t => t.SensorType == sensorType && t.Condition(context))
                .FirstOrDefault()?.Mode ?? sensor.DefaultMode;

            sensor.SetMode(mode);
            sensor.Update(context);
        }
    }
}
