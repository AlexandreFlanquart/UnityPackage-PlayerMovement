# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

A Unity 6 (`6000.0`) UPM package — `com.myunitypackage.controller` (displayName "UnityPackage - Controller") — that ships ready-to-use character/vehicle controllers: 3D first/third-person, first↔third switch, vehicle, 2D player, 2D click-to-move, and 2D NavMesh AI agents. Distributed via git URL through Unity Package Manager (see README).

## Build / test / run

There is no standalone build system, linter, or test suite. Compilation and validation happen inside the Unity Editor. This repo is one package under a larger Unity project (`d:\Workspace_Unity\UnityPackages`).

- To verify a code change compiles or to inspect errors, drive the Editor through the **`unity-mcp`** tools rather than a shell build: `refresh_unity` (recompile), `read_console` (compile/runtime errors), `run_tests` (Test Runner). Always check the `mcpforunity://custom-tools` resource and `mcpforunity://instances` first; pin the instance with `set_active_instance` when several are connected.
- There are currently **no test assemblies** in this package.

## Assemblies

Two asmdefs, and the split matters:

- **`MyUnityPackage.Controller`** (`Runtime/`) — namespace `MyUnityPackage.Controller`. All reusable controller logic. References the Toolkit, Cinemachine, and Input System by GUID.
- **`MyUnityPackage.ControllerSample`** (`Samples/`) — namespace `MyUnityPackage.ControllerSample`. Example scenes, prefabs, the `PlayerControls.inputactions` asset, and — importantly — the **concrete input implementations**.

## Core architecture: input is decoupled from movement via interfaces

This is the single most important pattern to understand before editing.

Runtime controllers never read the Input System directly. They depend on **interfaces** in `Runtime/Scripts/Player/`, resolved with `GetComponent<TInterface>()` on the same GameObject:

- `IPlayerMovement` — the main one: `OnMoveEvent`, `OnLookEvent`, `OnJumpPressed/Released`, `OnSprintStarted/Canceled`, `OnCrouchStarted/Canceled`, `OnClickPressed` (all C# `event Action`).
- `IThirdPersoninput` — camera zoom/scroll properties for the third-person camera.
- `IVehicleInput` (`Runtime/Scripts/Vehicle/`) — `MovementInput`, `IsDrifting`, etc.
- `IPlayerClickMovement` — screen-space click for click-to-move.

The **concrete implementations live in `Samples/Scripts/Input/`** (e.g. `PlayerMovementInput`, `ThirdPersonInput`, `VehicleInput`), not in Runtime. Each one implements the runtime interface *and* the generated `PlayerControls.I*Actions` callback interface, translating Input System callbacks into the interface's `event Action`s. Consequence: **the Runtime package alone has no working input** — a consumer must import the Sample or supply their own `IPlayerMovement` implementation on the controller GameObject.

Input plumbing:
- `PlayerInputManager` (Sample, singleton, `[DefaultExecutionOrder(-3)]`) owns the generated `PlayerControls` asset. Input scripts read `PlayerInputManager.Instance.PlayerControls`, enable their action map, and `SetCallbacks(this)` in `OnEnable`.
- Controllers subscribe to the interface events in `Awake`/`OnEnable` and unsubscribe in `OnDisable`.

Execution order is deliberate: `PlayerInputManager` (-3) → input scripts → `PlayerController` (`[DefaultExecutionOrder(-1)]`).

## Component pieces

- **`PlayerController`** (3D) — `CharacterController`-based, custom gravity/drag/anti-bump, slope handling via `CharacterControllerUtils.GetNormalWithSphereCast`. Requires `PlayerState` + `PlayerAnimation`. Camera-relative movement.
- **`PlayerState`** — tiny state machine over `EPlayerState` (Idle/Walk/Run/Sprint/Jump/Fall/Climb) with an `OnStateChanged` event; `IsGrounded()` is derived from the state. Animation and movement both read from it.
- **`PlayerController2D`** — `Rigidbody2D`, `FixedUpdate` velocity. Sprint/crouch each support `InputToggleMode.Hold` or `Toggle`.
- **Vehicle** (`Runtime/Scripts/Vehicle/`) — `VehicleController` drives an array of `WheelController`s (`Rigidbody` + `WheelCollider`). Tuning data is in **ScriptableObjects** (`MotorSO`, `WheelSO`) applied via `ApplyMotorSO`/`ApplyWheelSo`. `HeadlightHandler`, `VehicleEffects`, `VehiculeAudio` handle presentation.
- **2D AI** (`Runtime/Scripts/Player/2D/`) — `IAController2D` (sealed) drives a `NavMeshAgent` configured for 2D (`updateRotation/updateUpAxis = false`), with `Random` or `Path` patrol (Loop/PingPong). Paired with `IAAnimator2D`, `VisionCone2D`. `ClickToMoveController2D` handles click-to-move.
- **`SwitchPOV`** — toggles first/third cameras and enables/disables the third-person input component.

## Conventions specific to this repo

- **Logging uses `MUPLogger`** (`using MyUnityPackage.Toolkit;`), not `Debug.Log`. It is a static class with `Info/Warning/Error/Log(level, msg, context, editorOnly)`. This is a **hard dependency on the sibling `UnityPackage-Toolkit` package** (`MyUnityPackage.Toolkit`) that is *not* declared in `package.json` (only `com.unity.cinemachine` is). Both asmdefs reference the Toolkit; keep new code using `MUPLogger`. (Note: some older 2D code still uses `Debug.LogError` — prefer `MUPLogger` in new code.)
- Follow the existing style: `[SerializeField] private` fields, `_camelCase` for private fields where already used, camera-relative movement, physics in `FixedUpdate`.
- Sample content currently sits in `Samples/` (a normal, always-imported folder). Note `package.json`'s `samples[].path` still points at `Samples~` (the UPM convention for import-on-demand hidden samples) — there is an in-progress migration between the two; check both when touching sample assets.

## Global instructions reminder

User-level instructions (see `~/.claude/CLAUDE.md`) apply: respond in the user's language, propose architecture before large changes, add a short "⚠ Points d'attention" after significant code blocks, only commit when explicitly asked, and follow the Unity naming/performance conventions there.
