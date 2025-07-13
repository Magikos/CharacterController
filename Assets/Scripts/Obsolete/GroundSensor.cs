// using System.Collections.Generic;
// using UnityEngine;

// #if UNITY_EDITOR
// using MiniTools.BetterGizmos;
// #endif

// [RequireComponent(typeof(CapsuleCollider))]
// public class GroundSensor : MonoBehaviour, ISensor<CharacterContext>
// {
//     [Header("Settings")]
//     [SerializeField] private float checkDistance = 0.3f;
//     [SerializeField] private LayerMask excludeLayers = 0; // Layers to ignore (e.g., water, non-solid surfaces)

//     private CapsuleCollider _collider;
//     private ICharacterContext _context;
//     private RaycastHit[] _hits = new RaycastHit[4]; // Avoid allocations

//     // Cached colliders for performance optimization and self-exclusion
//     private HashSet<Collider> _selfColliders;
//     private bool _selfCollidersInitialized = false;

//     // ISensor implementation
//     public SensorUpdateMode DefaultMode => SensorUpdateMode.Reduced;

//     public void Initialize(CharacterContext context)
//     {
//         _collider = GetComponent<CapsuleCollider>();
//         _context = context;

//         // Debug collider info
//         Logwin.Log("GroundSensor", $"Collider - Center: {_collider.center}, Height: {_collider.height}, Radius: {_collider.radius}");
//         Logwin.Log("GroundSensor", $"Settings - CheckDistance: {checkDistance}, ExcludeLayers: {excludeLayers.value}");

//         // Cache all colliders on this character for fast self-collision checks
//         InitializeSelfColliders();
//     }

//     private void InitializeSelfColliders()
//     {
//         if (_selfCollidersInitialized) return;

//         _selfColliders = new HashSet<Collider>();

//         // Get all colliders on this GameObject and its children
//         Collider[] allColliders = GetComponentsInChildren<Collider>();
//         foreach (var col in allColliders)
//         {
//             _selfColliders.Add(col);
//         }

//         _selfCollidersInitialized = true;
//         Logwin.Log("GroundSensor", $"Cached {_selfColliders.Count} self-colliders for fast detection");
//     }

//     public void UpdateSensor(CharacterContext context)
//     {
//         Vector3 center = transform.position + _collider.center;
//         float radius = _collider.radius * 0.95f; // Slightly smaller to avoid edge cases

//         // Calculate cast parameters differently to ensure we reach the ground
//         float halfHeight = _collider.height / 2f;

//         // Start the cast from the bottom of the collider
//         Vector3 castStart = center - Vector3.up * (halfHeight - radius);

//         // Cast downward from collider bottom with additional check distance
//         float castDistance = checkDistance;

//         // Debug logging for ground detection
//         Logwin.Log("GroundSensor", $"Cast params - Start: {castStart}, Radius: {radius}, Distance: {castDistance}");
//         Logwin.Log("GroundSensor", $"Transform position: {transform.position}, Collider center: {_collider.center}, Collider height: {_collider.height}, Collider radius: {_collider.radius}");

//         // Calculate layer mask: everything except excluded layers
//         int layerMask = ~excludeLayers.value;

//         // Use SphereCastNonAlloc for more reliable ground detection with better performance
//         int hitCount = Physics.SphereCastNonAlloc(
//             castStart,
//             radius,
//             Vector3.down,
//             _hits,
//             castDistance,
//             layerMask,
//             QueryTriggerInteraction.Ignore
//         );

//         // Find the closest valid hit (excluding self-colliders)
//         bool grounded = false;
//         RaycastHit hit = new RaycastHit();
//         float closestDistance = float.MaxValue;

//         for (int i = 0; i < hitCount; i++)
//         {
//             // Skip if it's one of our own colliders
//             if (_selfColliders.Contains(_hits[i].collider))
//             {
//                 Logwin.Log("GroundSensor", $"REJECTED SELF - Hit own collider: {_hits[i].collider.name}");
//                 continue;
//             }

//             // Find the closest valid hit
//             if (_hits[i].distance < closestDistance)
//             {
//                 closestDistance = _hits[i].distance;
//                 hit = _hits[i];
//                 grounded = true;
//             }
//         }

//         // Debug what we hit
//         if (grounded)
//         {
//             Logwin.Log("GroundSensor", $"HIT DETECTED - Object: {hit.collider.name}, Distance: {hit.distance:F3}, Point: {hit.point}, Normal: {hit.normal}");
//             Logwin.Log("GroundSensor", $"Transform Y: {transform.position.y:F3}, Contact Y: {hit.point.y:F3}, Desired Y: {context.Sensor.DesiredGroundPosition.y:F3}");
//         }
//         else
//         {
//             Logwin.Log("GroundSensor", $"NO HIT - Found {hitCount} total hits, Exclude mask: {excludeLayers.value}, Layer mask used: {layerMask}, Cast distance: {castDistance}");
//         }

//         // Additional check: make sure we're not detecting any of our own colliders
//         if (grounded && hit.collider != null && _selfColliders.Contains(hit.collider))
//         {
//             Logwin.Log("GroundSensor", $"REJECTED - Hit own collider: {hit.collider.name}");
//             grounded = false;
//         }

