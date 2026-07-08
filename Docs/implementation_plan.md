# Implementation Plan: VR Breeding Lab - Dynamic TTS Avatar, Semen Selection, MCQ, Timing & Full AI Procedure Pipeline

This plan addresses the next development tasks for the **Veterinar_VR** Quest standalone training experience. It integrates a dynamic TTS guiding system, tracking of session elapsed time, an MCQ content layer (driven by `QuestionData` and `CowData` ScriptableObjects), a full Beef/Dairy semen selection spec in `S04_AIProcedure`, validation questions in `S05_ValidationDashboard`, scoreboard improvements, and the **full 4-phase AI insemination procedure pipeline** specified in the URS.

---

## URS Feasibility Assessment — Full AI Insemination Procedure

The URS specifies four distinct phases for S04. Here is the verdict for each step on Quest 3S (XRI 3.5, URP):

| Phase | Step | Verdict | Notes |
|---|---|---|---|
| **Thawing** | Forceps grab straw from N₂ tank | ✅ Doable | XR Grab Interactable + Snap Zone |
| | 35–37°C water timer display | ✅ Doable | Reuse `ElapsedTime` subsystem |
| | Straw cutter interaction | ✅ Doable | XR Grab + trigger zone |
| **Assembly** | Straw → pistolette insertion | ✅ Doable | Ordered Snap Zone sequence |
| | AI Sheath attachment | ✅ Doable | Second Snap Zone on pistolette |
| | PD Glove on left virtual hand | ✅ Doable | Visual mesh toggle on left controller |
| **Insertion Logic** | 45° entry angle on vulva, haptic warning | ✅ Doable | `XRBaseController.SendHapticImpulse()` on angle mismatch |
| | Horizontal straighten through vaginal canal | ✅ Doable | Collider trigger zone state transition |
| | Left-hand rectal cervix grip | ⚠️ Partial | Dual-controller tracking works; internal anatomy shown as a 2D cross-section diagram panel rather than 3D volumetric |
| **Deposition Validation** | Position check at cervical zone | ✅ Doable | BoxCollider trigger zone at correct anatomy point |
| | SPATIAL_ERROR_LOG (early / too-deep release) | ✅ Doable | Extend `TrainingSessionState` with `List<string> ProcedureErrors` |

> [!IMPORTANT]
> Interior anatomy (vaginal canal, cervical rings) will be rendered using a **simplified cylinder proxy mesh** with a labelled **2D cross-section diagram overlay**, not volumetric anatomy. This is the recommended approach for standalone VR on Quest 3S.

> [!NOTE]
> The left-hand rectal palpation is represented as a separate controller interaction — the user presses/holds the left Grip to simulate the guiding hand on the cervix while the right hand manipulates the pistolette. No body-physics simulation is required.

---

## Proposed Changes

### Component 1: Core Session Timing & State
We need to track session duration and the selected semen type globally across scenes.

#### [MODIFY] [TrainingSessionState.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Core/TrainingSessionState.cs)
- Add `public float ElapsedTime { get; private set; }` to track training duration in seconds.
- Increment `ElapsedTime += Time.deltaTime` in `Update()` only when the session has active progress (e.g., after the user selects a language, `HasLanguageSelection == true`).
- Add `public string SelectedSemenType { get; private set; } = string.Empty;` to store the genetic material selected in Scene 04.
- Add `public void SelectSemen(string semenType)` to set the semen type.
- Update `ResetSession()` to reset `ElapsedTime = 0f` and `SelectedSemenType = string.Empty`.

---

### Component 2: Data & MCQ Content Layer
Create the required `QuestionData` and `CowData` ScriptableObjects and automate their synchronization using the editor bootstrap script.

#### [NEW] `QuestionData` Assets
Create Question ScriptableObjects in `Assets/_Project/VeterinarVR/ScriptableObjects/Questions/`:
1. `Q_S03_Ready.asset`: Ask if `Cow_B` is ready based on scan parameters (38.5°C and follicle size).
2. `Q_S05_Traits.asset`: Ask which cow shows optimal genetic traits.
3. `Q_S05_Records.asset`: Ask about the primary benefit of digital record-keeping.

