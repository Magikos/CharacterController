using System;

public class AirborneTransitionBuilder
{
    public static Type PickMovementState(CharacterContext context)
    {
        // Logic to determine the initial state based on context
        // For now, we default to FallingState
        return typeof(FallingState);
    }
}
