using System;
using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;

namespace VeterinarVR.Gameplay
{
    public enum AIProcedurePhase
    {
        SemenSelection,
        ThawingPrep,
        EquipmentAssembly,
        InsertionLogic,
        DepositionValidation,
        Complete
    }

    public sealed class AIProcedureController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private Text stepLabel;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text feedbackLabel;
        [SerializeField] private GameObject beginButtonRoot;
        [SerializeField] private GameObject actionButtonsRoot;
        [SerializeField] private GameObject completeStepButtonRoot;
        [SerializeField] private GameObject proceedButtonRoot;
        [SerializeField] private int completionReward = 15;
        [SerializeField] private int skipPenalty = 5;
        [SerializeField] private int pickupStepIndex;
        [SerializeField] private int placementStepIndex = 1;
        [SerializeField] private string[] procedureSteps = Array.Empty<string>();
        [SerializeField] private string[] bahasaProcedureSteps = Array.Empty<string>();

        private TrainingContentCatalog contentCatalog;
        public int CurrentStepIndex { get; private set; }
        public bool HasStarted { get; private set; }
        public bool IsAwaitingSemenSelection { get; private set; } = true;
        public bool IsProcedureComplete => CurrentStepIndex >= GetStepCount() && GetStepCount() > 0;
        public bool IsAwaitingPlacementInteraction =>
            HasStarted &&
            !IsProcedureComplete &&
            CurrentStepIndex == placementStepIndex;

        public bool IsAwaitingToolPickupInteraction =>
            HasStarted &&
            !IsProcedureComplete &&
            CurrentStepIndex == pickupStepIndex;

        public bool IsAwaitingFinalCheckpointInteraction =>
            HasStarted &&
            !IsProcedureComplete &&
            !IsAwaitingToolPickupInteraction &&
            !IsAwaitingPlacementInteraction;

        public event Action<int, string> StepAdvanced;
        public event Action ProcedureCompleted;
        public event Action<AIProcedurePhase> PhaseAdvanced;

        public AIProcedurePhase CurrentPhase { get; private set; } = AIProcedurePhase.SemenSelection;

        public void AdvancePhase(bool completedCorrectly = true)
        {
            if (CurrentPhase == AIProcedurePhase.Complete) return;

            CurrentPhase = CurrentPhase switch
            {
                AIProcedurePhase.SemenSelection    => AIProcedurePhase.ThawingPrep,
                AIProcedurePhase.ThawingPrep       => AIProcedurePhase.EquipmentAssembly,
                AIProcedurePhase.EquipmentAssembly => AIProcedurePhase.InsertionLogic,
                AIProcedurePhase.InsertionLogic    => AIProcedurePhase.DepositionValidation,
                AIProcedurePhase.DepositionValidation => AIProcedurePhase.Complete,
                _ => AIProcedurePhase.Complete
            };

            if (completedCorrectly && scoreManager != null)
            {
                scoreManager.AwardPoints(10);
            }

            AudioRuntimeDirector.PlayProcedureFoley();
            PhaseAdvanced?.Invoke(CurrentPhase);

            if (CurrentPhase == AIProcedurePhase.Complete)
            {
                ProcedureCompleted?.Invoke();
            }
        }

        /// <summary>Called by InsertionAngleDetector when the 45-degree entry angle is confirmed correct.</summary>
        public void OnAngleEntryValidated(float angleDegrees)
        {
            if (CurrentPhase != AIProcedurePhase.InsertionLogic) return;
            RefreshView(IsBahasa()
                ? $"Sudut masuk betul ({angleDegrees:F0}°). Luruskan pistolet dan teruskan."
                : $"Correct entry angle ({angleDegrees:F0}°). Straighten the pistolette and continue.");
            CompletePickupStep();
        }

        private void Awake()
        {
            if (sessionState == null)
            {
                sessionState = FindFirstObjectByType<TrainingSessionState>();
            }

            if (scoreManager == null)
            {
                scoreManager = FindFirstObjectByType<ScoreManager>();
            }

            if (sceneLoader == null)
            {
                sceneLoader = FindFirstObjectByType<SceneLoader>();
            }

            contentCatalog = TrainingContentCatalog.LoadDefault();
        }

        private void Start()
        {
            RefreshView();
        }

