using System.Collections.Generic;
using UnityEngine;
using System;

public interface ISensorManager<TContext> : IDisposable
{
    void Initialize(TContext context);
    void UpdateSensors(TContext context);
    SensorManager<TContext> WithMonoSensor<T>() where T : MonoBehaviour, ISensor<TContext>;
    SensorManager<TContext> WithSensor(IEnumerable<ISensor<TContext>> sensors);
    SensorManager<TContext> WithSensor(params ISensor<TContext>[] sensors);
    SensorManager<TContext> WithTransition(IEnumerable<SensorTransition<TContext>> transitions);
    SensorManager<TContext> WithTransition(params SensorTransition<TContext>[] transitions);
}
