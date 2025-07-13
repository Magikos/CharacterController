using UnityEngine;

/// <summary>
/// Concrete implementation of ICharacterContext using structured sub-contexts.
/// Organized for clarity and future refactor into interfaces as needed.
/// </summary>
public class CharacterContext : ICharacterContext
{
    // --- Timing ---
    public float DeltaTime { get; set; }
    public float FixedDeltaTime { get; set; }

    // --- Subcontexts ---
    public ICharacterInputContext Input { get; } = new CharacterInputContext();
    public ICharacterSensorContext Sensor { get; } = new CharacterSensorContext();
    public ICharacterIntentContext Intent { get; } = new CharacterIntentContext();
    public ICharacterMotorContext Motor { get; } = new CharacterMotorContext();
    public ICharacterVitalsSystem Vitals { get; } = new CharacterVitalsSystem();
    public ICharacterReferences References { get; } = new CharacterReferences();

    // --- Stats or Config ---
    public CharacterStats Stats { get; set; } = new();

    public void Initialize(GameObject owner)
    {
        References.Initialize(owner); //first to ensure all references are set up

        Input.Initialize(owner);
        Sensor.Initialize(owner);
        Intent.Initialize(owner);
        Motor.Initialize(owner);
    }
}
