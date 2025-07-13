
/// <summary>
/// Provides raw input to the character system.
/// </summary>
public interface IInputProvider
{
    void Initialize(ICharacterContext context);
    void UpdateInput(ICharacterContext context);
}
