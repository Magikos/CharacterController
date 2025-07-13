using System;
using System.Collections.Generic;

public class SensorTransition<TContext>
{
    public Type SensorType = null;
    public SensorUpdateMode Mode = SensorUpdateMode.Disabled; // Default to disabled
    public Func<TContext, bool> Condition = _ => false; // Transition never activates unless set
}
