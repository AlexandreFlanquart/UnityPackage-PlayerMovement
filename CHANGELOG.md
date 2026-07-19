## [1.0.4] - 2026-07-18
### Added
- Generic runtime control contract `IMovementControl` (`SetMovementEnabled`, `Stop`, `MovementEnabled`, `IsMoving`), implemented by `ClickToMoveController2D`, `PlayerController2D` and `IAController2D` — so game code can freeze/restore any mover (player or NPC).
- `IPlayerController2D : IMovementControl` for the click-to-move player, adding NavMesh commands `MoveTo`, `Warp`, resumable `Pause`/`Resume`, and the `OnDestinationReached` event (also exposed as a serialized `UnityEvent`).
- `ClickToMoveController2D.SetCamera(Camera)` and lazy re-resolution of the main camera; `Stop`/`Toggle Movement` context-menu entries and editor gizmos for the current path.
- `DirectionalAnimator2D` base class + `DirectionMode` (Free/Snap4/Snap8) + public `DirectionQuantizer`; cached animator parameter hashes; `FacingDirection` accessor. Ready for 4- and 8-direction blend trees.
- `VisionCone2D.OnTargetSpotted`/`OnTargetLost` edge-triggered events (detection was previously computed but unconsumed).
- Touchscreen tap binding for the click-to-move `Click` action; the click-to-move player prefab is now animated (Animator + `IAAnimator2D`).
- Edit-mode tests: control gate/idempotence, direction quantization (all sectors + boundaries), and patrol path indexing.
- `com.unity.ai.navigation` and `com.unity.inputsystem` dependencies.
- `IAController2D` anti-stuck watchdog: an agent kept motionless by another agent's avoidance (RVO deadlock) now treats itself as arrived (near its destination) or repicks a destination (mid-route) after `stuckTimeout` seconds; per-agent `avoidancePriority` is randomized on `Awake` (opt-out) so two converging agents no longer tie forever. `HasReachedDestination` now also waits on `pathPending`.
- `Sbire4 - VisionCone` prefab: an `IAController2D` patrol with a `VisionCone2D` child pre-wired to the agent, ready to drop in a scene.
- `MovementModeSwitcher2D` and the `Player - Hybrid` prefab: runtime switch between click-to-move and direct (WASD/ZQSD) control on a single GameObject, with a NavMesh-snap/refusal safeguard and a Rigidbody2D body-type swap so the handover never leaves the player off-mesh or falling. Edit-mode tests for the pure stuck/tolerance decision logic (`IAControllerStuckTests`).
### Why
- Consumers had no way to pilot the movers at runtime and had to reach into the `NavMeshAgent`/`Rigidbody2D` over the package (e.g. to freeze movement during a dialogue). Controllers exposed no public control member; `enabled = false` did not stop the current path. This contract lets game code cut input **and** stop the in-progress trajectory, then restore piloting — with no game-specific dependency.
### Fixed
- Legacy `Input.mousePosition` replaced by `Pointer.current` — click-to-move now works in "Input System only" projects and supports touch/pen.
- Click outside any ground collider no longer sends the click-to-move agent toward the world origin.
- `IAController2D` crash (`IndexOutOfRangeException`) with a single-waypoint path.
- `IAController2D` errors when spawned off the NavMesh (the first destination is now deferred until the agent is valid); agents are no longer left permanently stopped after `Stop()`.
- `PlayerAnimator2D` false error logs when references resolved via the `GetComponent` fallback.
- A movement key held across `PlayerController2D.SetMovementEnabled(false→true)` is no longer ignored.
- Per-iteration `WaitForSeconds` allocation in `VisionCone2D`.
- 2D NavMesh characters dropping by `baseOffset × scale` when entering play mode: `baseOffset` is applied along the surface normal (Z) at runtime, not +Y like the editor gizmo. NavMesh prefabs now keep `baseOffset = 0` with the root at the feet and visuals on an offset `Visual` child.
- `PlayerAnimator2D` no longer leaves a stale sprint/crouch pose when disabled (e.g. by `MovementModeSwitcher2D`): its `IsSprinting`/`IsCrouching` bools are now reset in `OnDisable`.
### Changed
- `ClickToMoveController2D` is no longer `sealed` (extensibility); its click raycast distance is now configurable. Input is ignored while movement is disabled. `enabled` behaviour is unchanged (backward compatible).
- 2D animators (`PlayerAnimator2D`, `IAAnimator2D`) now derive from `DirectionalAnimator2D` (same components/GUIDs — existing prefabs keep their values).
- Declared the `com.myunitypackage.toolkit` dependency in `package.json` (used by the runtime for logging).
- Sbire prefabs renamed (`Sbire1 - Path`, `Sbire2 - RandomRadius`, `Sbire3 - RandomZone`, GUIDs preserved); Sbire3 defaults tuned for zone patrol (wider radius, more sampling attempts).
- NavMesh prefabs restructured: root = NavMesh contact point (feet), visuals moved to a `Visual` child offset by the sprite half height; `IAAnimator2D` now resolves its agent via `GetComponentInParent`. The click-to-move camera child is disabled by default (opt-in for camera-less scenes).
- Aligned with official Unity 2D guidance: blend trees switched from Freeform Directional (which requires a center motion we don't have) to **2D Simple Directional**; `Rigidbody2D` interpolation enabled on `Player2D` (smooth rendering between physics steps); SpriteRenderer **Sort Point = Pivot** unified across character prefabs; README documents top-down Y-sorting setup (Transparency Sort Axis) and bottom-pivot sprites.
- Sample prefabs (`Sbire1/2/3`, `Player - ClickToMove`): `NavMeshAgent.radius` tightened to match the visual scale, `stoppingDistance` raised off 0, and a small post-arrival wait added, so agents no longer perpetually swarm the same spot; a kinematic `Rigidbody2D` added to every NavMesh-driven prefab (was a static collider that moved — a 2D physics anti-pattern). `Player2D` and `Player - ClickToMove` main cameras switched to orthographic. `Controllre2DIAScene` sample: `Player - Hybrid` is now the sole click listener in the active map (was fighting a duplicate `Player - ClickToMove` for the same clicks), the six leftover non-prefab `VisionCone2D` children were removed (superseded by `Sbire4 - VisionCone`), and one `Sbire1 - Path` instance now demonstrates `pathMode = Loop` (every other instance/prefab was `PingPong`).
### Removed
- Dead, unused `IPlayerClickMovement` interface (its `OnClickMove` event was never raised).

## [1.0.3] - 2026-01-09
### Added
- IA controller 2D
- NavMesh system with sprites & tilemaps
- Click&Move 2D

## [1.0.2] - 2025-12-15
### Added
- Player controller 2D

## [1.0.1] - 2025-10-07
### Added
- Vehicle Controller

## [1.0.0] - 2025-09-17
### Added
- 1st Person Controller
- 3rd Person Controller
- Switch between Controller



[1.0.0]: https://github.com/AlexandreFlanquart/UnityPackage-PlayerMovement/releases/tag/v1.0.0
[1.0.1]: https://github.com/AlexandreFlanquart/UnityPackage-PlayerMovement/releases/tag/v1.0.1