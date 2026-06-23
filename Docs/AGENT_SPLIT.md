# Agent Split

Use short-lived UVCS branches from `/main`. Rebase or merge only after checking current status, because multiple agents may be editing the workspace.

## Branch Naming

- Agent 1: `/main/agent1-baseline-xr-foundation`
- Agent 2: `/main/agent2-vr-scenario-flow`
- Agent 3: `/main/agent3-validation-dashboard`

## Agent 1: Collaboration Baseline and XR Foundation

Owns:

- `ignore.conf`
- `Docs/**`
- XR setup notes and smoke-test acceptance criteria

Avoids unless explicitly coordinated:

- `Assets/**`
- `Packages/manifest.json`
- `Packages/packages-lock.json`
- `ProjectSettings/**`
- Unity scene files

## Agent 2: VR Scenario Flow and Core Interaction

Owns after XR smoke test:

- S01 Greeting
- S02 Herd Observation
- S03 Cow Scan Decision
- S04 AI Procedure
- XR Origin scene setup, locomotion, scan interaction, cow selection, and procedure interaction assets/scripts

Suggested paths:

- `Assets/Scenes/S01_Greeting*`
- `Assets/Scenes/S02_HerdObservation*`
- `Assets/Scenes/S03_CowScanDecision*`
- `Assets/Scenes/S04_AIProcedure*`
- `Assets/Scripts/Scenario/**`
- `Assets/Scripts/XR/**`
- `Assets/Prefabs/XR/**`
- `Assets/Prefabs/Scenario/**`

Coordinate before editing:

- Shared script assemblies
- Global input actions
- Project-wide XR settings
- Shared UI/data models used by Agent 3

## Agent 3: Validation, Dashboard, Results, and Data Contracts

Owns after XR smoke test:

- S05 Validation Dashboard
- S06 Results
- Training outcome state, scoring/validation summaries, dashboard UI, result presentation, and data handoff from Agent 2

Suggested paths:

- `Assets/Scenes/S05_ValidationDashboard*`
- `Assets/Scenes/S06_Results*`
- `Assets/Scripts/Validation/**`
- `Assets/Scripts/Results/**`
- `Assets/Scripts/Data/**`
- `Assets/Prefabs/UI/**`
- `Assets/Prefabs/Dashboard/**`

Coordinate before editing:

- Scenario events emitted by Agent 2
- Shared save/session data
- Any package, input, or ProjectSettings change

## Shared Rules

- Check status before editing and before check-in.
- Do not revert another agent's work.
- Keep Unity-generated metadata with its asset when Unity creates it.
- Keep generated local files ignored: `Library`, `Temp`, `Logs`, `UserSettings`, IDE files, build outputs, `*.csproj`, `*.sln`, and `*.slnx`.
- Package and ProjectSettings edits should come from Unity Editor changes whenever possible.
