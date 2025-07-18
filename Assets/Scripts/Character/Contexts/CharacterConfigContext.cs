using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class CharacterConfigContextAttribute : Attribute
{
    public string Name { get; }
    public CharacterConfigContextAttribute(string name)
    {
        Name = name;
    }
}

public class CharacterConfigContext
{
    public float StepHeightRatio { get; set; } = 0.1f; // Ratio of collider height to step height
    public float ColliderHeight { get; set; } = 2f;
    public float ColliderThickness { get; set; } = 1f;
    public Vector3 ColliderOffset { get; set; } = Vector3.zero;
    public bool DebugMode { get; set; } = false;
    public float GravityScale { get; set; } = 1f; // Scale for gravity effect
    public float TerminalVelocity { get; set; } = 15f; // Maximum falling speed
    public float MaxGroundCheckDistance { get; set; } = 5f; // Maximum distance for ground checks
    public float GroundedTolerance { get; set; } = 0.1f;
    public float MovementSpeed { get; set; } = 5f; // Base movement speed
    public float JumpForce { get; set; } = 10f; // Force applied
    public float JumpDuration { get; set; } = 0.2f; // Duration of the jump
    public float AirControlRate { get; set; } = 2f; // Rate of air control
    public float AirFriction { get; set; } = 0.5f; // Air friction applied during jumps
    public float GroundFriction { get; set; } = 100f;
    public float SlopeGravityScale { get; set; } = -9.81f; // Gravity applied on slopes
    public float MaxSlopeAngle { get; set; } = 45f; // Maximum angle for slope detection
    public float SlopeRayLength { get; set; } = 1f; // Length of the ray used for slope detection
    public bool UseLocalMomentum { get; set; } = true; // Use local momentum for movement calculations

    public void Initialize(GameObject owner)
    {
        var configType = GetType();
        var ownerType = owner.GetType();

        foreach (var ownerMember in ownerType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
        {
            var attr = ownerMember.GetCustomAttribute<CharacterConfigContextAttribute>();
            if (attr == null) continue;

            var configMember = configType.GetMember(attr.Name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault();
            if (configMember == null) continue;

            object value = null;
            if (ownerMember is PropertyInfo ownerProp && ownerProp.CanRead)
                value = ownerProp.GetValue(owner);
            else if (ownerMember is FieldInfo ownerField)
                value = ownerField.GetValue(owner);

            if (value != null)
            {
                if (configMember is PropertyInfo configProp && configProp.CanWrite)
                    configProp.SetValue(this, value);
                else if (configMember is FieldInfo configField)
                    configField.SetValue(this, value);
            }
        }
    }

    public void ResetFrameContext()
    {
        // Reset any frame-specific context if needed
    }

    public void Dispose()
    {
        // Cleanup logic if needed
    }
}