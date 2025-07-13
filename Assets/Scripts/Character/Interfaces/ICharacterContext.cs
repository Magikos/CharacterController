using UnityEngine;

/// <summary>
/// Interface that defines the full character context structure.
/// </summary>
public interface ICharacterContext
{
    float DeltaTime { get; set; }
    float FixedDeltaTime { get; set; }

    ICharacterReferences References { get; }
    ICharacterInputContext Input { get; }
    ICharacterSensorContext Sensor { get; }
    ICharacterIntentContext Intent { get; }
    ICharacterMotorContext Motor { get; }
    ICharacterVitalsSystem Vitals { get; }

    CharacterStats Stats { get; set; }

    void Initialize(GameObject owner);
}
