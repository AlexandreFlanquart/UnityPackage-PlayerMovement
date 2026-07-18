using System.Runtime.CompilerServices;

// Exposes internal members (e.g. IAController2D.NextIndex) to the edit-mode test assembly.
[assembly: InternalsVisibleTo("MyUnityPackage.Controller.EditorTests")]
