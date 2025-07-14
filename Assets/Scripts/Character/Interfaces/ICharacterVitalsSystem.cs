using System;

public interface ICharacterVitalsSystem : IDisposable
{
    float Health { get; set; }
    float Stamina { get; set; }
    float Mana { get; set; }
    float Hunger { get; set; }
    float Thirst { get; set; }
    float Fatigue { get; set; }
    float MaxHealth { get; set; }
    float MaxStamina { get; set; }
    float MaxMana { get; set; }

    bool CanSprint { get; }
    bool CanJump { get; }

    void DrainStamina(float v);
}