#### [NEW] `CowData` Assets
Create Cow ScriptableObjects in `Assets/_Project/VeterinarVR/ScriptableObjects/Cows/`:
1. `Cow_A.asset`: Low milk yield, no heat.
2. `Cow_B.asset` (Holstein): High milk yield, mucus discharge, smart collar. Link `Q_S03_Ready` to its `questionSet`.
3. `Cow_C.asset`: Average milk yield, restless but no physical signs.

#### [MODIFY] [VeterinarProjectBootstrap.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Editor/VeterinarProjectBootstrap.cs)
- Add automation method `EnsureCowAndQuestionDataAssets()` to create and link the Cow/Question ScriptableObjects programmatically.
- Automatically find `ValidationDashboardController` in the `S05_ValidationDashboard` scene and populate the `availableCows` array with references to `Cow_A`, `Cow_B`, and `Cow_C`.
- Update `SyncTrainingContentCatalog` menu item to trigger this scriptable object synchronization.

---

### Component 3: Guide Avatar & TTS Integration
We need a dynamic guiding system. We will build a narrator wrapper that plays speech audio, shows subtitles on the HUD, and drives the avatar's Animator `Talk` trigger. We will also include a reference `StreamojiTtsClient` REST API wrapper.

#### [NEW] [GuideNarrator.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/XR/GuideNarrator.cs)
- Attach this script to the `GuideAvatar` prefab.
- Implement `public void Speak(string text, AudioClip clip = null)`:
  - Play the `AudioClip` via an `AudioSource`.
  - Set the `Talk` trigger/parameter on the `Animator` to play the talk clip.
  - Stop the animation state when the audio finishes playing.
  - Push the text to a world-space subtitle UI panel.

#### [NEW] [StreamojiTtsClient.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/XR/StreamojiTtsClient.cs)
- Implement a client using `UnityWebRequest` that POSTs to the Streamoji endpoint (`https://us-central1-streamoji-265f4.cloudfunctions.net/getAuthToken` and `/avatar_tts`).
- Download the generated speech audio at runtime and pass it to `GuideNarrator`.
- Expose `ClientId` and `ClientSecret` fields for future live cloud credentials mapping.

---

### Component 4: Scene 03 - Cow Scan Decision MCQ
Add the MCQ check before the user is allowed to proceed to Scene 04.

#### [MODIFY] [CowScanController.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Gameplay/CowScanController.cs)
- Instead of showing the "Proceed" button directly after scan completion, display the MCQ Panel containing `Q_S03_Ready`.
- Add `public void AnswerScanQuestion(int optionIndex)` to validate the choice. If correct, award points and show the Proceed button. If incorrect, record a mistake and show feedback.

#### [MODIFY] [S03_CowScanDecision.unity](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scenes/S03_CowScanDecision.unity)
- Build an MCQ panel Canvas in the scene containing a prompt text and option buttons. Hook the buttons up to `CowScanController.AnswerScanQuestion`.

---

### Component 5: Scene 04 - AI Procedure Semen Selection
Upgrade S04 to require choosing Semen genetic material before proceeding.

#### [MODIFY] [AIProcedureController.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Gameplay/AIProcedureController.cs)
- Add state `IsAwaitingSemenSelection` that starts as `true`.
- Add `public void SelectSemen(string semenType)` called by the UI buttons. Sets `SelectedSemenType` on the session state. If "Dairy" is chosen for `Cow_B`, award points; if "Beef" is chosen, record it but proceed.
- Unlock tool pickup and transition `IsAwaitingSemenSelection` to `false` only after semen choice is registered.

#### [MODIFY] [S04_AIProcedure.unity](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scenes/S04_AIProcedure.unity)
- Place two UI Buttons on the `WorldSpaceCanvas` for:
  - **Semen Lembu Daging (Beef)**
  - **Semen Lembu Susu (Dairy)**
