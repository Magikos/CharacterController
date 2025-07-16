using UnityEngine;

/// <summary>
/// Concrete implementation of ICharacterContext using structured sub-contexts.
/// Organized for clarity and future refactor into interfaces as needed.
/// </summary>
public class CharacterContext
{
    // --- Subcontexts ---
    public CharacterConfigContext Config { get; set; } = new CharacterConfigContext();
    public CharacterInputContext Input { get; } = new CharacterInputContext();
    public CharacterSensorContext Sensor { get; } = new CharacterSensorContext();
    public CharacterIntentContext Intent { get; } = new CharacterIntentContext();
    public CharacterMotorContext Motor { get; } = new CharacterMotorContext();
    public CharacterVitalsSystem Vitals { get; } = new CharacterVitalsSystem();
    public CharacterReferences References { get; } = new CharacterReferences();

    // --- Stats or Config ---
    public CharacterStats Stats { get; set; } = new();

    public void Initialize(GameObject owner)
    {
        References.Initialize(owner); //first to ensure all references are set up

        Config.Initialize(owner);
        Input.Initialize(owner);
        Sensor.Initialize(owner);
        Intent.Initialize(owner);
        Motor.Initialize(owner);
    }

    public void Dispose()
    {
        Input.Dispose();
        Sensor.Dispose();
        Intent.Dispose();
        Motor.Dispose();
        Vitals.Dispose();
        References.Dispose();
    }
}
