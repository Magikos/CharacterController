
/// <summary>
/// Responsible for physically moving the character in the world.
/// </summary>
public interface ICharacterMotor
{
    void Initialize(ICharacterContext context);
    void ApplyMotion(ICharacterContext context);
}
