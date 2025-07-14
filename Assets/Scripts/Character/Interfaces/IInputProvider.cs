using System;

/// <summary>
/// Provides raw input to the character system.
/// </summary>
public interface IInputProvider : IDisposable
{
    void Initialize(ICharacterContext context);
    void UpdateInput(ICharacterContext context);
}
