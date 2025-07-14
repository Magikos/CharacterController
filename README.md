# Magikos Character Controller

## Overview

This repository contains a custom, modular character controller built in Unity for the Magikos game—a magic-focused, open-world survival RPG. The controller is designed to handle dynamic movement in a world where magic influences physics and interactions. It uses a state-driven approach with layers for locomotion, combat, and spellcasting, allowing for flexible extensions like climbing, swimming, or flight in future updates.

Key goals:
- Modular and extensible architecture following DRY (Don't Repeat Yourself) and SRP (Single Responsibility Principle).
- Integration with Unity's new Input System for abstracted inputs.
- Support for adaptive gravity, variable jump heights, ceiling detection, and environmental effects tied to Magikos' magic systems (e.g., orbs and rituals).

**Note**: This is a work-in-progress project and currently in a broken state. Some features like jump logic, state transitions, or gravity curves may not function as expected. Contributions or bug fixes are welcome!

## Features

- **Modular State Machine**: Adaptive State Machine (ASM) for smooth transitions between states (e.g., idle, running, jumping, spellcasting).
- **Layered Movement**: Separate layers for base locomotion, combat modifiers, and magic influences.
- **Ground Checking & Jumping**: Robust ground detection, variable jump heights, ceiling avoidance, and customizable gravity curves.
- **Input Abstraction**: Wrapper around Unity's Input System for easy remapping and multiplayer support.
- **Scalability**: Designed to expand with Magikos features like mana-based movement boosts or orb-embedded speed alterations.
- **Debug Tools**: Integrated with the project's EventBus for runtime diagnosis (e.g., event logging overlay via F10 in dev builds).

Planned enhancements (from MOVEMENT_IMPROVEMENTS.md and FRAME_RESET_SYSTEM.md):
- Advanced jumping mechanics (e.g., double jumps evolving into flight via spell progression).
- Frame reset system for consistent physics updates.
- Integration with rituals for world-altering movement (e.g., low-gravity zones).

## Requirements

- Unity 2022.3 or later (tested on 2023.x for UI Toolkit compatibility).
- Unity's New Input System package (enable in Project Settings > Player > Active Input Handling).
- Optional: Asset Inventory, Quantum Console (QFSW.QC), or other plugins listed in Packages/manifest.json for extended features.

## Installation

1. Clone the repository:
   ```
   git clone https://github.com/Magikos/CharacterController.git
   ```

2. Open the project in Unity Hub (add the cloned folder as a new project).

3. Import any required packages via the Package Manager if not already included (e.g., Input System).

4. Attach the main `CharacterController` script to your player GameObject in a scene.

## Usage

1. **Setup in Scene**:
   - Create a player prefab or GameObject.
   - Add the `CharacterController` component (found in Assets/Scripts/StateMachine/ or similar).
   - Configure serialized fields like movement speed, jump height, gravity multiplier in the Inspector.

2. **Basic Movement**:
   - Use WASD/Arrow keys for locomotion (abstracted via Input System).
   - Space for jump; hold for variable height.
   - Integrate with Magikos magic: Override states for spellcasting (e.g., via EventBus events).

3. **Debugging**:
   - Press F10 to toggle EventBus debug overlay (shows raised events and listeners).
   - Check console logs for state transitions and input data.

Example code snippet for extending a state (from AdaptiveStateMachine.cs):
```csharp
public class JumpState : State {
    public override void Enter() {
        // Apply jump force, check ceiling
    }
}
```

## Known Issues

- Jump logic may fail on uneven terrain (ground checking needs refinement).
- State transitions can glitch during rapid input (e.g., jump + spellcast).
- Gravity curves not fully adaptive; fixed values cause inconsistent falls.
- Multiplayer syncing not implemented yet.
- Some dependencies (e.g., plugins in Packages/) may cause build errors—remove unused ones.

Track progress in issues or the .md notes files.

## Contributing

Feel free to fork, submit pull requests, or open issues for bugs/fixes. Focus areas:
- Fixing jump and gravity bugs.
- Adding swimming/flight states.
- Integrating with Magikos orb system for dynamic modifiers.

## License

This project is licensed under the MIT License. See LICENSE file for details (add one if not present).

For more on the full Magikos project, check the overview in discussions or contact the maintainer.
