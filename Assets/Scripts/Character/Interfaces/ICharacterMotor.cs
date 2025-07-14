using System;

/// <summary>
/// Responsible for physically moving the character in the world.
/// </summary>
public interface ICharacterMotor : IDisposable
{
    void Initialize(ICharacterContext context);
    void ApplyMotion(ICharacterContext context);
}