        public void SelectSemen(string semenType)
        {
            if (string.IsNullOrWhiteSpace(semenType))
            {
                return;
            }

            IsAwaitingSemenSelection = false;

            if (sessionState != null)
            {
                sessionState.SelectSemen(semenType);
            }

            // Award bonus points for correct semen choice (Dairy for Cow_B Holstein)
            bool isDairyForHolstein = semenType.Contains("Dairy") || semenType.Contains("Susu");
            bool isCowB = sessionState != null && sessionState.SelectedCowId == "Cow_B";
            if (isDairyForHolstein && isCowB && scoreManager != null)
            {
                scoreManager.AwardPoints(10);
            }

            AudioRuntimeDirector.PlayUiClick();
            RefreshView(IsBahasa()
                ? $"Semen dipilih: {semenType}. Tekan Mula Prosedur untuk meneruskan."
                : $"Semen selected: {semenType}. Press Start Procedure to continue.");
        }

        public void StartProcedure()
        {
            if (IsAwaitingSemenSelection)
            {
                RefreshView(IsBahasa()
                    ? "Sila pilih jenis semen sebelum memulakan prosedur."
                    : "Please select a semen type before starting the procedure.");
                return;
            }

            if (GetStepCount() == 0)
            {
                RefreshView(IsBahasa() ? "Tiada langkah prosedur dikonfigurasikan." : "No procedure steps are configured.");
                return;
            }

            CurrentStepIndex = 0;
            HasStarted = true;
            UpdateSessionProgress();
            AudioRuntimeDirector.PlayUiClick();
            RefreshView(IsBahasa() ? "Prosedur dimulakan. Lengkapkan setiap langkah mengikut turutan." : "Procedure started. Complete each step in order.");

            if (GetStepCount() > 0)
            {
                StepAdvanced?.Invoke(CurrentStepIndex, GetCurrentStepLabel());
            }
        }

        public void CompleteCurrentStep()
        {
            if (IsAwaitingPlacementInteraction)
            {
                RefreshView(IsBahasa()
                    ? "Ambil alat inseminasi dan letakkan di zon sasaran."
                    : "Grab the insemination tool and place it into the target zone.");
                return;
            }

            AdvanceStep(true);
        }

        public void SkipCurrentStep()
        {
            AdvanceStep(false);
        }

        public void ProceedToDashboard()
        {
            if (!IsProcedureComplete || sceneLoader == null)
            {
                return;
            }

            sceneLoader.LoadValidationDashboard();
        }

        public void CompletePlacementStep()
        {
            if (!IsAwaitingPlacementInteraction)
            {
                return;
            }

            AdvanceStep(true);
        }

        public void CompletePickupStep()
        {
            if (!IsAwaitingToolPickupInteraction)
            {
                return;
            }

            AdvanceStep(true);
        }

        public void CompleteFinalCheckpointStep()
        {
            if (!IsAwaitingFinalCheckpointInteraction)
            {
                return;
            }

            AdvanceStep(true);
        }

        public void AdvanceStep(bool completedCorrectly = true)
        {
            if (!HasStarted || GetStepCount() == 0 || IsProcedureComplete)
            {
                return;
            }

            var currentStep = GetStepLabelAt(CurrentStepIndex);
            var configuredCompletionReward = contentCatalog != null ? contentCatalog.ProcedureCompletionReward : completionReward;
            var configuredSkipPenalty = contentCatalog != null ? contentCatalog.ProcedureSkipPenalty : skipPenalty;
            if (scoreManager != null)
            {
                if (completedCorrectly)
                {
                    scoreManager.AwardPoints(configuredCompletionReward);
                }
                else
                {
                    scoreManager.RegisterMistake(configuredSkipPenalty);
                }
            }

            CurrentStepIndex++;
            UpdateSessionProgress();
            var feedback = completedCorrectly
                ? IsBahasa()
                    ? $"{GetStepLabelAt(CurrentStepIndex - 1)} selesai."
                    : $"{currentStep} completed."
                : IsBahasa()
                    ? $"{GetStepLabelAt(CurrentStepIndex - 1)} dilangkau atau diselesaikan dengan salah."
                    : $"{currentStep} skipped or completed incorrectly.";

            if (!IsProcedureComplete)
            {
                if (IsAwaitingPlacementInteraction)
                {
                    feedback += IsBahasa()
                        ? " Seterusnya, letakkan alat pada zon sasaran."
                        : " Next, place the tool in the target zone.";
                }
                else if (IsAwaitingFinalCheckpointInteraction)
                {
                    feedback += IsBahasa()
                        ? " Seterusnya, klik Deliver Dose di dalam babak."
                        : " Next, click Deliver Dose in the scene.";
                }
            }

            if (IsProcedureComplete)
            {
                AudioRuntimeDirector.PlayUiConfirm();
                ProcedureCompleted?.Invoke();
                RefreshView(IsBahasa() ? feedback + " Prosedur selesai." : feedback + " Procedure complete.");
                return;
            }

            AudioRuntimeDirector.PlayProcedureFoley();
            StepAdvanced?.Invoke(CurrentStepIndex, GetCurrentStepLabel());
            RefreshView(feedback);
        }