- Hide the "Start Procedure" controls until a semen selection is made.

---

### Component 6: Scene 05 - Validation Questions
Integrate the genetic traits and record-keeping MCQ panel on the dashboard.

#### [MODIFY] [ValidationDashboardController.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/UI/ValidationDashboardController.cs)
- Add a panel for Validation Questions (`Q_S05_Traits` and `Q_S05_Records`).
- Require both questions to be answered before enabling the "Proceed to Results" button.

#### [MODIFY] [S05_ValidationDashboard.unity](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scenes/S05_ValidationDashboard.unity)
- Build a Validation MCQ Canvas Panel and hook up the option buttons to `ValidationDashboardController`.

---

### Component 7: Scene 06 - Scoreboard Time & Dynamic Guide Feedback
Update S06 to display session elapsed time and provide dynamic narrator responses.

#### [MODIFY] [ResultsPanelController.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/UI/ResultsPanelController.cs)
- Add `[SerializeField] private Text timeLabel;` to display session time.
- Format `sessionState.ElapsedTime` into `MM:SS` (e.g. `Time: 03:45` or `Masa: 03:45`) and display it.
- Wire the scene's `GuideAvatar` with `GuideNarrator` to speak a dynamic concluding response based on the score outcome tier:
  - **Gold/Excellent:** Praises the user and invites them to the MAHA farm.
  - **Silver/Competent:** Commends the effort and suggests minor practice.
  - **Bronze/Needs Practice:** Suggests repeating the module to refine skills.

#### [MODIFY] [S06_ResultsScoreboard.unity](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scenes/S06_ResultsScoreboard.unity)
- Add the time text component to the results scoreboard canvas UI and bind it to `ResultsPanelController.timeLabel`.

---

### Component 8: Scene 04 — Full AI Procedure Pipeline (URS Phases 1–4)
This is a major upgrade to the existing S04 step-system to implement the full URS specification. The scene will be restructured into four sequential phases, each unlocking the next.

#### [MODIFY] [AIProcedureController.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Gameplay/AIProcedureController.cs)
Replace the flat step array with a **phase-state enum** pattern:

```
Phase 0 → THAWING    (Grab straw, soak timer, cut straw)
Phase 1 → ASSEMBLY   (Insert straw, attach sheath, wear glove)
Phase 2 → INSERTION  (Angle check, straighten, cervix grip)
Phase 3 → DEPOSITION (Spatial zone validation, release semen)
```

- Add `enum AIProcedurePhase { ThawingPrep, EquipmentAssembly, InsertionLogic, DepositionValidation }`
- Add `public AIProcedurePhase CurrentPhase { get; private set; }` driven by phase completion.
- Add `public void AdvancePhase(bool completedCorrectly)` to move between phases and award/deduct points.
- Existing `CurrentStepIndex` and `AdvanceStep()` become sub-step logic within each phase.
- Haptic warning: call `XRBaseController.SendHapticImpulse(amplitude: 0.8f, duration: 0.3f)` when insertion angle < 30° (too flat for vulva entry).
- Add `public void OnAngleEntryValidated(float angleDegrees)` — called from new `InsertionAngleDetector.cs`.

#### [NEW] [InsertionAngleDetector.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Gameplay/InsertionAngleDetector.cs)
- Attach to the pistolette `XR Grab Interactable`.
- In `Update()`, while the user is holding the pistolette and has entered the vulva trigger zone:
  - Read `transform.forward` angle vs world-up.
  - If angle < 30° (horizontal), fire haptic warning via `XRBaseController.SendHapticImpulse()` and display a red warning text on the HUD.
  - If angle ≥ 30°–60° (target range), call `AIProcedureController.OnAngleEntryValidated()` to advance.

