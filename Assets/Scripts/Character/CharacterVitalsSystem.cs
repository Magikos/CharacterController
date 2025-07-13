using System;

public class CharacterVitalsSystem : ICharacterVitalsSystem
{
    // This class can be used to manage character vitals like health, stamina, etc.
    // Currently, it is empty but can be expanded as needed.

    // Example properties for vitals
    public float Health { get; set; } = 100f;
    public float Stamina { get; set; } = 100f;
    public float Mana { get; set; } = 100f;
    public float Hunger { get; set; } = 100f;
    public float Thirst { get; set; } = 100f;
    public float Fatigue { get; set; } = 0f; // Fatigue can increase over time without rest
    public float MaxHealth { get; set; } = 100f;
    public float MaxStamina { get; set; } = 100f;
    public float MaxMana { get; set; } = 100f;

    public bool CanSprint => Stamina > 0;
    public bool CanJump => Stamina > 0;

    public void DrainStamina(float amount)
    {
        var staminaBefore = Stamina;
        Stamina -= amount;
        if (Stamina < 0) Stamina = 0;

        EventBus<StaminaChangedEvent>.Raise(new StaminaChangedEvent(staminaBefore, Stamina));
    }
}