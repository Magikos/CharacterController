# Character Controller Movement Improvements

## Overview

Major improvements have been made to the character controller system to address movement fluidity, jump mechanics, and state transitions. The focus was on creating more responsive and natural-feeling movement.

## Key Improvements Made

### 1. **Gravity System Overhaul**

- **Previous**: Gravity only applied during falling
- **Now**: Gravity consistently applied except when actively jumping
- **Benefits**: More realistic physics, smoother transitions
- **Implementation**: Motor now handles gravity with configurable gravity scale

### 2. **Variable Jump Height System**

- **Previous**: Fixed jump force with basic hold-to-extend
- **Now**: Mario-style responsive jumping (tap = short hop, hold = higher jump)
- **Features**:
  - Minimum jump height for quick taps (1.0f)
  - Maximum jump height for sustained holds (2.5f)
  - 0.3-second input window for variable control
  - Smooth velocity interpolation based on hold time
- **Benefits**: More player control, natural feel

### 3. **Enhanced Air Control**

- **Jump State**: 30% air control while jumping upward
- **Fall State**: 20% air control while falling
- **Implementation**: Momentum preservation with limited steering
- **Benefits**: Realistic physics with subtle player agency

### 4. **Improved State Transitions**

- **Jump → Fall**: Natural transition when velocity becomes negative
- **Grounded Detection**: Uses grace period to prevent jitter
- **Landing States**: Automatic selection based on fall distance
  - 0-2.5f: Direct to movement state
  - 2.5f-5f: Soft landing
  - 5f+: Hard landing

### 5. **Enhanced Ground Detection**

- **Previous**: OverlapCapsule with potential jitter issues
- **Now**: SphereCast for more reliable detection
- **Features**:
  - Better edge case handling
  - Improved debug visualization
  - Reduced false positives
  - State change logging

### 6. **Configurable Physics Parameters**

Added new CharacterStats properties:

- `GroundDrag`: Friction when grounded (5f)
- `AirDrag`: Air resistance when airborne (0.5f)
- `GravityScale`: Gravity multiplier (1f)
- `AirAccelerationMultiplier`: Air control factor (0.3f)

### 7. **Enhanced Logging & Debugging**

- Detailed state transition logging
- Velocity and timing information
- Fall distance tracking
- Ground state change notifications
- Visual debug rays for ground detection

## Technical Implementation Details

### Motor Physics Flow

1. **Gravity Application**: Always applied unless actively jumping upward
2. **Horizontal Movement**: Different acceleration for ground vs air
3. **Jump Force**: Applied as direct velocity override during jump state
4. **Ground Clamping**: Prevents negative Y velocity when grounded
5. **Terminal Velocity**: Caps maximum falling speed (-15 m/s)
6. **Drag System**: Separate drag values for ground and air

### State Machine Flow

```
Grounded States:
  Idle ↔ Walk ↔ Run ↔ Sprint
  Any → Jump (on input + conditions)
  Landing States → Movement State

Airborne States:
  Jump → Fall (when velocity.y <= 0)
  Fall → Grounded (when touching ground)
```

### Jump Mechanics Detail

```
Input Window: 0.3 seconds
Min Height: 1.0f (tap)
Max Height: 2.5f (hold)
Velocity Curve: Smooth interpolation based on hold time
Air Control: 30% of ground movement
```

## Configuration Options

### In CharacterStats:

- `Acceleration`: Ground movement responsiveness (20f)
- `AirAccelerationMultiplier`: Air control factor (0.3f)
- `GravityScale`: Gravity strength multiplier (1f)
- `GroundDrag` / `AirDrag`: Movement damping

### In LocomotionSettings:

- `GroundedGracePeriod`: Prevents ground jitter (0.2f)
- Movement speed thresholds for state transitions

### In Motor:

- `gravity`: Base gravity value (-9.81f)
- `terminalVelocity`: Maximum fall speed (-15f)

## Expected Improvements

### Player Experience:

- More responsive and natural jump feel
- Better air control without being "floaty"
- Smoother transitions between movement states
- More predictable ground detection

### Technical Benefits:

- Reduced ground detection jitter
- More configurable physics parameters
- Better debugging and logging
- Cleaner state transition logic

### Future Extensibility:

- Foundation for advanced movement (wall jumping, double jumps)
- Easier integration with magic/orb effects
- Modular physics that can be data-driven
- Better support for different character types

## Next Steps

1. **Test and Tune**: Play test the new movement system
2. **Animation Integration**: Connect to blend trees and animation states
3. **Advanced Features**: Add step-up, slope handling, wall detection
4. **Magic Integration**: Prepare for orb-based movement modifications
5. **Performance**: Optimize for multiple characters

## Usage Notes

The system maintains backward compatibility while providing much more responsive movement. All previous state transitions still work, but with improved physics and timing. The new jump system should feel much more natural and responsive to player input.
