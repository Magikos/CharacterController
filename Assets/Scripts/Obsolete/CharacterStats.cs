// using UnityEngine;

// public class CharacterStats : ICharacterStats
// {
//     public float Stamina { get; set; } = 100f;
//     public float MaxStamina { get; } = 100f;
//     public float RegenRate { get; } = 10f;

//     public void DrainStamina(float amount) => Stamina = Mathf.Max(0, Stamina - amount);

//     public void RegenStamina(float deltaTime, bool canRegen)
//     {
//         if (canRegen)
//             Stamina = Mathf.Min(MaxStamina, Stamina + RegenRate * deltaTime);
//     }

//     public bool CanSprint => Stamina > 0;
// }
