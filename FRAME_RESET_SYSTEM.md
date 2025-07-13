# Frame Reset System Implementation

## Overview

Implemented a comprehensive frame reset system that provides a "clean slate" each frame, eliminating sticky behavior and making state intent explicit.

## Key Changes

### 1. **Comprehensive Frame Reset Method**

```csharp
public void ResetFrameIntent()
{
    // Commands/Actions (reset to neutral)
    WantsToJump = false;
    WantsToSprint = false;
    WantsToCrouch = false;

    // Physics overrides (reset to default motor behavior)
    OverrideGravity = false;
    OverrideYVelocity = false;
    OverrideHorizontalPhysics = false;

    // Movement intent (reset to no movement)
    DesiredVelocity = Vector3.zero;
}
```

### 2. **Controller Integration**

- Called in `Update()` before any state processing
- Ensures every frame starts with neutral intent
- States must explicitly declare their needs each frame

### 3. **State Behavior**

- **States now MUST set their intent every frame**
- No more "set once and forget" behavior
- Makes state logic more explicit and predictable

## Benefits

### **Eliminates Sticky Behavior**

- No more flags that get "stuck" on
- No more physics overrides that persist unintentionally
- Clean separation between frames

### **Explicit State Intent**

- States clearly declare what they want each frame
- Easy to debug - just look at the state's FixedUpdate/Update
- No hidden dependencies between frames

### **Future-Proof Architecture**

- New intent properties automatically get reset
- Magic states, complex movement, etc. will work seamlessly
- Consistent behavior pattern for all states

### **Maintainable**

- One place to add new intent properties
- Clear contract: "states set intent, motor applies it"
- Self-documenting system behavior

## State Examples

### **JumpState (Active Control)**

```csharp
public override void FixedUpdate(CharacterContext context)
{
    // Declare physics control each frame
    context.Intent.OverrideGravity = true;
    context.Intent.OverrideYVelocity = true;

    // Set exact velocity
    context.Intent.DesiredVelocity = calculatedVelocity;
}
```

### **IdleState (Default Behavior)**

```csharp
public override void FixedUpdate(CharacterContext context)
{
    // No overrides needed - uses default motor behavior
    ApplyMovement(context, Vector3.zero, 0f);
}
```

### **FallingState (Gravity-Driven)**

```csharp
public override void FixedUpdate(CharacterContext context)
{
    // No physics overrides - let motor handle gravity
    // Just set horizontal intent
    context.Intent.DesiredVelocity = new Vector3(horizontalMove.x, currentVelocity.y, horizontalMove.z);
}
```

## Frame Flow

```
Update():
1. Reset all intent to neutral
2. Input updates intent (button presses, etc.)
3. Sensor updates persistent state (grounded, etc.)
4. State machine updates intent (movement, overrides)

FixedUpdate():
5. State machine refines intent (if needed)
6. Motor applies intent to physics
```

## Design Principles

### **Intent vs State Separation**

- **Intent**: What the character wants to do (reset each frame)
- **State**: What the character's current condition is (persistent)

### **Explicit Over Implicit**

- States must explicitly declare their needs
- No hidden carryover between frames
- Easy to trace what's causing specific behavior

### **Motor as Pure Applier**

- Motor doesn't make decisions, just applies intent
- All decision logic lives in states
- Clean separation of concerns

## Future Extensions

This system naturally supports:

- **Magic Effects**: Levitation, telekinesis, etc.
- **Complex Movement**: Wall running, swimming, flying
- **Combat Integration**: Fighting while moving
- **Environmental Effects**: Wind, conveyor belts, etc.

All future systems just need to set appropriate intent flags - no motor modifications required.
