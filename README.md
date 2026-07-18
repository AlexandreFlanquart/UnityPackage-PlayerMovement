# UnityPackage-PlayerMovement

## About
UnityPackage-PlayerMovement is a Unity package that provides ready-to-use controller prefabs:
- First-person controller prefab
- Third-person controller prefab
- Switch prefab (first-person ↔ third-person)
- Vehicle controller prefab
- 2D controller prefab

The package is designed to be modular: input is split into action maps and handled by dedicated scripts so you can swap/extend behaviors easily.

## What's New
To see the last update of the package check [here](CHANGELOG.md) !

## Controlling movers at runtime
All controllers expose a public control contract so game code can freeze/restore them —
e.g. block movement during a dialogue — without knowing the concrete controller or
reaching into its `Rigidbody2D` / `NavMeshAgent`.

- **`IMovementControl`** (base, on `ClickToMoveController2D`, `PlayerController2D`,
  `IAController2D`): `SetMovementEnabled(bool)`, `Stop()`, `MovementEnabled`, `IsMoving`.
- **`IPlayerController2D : IMovementControl`** (on `ClickToMoveController2D`) adds the
  NavMesh commands `MoveTo`, `Warp`, `Pause`/`Resume`, and the `OnDestinationReached` event.

```csharp
using MyUnityPackage.Controller;

// Generic freeze/restore — works for the click-to-move player, the Rigidbody player,
// and the NavMesh AI agents (NPCs) alike:
var mover = someGameObject.GetComponent<IMovementControl>();
mover.SetMovementEnabled(false); // cuts input/AI AND stops immediately (no glide)
mover.SetMovementEnabled(true);  // restores piloting
bool moving = mover.IsMoving;

// Full click-to-move control:
var player = playerGameObject.GetComponent<IPlayerController2D>();
player.MoveTo(targetWorldPosition); // snap to NavMesh + go (ignores MovementEnabled)
player.Warp(spawnWorldPosition);    // teleport onto the NavMesh
player.Stop();                      // stop and clear the path now
player.Pause();                     // stop but KEEP the destination...
player.Resume();                    // ...then continue toward it
player.OnDestinationReached += () => { /* arrived */ };
```

`SetMovementEnabled(false)` makes input inert and freezes the mover in place (no glide to
the last clicked point); `SetMovementEnabled(true)` restores it. All calls are idempotent.
`Stop()` clears the destination, whereas `Pause()`/`Resume()` keep it for resumption.
Disabling the component with `enabled = false` keeps its previous behaviour (unsubscribes
from input). If the main camera is created/replaced at runtime, call
`ClickToMoveController2D.SetCamera(cam)` so clicks keep resolving.

## 2D movement samples
One prefab per 2D movement type (under `Samples/Prefabs/`):

| Prefab | Movement | Notes |
|---|---|---|
| `Player2D` | WASD/ZQSD (`PlayerController2D`, Rigidbody2D) | Sprint/crouch Hold or Toggle |
| `Player - ClickToMove` | Click / tap to move (`ClickToMoveController2D`, NavMeshAgent) | Mouse, touch and pen; now animated |
| `Sbire1 - Path` | AI patrol along waypoints (`IAController2D`) | `Loop` or `PingPong` |
| `Sbire2 - RandomRadius` | AI random points around itself | No zone constraint |
| `Sbire3 - RandomZone` | AI random points inside a zone | **Assign `patrolZone` on the scene instance** (a prefab asset cannot reference a scene collider) |

Helper prefabs (drop into a scene):
- **`PlayerInputManager`** — required **once per scene**; the singleton that owns `PlayerControls`.
  Without it, the input scripts log an error and receive no input.
- **`NavMeshSurface2D`** — a preconfigured NavMeshPlus surface (rotated to the XY plane, agent
  type aligned with the prefabs). Drop it in, add your ground/wall colliders, then press **Bake**
  (the baked data is scene-scoped, so bake per scene).

### Prefab structure — the root is the feet
NavMesh prefabs (`Player - ClickToMove`, the three sbires) put the **NavMeshAgent on the root**
(= the NavMesh contact point, at the **feet**) and all visuals (`SpriteRenderer`, `Animator`,
`IAAnimator2D`) on a **`Visual` child** offset upward by the sprite's half height. When you swap
in your own art on an instance, set the instance's `Visual` local Y to *your* sprite's half
height so its bottom sits back on the root.

> 💡 **Best practice for your own art**: set the sprite **Pivot to Bottom** in the import
> settings — then `Visual` local Y is simply **0** and the sprite naturally stands on the root.
> The half-height offset is only needed for center-pivot sprites (like the placeholder capsule).

### Y-sorting (top-down depth)
For characters to pass in front of / behind obstacles based on their Y position, configure the
project once (this is a project setting, not something the package can ship):
- **Edit > Project Settings > Graphics** (or your URP 2D Renderer asset): set **Transparency
  Sort Mode = Custom Axis**, with axis **(0, 1, 0)**.
- The package prefabs already use **SpriteRenderer Sort Point = Pivot**, which combined with
  bottom pivots (or the `Visual` offset) sorts characters by their feet, as recommended by the
  official Unity docs.

> ⚠ **Never use `NavMeshAgent.baseOffset` to push the contact point to the feet in 2D.** The
> editor gizmo applies it along +Y, but at runtime it is applied along the **NavMesh surface
> normal (Z with NavMeshPlus XY surfaces)** — everything visually drops by `baseOffset × scale`
> the moment you press Play. Keep `baseOffset = 0` and offset the `Visual` child instead.

