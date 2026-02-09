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


## Input system
The samples use Unity's **Input System** and include an input asset:
- `Samples~/Input/PlayerControls.inputactions`

If you want to customize bindings, duplicate the `.inputactions` file and update the scripts that read from `PlayerControls`.

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
