Data/UI split for Veterinar VR:

- `Scripts/Data/` holds code-only content contracts and ScriptableObject definitions.
- `ScriptableObjects/Cows/` and `ScriptableObjects/Questions/` are reserved for authored assets built from those definitions.
- `Scripts/UI/` holds scene-facing controllers that read session state and data assets, then drive world-space or screen-space presentation.