### Building a scene — manual wiring
Component-complete prefabs still need scene-level setup:
1. Add the **`PlayerInputManager`** prefab (once).
2. **Player2D** brings its own camera. **`Player - ClickToMove`** also ships one but **disabled by
   default** (to avoid fighting an existing scene camera / duplicate AudioListener) — enable its
   `Main Camera` child only in scenes that have no camera; otherwise keep your scene camera
   tagged **MainCamera** (the controller reads `Camera.main`).
3. For click-to-move / sbires: add the **`NavMeshSurface2D`** prefab and **Bake**. Put the walkable
   ground under 2D colliders.
4. **Ground layer**: `Player - ClickToMove`'s `groundMask` targets a layer named **"Ground"** (layer 9
   in this project). In a fresh project this layer won't exist — create a layer, assign it to your
   ground, and set the prefab's `groundMask` to it, otherwise clicks detect nothing.
5. **Sbire1 - Path**: place waypoint Transforms and assign them to its `points` array.
   **Sbire3 - RandomZone**: assign a `patrolZone` collider on the instance.

### Animation (4 / 8 directions)
2D animators derive from **`DirectionalAnimator2D`**, which writes a facing direction to the
`MoveX`/`MoveY` animator parameters and the raw speed to `Speed`:
- `PlayerAnimator2D` reads the `Rigidbody2D` velocity (and drives `IsSprinting`/`IsCrouching`).
- `IAAnimator2D` reads the `NavMeshAgent` desired velocity (used by the sbires and the click-to-move player).

The `DirectionMode` field controls snapping: `Free` (no snapping), `Snap4` (cardinals) or
`Snap8` (cardinals + diagonals). The sample blend trees use **2D Simple Directional** (the type
Unity's docs prescribe for one motion per direction). For an **8-direction** setup, add diagonal
clips as blend-tree children at `(±0.71, ±0.71)` and set `DirectionMode = Snap8` — the quantizer
already emits those exact unit diagonals. `Speed` stays continuous in every mode.

Notes: the `IsCrouching` animator parameter is declared but not used by any transition yet
(reserved — wire it up if you add crouch clips). The sample states run at `Speed 0.1` to slow
the 3-frame placeholder clips; with your own clips, prefer authoring the correct sample rate
and resetting the state speed to 1.

### Updating the dependency in a consumer project
This package is consumed by git URL, so after pushing changes here:
1. Push the new commit/tag to the package repo (bump the semver in `package.json`).
2. In the consumer project, force Unity to re-resolve: either **Packages > In Project >
   this package > Update**, or edit `Packages/packages-lock.json` and bump the pinned
   `hash` (commit SHA) for `com.myunitypackage.controller`, then let Unity re-resolve.
   Pinning a tag (`...git#v1.0.4`) in `manifest.json` also works.


## Input system
The samples use Unity's **Input System** and include an input asset:
- `Samples/Input/PlayerControls.inputactions`

Click-to-move works with mouse, pen and **touch** (the `Click` action is bound to
`<Mouse>/leftButton` and `<Touchscreen>/primaryTouch/tap`); the pointer position is read from
`Pointer.current`, so it also works in projects set to "Input System Package (New)" only.

> Note: the sample `PlayerInputManager` (namespace `MyUnityPackage.ControllerSample`) shares its
> name with `UnityEngine.InputSystem.PlayerInputManager`. If a script imports both namespaces,
> qualify the type or use an alias (`using PIM = MyUnityPackage.ControllerSample.PlayerInputManager;`).

If you want to customize bindings, duplicate the `.inputactions` file and update the scripts that read from `PlayerControls`.

## ⚙️ Requirements
Resolved automatically from the Unity registry (declared in `dependencies`):
- **Cinemachine** `3.1.4`
- **AI Navigation** (`com.unity.ai.navigation`) `2.0.12` — NavMesh baking for the 2D/3D AI samples.
- **Input System** (`com.unity.inputsystem`) `1.19.0` — used by the sample input scripts.

Must be installed manually (distributed by **git URL**, so UPM cannot resolve them automatically):
- **UnityPackage-Toolkit** (`com.myunitypackage.toolkit`) — the runtime uses `MUPLogger`.
  **Add package from git URL...**: `https://github.com/AlexandreFlanquart/UnityPackage-Toolkit.git`
- **NavMeshPlus** (`com.h8man.2d.navmeshplus`) — only needed to **re-bake** the 2D sample
  NavMeshes (the baked data ships with the samples, so it is optional for just running them):
  `https://github.com/h8man/NavMeshPlus.git`

## 📦 How to install in Unity
This guide explains how to install this Unity package using the **Unity Package Manager**.

### 🔹 1. Open the Package Manager
1. In Unity, go to the **top menu**.
2. Click **Window > Package Manager**.
3. The **Package Manager** window will open, showing the list of installed packages.

### 🔹 2. Add the Git Package
1. In the **Package Manager**, click the **➕** button (top left corner).
2. Select **"Add package from git URL..."**.
3. Enter the following Git repository URL: <br>
   https://github.com/AlexandreFlanquart/UnityPackage-PlayerMovement.git
4. Click **"Add"**, and Unity will download and install the package.

### 🔹 3. Install a Specific Version (Optional)
If you want to install a specific release, **append the tag** at the end of the URL: <br>
https://github.com/AlexandreFlanquart/UnityPackage-PlayerMovement.git#v1.0.0

This ensures you get the exact version you need.

### 🔹 4. That's it!
Your package is now installed and ready to use in your Unity project.

<br>

## 🛠️ Troubleshooting
- If inputs do not work, ensure the **Input System** package is installed and enabled in your project.
- If you modified the `.inputactions`, make sure the generated C# class (if any) and references are up to date.
- If there is an issue, report it to the dev team (include Unity version, package version/tag, and a repro).
