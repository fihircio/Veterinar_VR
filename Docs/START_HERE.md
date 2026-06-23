# START HERE

Unity version: 6000.4.5f1

Repository: `Veterinar_VR/Veterinar_VR@cloud`

Current branch: `/main`

## First Workflow

1. Check in the current baseline in Unity Version Control before XR setup.
   - Include source assets, `Packages/manifest.json`, `Packages/packages-lock.json`, `ProjectSettings`, `Assets`, `Docs`, and `ignore.conf`.
   - Do not include generated folders or local machine files already covered by `ignore.conf`: `Library`, `Temp`, `Logs`, `UserSettings`, build outputs, IDE files, APK/AAB outputs, `*.csproj`, `*.sln`, or `*.slnx`.

2. Install XR packages from Unity Package Manager in the Unity Editor.
   - Install XR Plugin Management.
   - Install OpenXR Plugin.
   - Install XR Interaction Toolkit.
   - Let Unity update `Packages/manifest.json`, `Packages/packages-lock.json`, and any required `ProjectSettings` files.
   - Enable OpenXR for Android/Quest, then enable the Meta Quest/OpenXR interaction profile required by the project.

3. Create an XR smoke test before feature work.
   - Add a minimal scene or test area with an XR Origin, tracked controllers/hands, a simple interactable, and basic teleport or locomotion.
   - Build or run against Quest 3S as early as possible.
   - Confirm headset tracking, controller input, interaction, frame stability, and Android build settings.
   - Check in only the smoke-test scene/assets and Unity-generated package/settings changes that are required for XR to work.

4. Split work after the XR smoke test passes.
   - Agent 1 keeps baseline coordination, docs, ignore rules, and XR foundation notes.
   - Agent 2 builds the in-VR scenario flow and interaction foundation.
   - Agent 3 builds validation, dashboard/results, data contracts, and presentation UI.

## Brief Target

The app is a six-scene Quest 3S VR training experience:

- S01 Greeting
- S02 Herd Observation
- S03 Cow Scan Decision
- S04 AI Procedure
- S05 Validation Dashboard
- S06 Results