//         // Update grounded state with some smoothing to prevent jitter
//         bool wasGrounded = context.Sensor.IsGrounded;
//         context.Sensor.IsGrounded = grounded;

//         if (grounded)
//         {
//             context.Sensor.TimeSinceLastGrounded = 0f;
//             context.Sensor.LastGroundedTime = Time.time;
//             context.Sensor.LastGroundedHeight = transform.position.y;

//             // Store surface information for slope handling
//             context.Sensor.GroundNormal = hit.normal;
//             context.Sensor.GroundContactPoint = hit.point;

//             // Simple and correct: transform position should match ground hit point
//             context.Sensor.DesiredGroundPosition = new Vector3(
//                 transform.position.x,
//                 hit.point.y,
//                 transform.position.z
//             );

//             // Debug the positioning calculation
//             Logwin.Log("GroundSensor", $"Position calc - Hit.point.y: {hit.point.y:F3}, DesiredPos.y: {hit.point.y:F3}, Current.y: {transform.position.y:F3}");
//         }
//         else
//         {
//             context.Sensor.TimeSinceLastGrounded += context.DeltaTime;
//             // Keep last known surface info when airborne for brief periods
//         }

//         // Log ground state changes
//         if (wasGrounded != grounded)
//         {
//             Logwin.Log("GroundSensor", $"Ground state changed: {wasGrounded} -> {grounded}");
//         }

// #if UNITY_EDITOR
//         Color color = grounded ? Color.green : Color.red;
//         Debug.DrawLine(castStart, castStart + Vector3.down * castDistance, color);
//         if (grounded && hit.point != Vector3.zero)
//         {
//             Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.5f, Color.yellow);
//         }
// #endif
//     }

// #if UNITY_EDITOR
//     void OnDrawGizmos()
//     {
//         if (!Application.isPlaying) return;

//         if (_collider == null) return;

//         Vector3 center = transform.position + _collider.center;
//         float radius = _collider.radius * 0.95f;
//         float halfHeight = _collider.height / 2f;

//         // Use the same calculation as in UpdateSensor
//         Vector3 castStart = center - Vector3.up * (halfHeight - radius);
//         Vector3 castEnd = castStart + Vector3.down * checkDistance;

//         Color capsuleColor = _context?.Sensor.IsGrounded == true ? Color.green : Color.red;

//         // Draw the sphere cast as a beautiful capsule using BetterGizmos
//         BetterGizmos.DrawSphere(capsuleColor, castStart, radius);

//         // Draw the cast extension
//         Color castColor = capsuleColor;
//         castColor.a = 0.5f; // Make it semi-transparent
//         BetterGizmos.DrawSphere(castColor, castEnd, radius);

//         // Draw center line showing cast direction
//         Gizmos.color = Color.yellow;
//         Gizmos.DrawLine(castStart, castEnd);

//         // Always draw cast parameters as text-like indicators
//         Vector3 textPos = transform.position + Vector3.right * 2f;
//         Gizmos.color = Color.white;
//         Gizmos.DrawWireCube(textPos, Vector3.one * 0.1f);

//         // Draw ground contact point if grounded
//         if (_context?.Sensor.IsGrounded == true)
//         {
//             // Calculate the same layer mask for visualization
//             int layerMask = ~excludeLayers.value;

//             // Perform the same cast to get hit point for visualization
//             bool hit = Physics.SphereCast(
//                 castStart,
//                 radius,
//                 Vector3.down,
//                 out RaycastHit hitInfo,
//                 checkDistance,
//                 layerMask,
//                 QueryTriggerInteraction.Ignore
//             );

//             if (hit && hitInfo.point != Vector3.zero)
//             {
//                 // Draw hit point
//                 BetterGizmos.DrawSphere(Color.yellow, hitInfo.point, 0.1f);

//                 // Draw surface normal with slope-aware coloring
//                 Vector3 normalStart = hitInfo.point;
//                 Vector3 normalEnd = normalStart + hitInfo.normal * 0.5f;

//                 // Color based on slope angle
//                 float slopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
//                 Color normalColor = Color.cyan;
//                 if (slopeAngle > LocomotionSettings.MaxWalkableSlope)
//                     normalColor = Color.red; // Too steep
//                 else if (slopeAngle > LocomotionSettings.MaxRunningSlope)
//                     normalColor = Color.yellow; // Walkable but not runnable

//                 BetterGizmos.DrawArrow(normalColor, normalStart, normalEnd, Vector3.forward, 0.1f);

//                 // Show slope angle as text would be nice, but gizmos don't support text easily
//                 // Draw slope angle indicator with small spheres
//                 if (slopeAngle > 5f) // Only show for noticeable slopes
//                 {
//                     Vector3 slopeIndicator = normalStart + Vector3.up * 0.3f;
//                     float intensity = slopeAngle / 90f; // Normalize to 0-1
//                     Color slopeColor = Color.Lerp(Color.green, Color.red, intensity);
//                     BetterGizmos.DrawSphere(slopeColor, slopeIndicator, 0.05f);
//                 }
//             }
//         }
//         else
//         {
//             // Draw "NOT GROUNDED" indicator
//             Vector3 errorPos = transform.position + Vector3.up * 2f;
//             Gizmos.color = Color.red;
//             Gizmos.DrawWireCube(errorPos, Vector3.one * 0.3f);
//         }
//     }
// #endif
// }