#### [NEW] [SpatialErrorLog.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Gameplay/SpatialErrorLog.cs)
- Thin data class: `public static class SpatialErrorLog` with `public static List<string> Entries`.
- `public static void Record(string errorCode, string detail)` appends a timestamp + code + message.
- Error codes: `ANGLE_FLAT`, `RELEASE_TOO_EARLY`, `RELEASE_TOO_DEEP`.
- On session reset, `Entries.Clear()` is called.

#### [MODIFY] [TrainingSessionState.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Core/TrainingSessionState.cs)
- Add `public int SpatialErrorCount { get; private set; }` incremented each time `SpatialErrorLog.Record()` is called.
- Update `ResetSession()` to also clear `SpatialErrorLog.Entries` and reset `SpatialErrorCount = 0`.

#### [NEW] Scene Objects in [S04_AIProcedure.unity](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scenes/S04_AIProcedure.unity)
New GameObjects to be added via Bootstrap or placed manually:

1. **`NitrogenTank`** — Static prop with snap zone. Forceps grab straw from it.
2. **`WarmWaterBeaker`** — Static prop. Has a `ThawingTimerZone` trigger (requires straw to remain inside for 5 seconds real-time to simulate 35–37°C thawing).
3. **`StrawCutter`** — XR Grab → on trigger press, fires a cut animation and advances the thawing phase.
4. **`Pistolette`** — Existing insemination gun. Gets a child `SnapZone` for straw insertion, and a second `SnapZone` for AI Sheath.
5. **`PDGloveToggle`** — A `SkinnedMeshRenderer` on the left controller's hand model, toggled on when the left Grip button is held and the assembly phase is active.
6. **`VulvaEntryZone`** — BoxCollider trigger. Activates `InsertionAngleDetector` monitoring when pistolette enters.
7. **`CervicalDepositZone`** — BoxCollider trigger positioned at the correct anatomical deposit point.
8. **`UterineTooDeepZone`** — BoxCollider trigger slightly past `CervicalDepositZone`. Entry while releasing triggers `RELEASE_TOO_DEEP` error.
9. **`AnatomyDiagramPanel`** — World-space canvas showing a 2D labelled cross-section of the reproductive tract, visible during insertion phase.
10. **`InsertionHUDWarning`** — Text element shown when angle haptic fires ("Dongak lebih — jangan mendatar!").

#### [MODIFY] [VeterinarProjectBootstrap.cs](file:///Users/fihiromar/.gemini/antigravity/worktrees/Veterinar_VR/refactor-agent-task-processing/Assets/_Project/VeterinarVR/Scripts/Editor/VeterinarProjectBootstrap.cs)
- Add `BuildS04FullProcedurePipeline()` method to automate creation of the above 10 scene objects.
- Add `SetupInsertionZoneColliders()` to position the vulva/cervical/uterine trigger volumes relative to the existing cow prop.
- Wire `InsertionAngleDetector` to the pistolette prefab and assign its `AIProcedureController` reference.

---

## Verification Plan

### Automated / Editor-Mode Tests
We can run our bootstrap synchronizations to verify the ScriptableObjects are generated and scene references are wired correctly.
- Execute `Veterinar VR/Bootstrap/Sync Training Content Catalog` to test the scriptable object generation.
- Execute `Veterinar VR/Bootstrap/Sync Scene Set` to verify compile safety of scene changes.

### Manual Verification
- Play through the scenes in the Unity Editor:
  1. In `S01_Greeting`, verify the guide avatar animator triggers talking state.
  2. Verify that `ElapsedTime` increases during playback.
  3. In `S03_CowScanDecision`, verify that the MCQ appears after scan and correctly maps points/penalties before proceeding.
  4. In `S04_AIProcedure`, check that you must select Beef/Dairy semen first, and that selection is recorded.
  5. In `S05_ValidationDashboard`, verify that validation MCQs are presented and validate progress.
  6. In `S06_ResultsScoreboard`, confirm that the total formatted time is displayed, and the guide avatar speaks the correct concluding response according to the score level.