        public void ResetProcedure()
        {
            CurrentStepIndex = 0;
            HasStarted = false;
            UpdateSessionProgress();
            RefreshView();
        }

        private void UpdateSessionProgress()
        {
            if (sessionState == null)
            {
                return;
            }

            var stepCount = GetStepCount();
            var completedSteps = Mathf.Clamp(CurrentStepIndex, 0, stepCount);
            sessionState.SetProcedureProgress(completedSteps, stepCount, IsProcedureComplete);
        }

        private void RefreshView(string feedback = "")
        {
            if (stepLabel != null)
            {
                stepLabel.text = GetStepCount() == 0
                    ? IsBahasa() ? "Tiada langkah prosedur dikonfigurasikan" : "No procedure steps configured"
                    : !HasStarted
                    ? IsBahasa() ? "Tekan Mula Prosedur" : "Press Start Procedure"
                    : IsProcedureComplete
                        ? IsBahasa() ? "Semua langkah inseminasi selesai" : "All insemination steps completed"
                        : IsBahasa()
                            ? $"Langkah {CurrentStepIndex + 1}: {GetCurrentStepLabel()}"
                            : $"Step {CurrentStepIndex + 1}: {GetCurrentStepLabel()}";
            }

            if (statusLabel != null)
            {
                statusLabel.text = GetStepCount() == 0
                    ? IsBahasa() ? "Tambah langkah prosedur sebelum menguji babak ini" : "Add procedure steps before testing this scene"
                    : !HasStarted
                    ? IsBahasa() ? "Sediakan stesen prosedur AI" : "Prepare the AI procedure station"
                    : IsProcedureComplete
                        ? IsBahasa() ? "Prosedur selesai" : "Procedure complete"
                        : IsAwaitingToolPickupInteraction
                            ? IsBahasa() ? "Gerakkan alat inseminasi dari dulang untuk memulakan prosedur" : "Move the insemination tool off the tray to begin the procedure"
                        : IsAwaitingPlacementInteraction
                            ? IsBahasa() ? "Ambil alat inseminasi dan letakkan di zon sasaran" : "Grab the insemination tool and place it in the target zone"
                            : IsBahasa() ? "Aktifkan titik penghantaran akhir untuk melengkapkan prosedur" : "Activate the final delivery checkpoint to complete the procedure";
            }

            if (beginButtonRoot != null)
            {
                beginButtonRoot.SetActive(!HasStarted && !IsAwaitingSemenSelection);
            }

            if (actionButtonsRoot != null)
            {
                actionButtonsRoot.SetActive(HasStarted && !IsProcedureComplete);
            }

            if (completeStepButtonRoot != null)
            {
                completeStepButtonRoot.SetActive(false);
            }

            if (proceedButtonRoot != null)
            {
                proceedButtonRoot.SetActive(IsProcedureComplete);
            }

            if (feedbackLabel != null)
            {
                feedbackLabel.text = feedback;
            }
        }

        private string GetCurrentStepLabel()
        {
            return GetStepLabelAt(CurrentStepIndex);
        }

        private string GetStepLabelAt(int index)
        {
            var steps = GetStepsForCurrentLanguage();
            if (index < 0 || index >= steps.Length)
            {
                return string.Empty;
            }

            return steps[index];
        }

        private bool IsBahasa()
        {
            return sessionState != null && sessionState.SelectedLanguage == SessionLanguage.BahasaMelayu;
        }

        private int GetStepCount()
        {
            return GetStepsForCurrentLanguage().Length;
        }

        private string[] GetStepsForCurrentLanguage()
        {
            if (contentCatalog != null)
            {
                return contentCatalog.GetProcedureSteps(IsBahasa() ? SessionLanguage.BahasaMelayu : SessionLanguage.English);
            }

            if (IsBahasa() && bahasaProcedureSteps != null && bahasaProcedureSteps.Length > 0)
            {
                return bahasaProcedureSteps;
            }

            return procedureSteps ?? Array.Empty<string>();
        }
    }
}
