
/// <summary>
/// Reads the environment around the character and updates the context.
/// </summary>
public interface IEnvironmentSensor
{
    void Initialize(ICharacterContext context);
    void UpdateSensor(ICharacterContext context);
}
