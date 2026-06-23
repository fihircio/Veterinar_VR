using System;
using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;

namespace VeterinarVR.Gameplay
{
    public sealed class CowScanController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private Text selectedCowLabel;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text feedbackLabel;
        [SerializeField] private GameObject findingsRoot;
        [SerializeField] private GameObject proceedButtonRoot;
        [SerializeField] private string correctFindingId = "MucusDischarge";
        [SerializeField] private int scanReward = 10;
        [SerializeField] private int scanMistakePenalty = 5;

        private TrainingContentCatalog contentCatalog;
        private int appliedScoreDelta;
        private int appliedCorrectDelta;
        private int appliedWrongDelta;
        public bool HasCompletedScan { get; private set; }
        public string ActiveCowId { get; private set; } = string.Empty;
        public string SelectedFindingId { get; private set; } = string.Empty;

        public event Action<string> ScanStarted;
        public event Action<string, bool> ScanCompleted;

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
            RefreshView(IsBahasa()
                ? "Mulakan imbasan, kemudian pilih hotspot pada lembu."
                : "Start the scan, then choose a hotspot on the cow.");
        }

        public void BeginScan()
        {
            ActiveCowId = sessionState != null ? sessionState.SelectedCowId : string.Empty;
            HasCompletedScan = false;
            SelectedFindingId = string.Empty;
            ScanStarted?.Invoke(ActiveCowId);
            AudioRuntimeDirector.PlayUiClick();
            RefreshView(IsBahasa()
                ? "Pengimbas sedia. Semak petunjuk birahi yang kelihatan."
                : "Scanner ready. Review the visible heat indicators.");
        }

        public void SelectTailRaise()
        {
            CompleteScan("TailRaise");
        }

        public void SelectMucusDischarge()
        {
            CompleteScan("MucusDischarge");
        }

        public void SelectRestlessness()
        {
            CompleteScan("Restlessness");
        }

        public void ProceedToResults()
        {
            if (!HasCompletedScan || sceneLoader == null)
            {
                return;
            }

            AudioRuntimeDirector.PlayUiConfirm();
            sceneLoader.LoadAIProcedure();
        }

        public void CompleteScan(string findingId)
        {
            if (string.IsNullOrWhiteSpace(ActiveCowId))
            {
                ActiveCowId = sessionState != null ? sessionState.SelectedCowId : string.Empty;
            }

            var nextFindingId = string.IsNullOrWhiteSpace(findingId) ? string.Empty : findingId.Trim();
            if (HasCompletedScan && string.Equals(SelectedFindingId, nextFindingId, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (HasCompletedScan && sessionState != null)
            {
                sessionState.ApplyScoreDelta(-appliedScoreDelta, -appliedCorrectDelta, -appliedWrongDelta);
            }

            SelectedFindingId = nextFindingId;
            HasCompletedScan = true;

            if (sessionState != null)
            {
                sessionState.SetScanFinding(SelectedFindingId);
            }

            var configuredCorrectFindingId = contentCatalog != null ? contentCatalog.CorrectFindingId : correctFindingId;
            var configuredScanReward = contentCatalog != null ? contentCatalog.ScanReward : scanReward;
            var configuredScanPenalty = contentCatalog != null ? contentCatalog.ScanMistakePenalty : scanMistakePenalty;
            var suspectedIssueFound = string.Equals(SelectedFindingId, configuredCorrectFindingId, StringComparison.OrdinalIgnoreCase);
            appliedScoreDelta = suspectedIssueFound ? configuredScanReward : -Mathf.Abs(configuredScanPenalty);
            appliedCorrectDelta = suspectedIssueFound ? 1 : 0;
            appliedWrongDelta = suspectedIssueFound ? 0 : 1;

            ScanCompleted?.Invoke(ActiveCowId, suspectedIssueFound);
            AudioRuntimeDirector.PlayCowAccent();
            if (sessionState != null)
            {
                sessionState.ApplyScoreDelta(appliedScoreDelta, appliedCorrectDelta, appliedWrongDelta);
            }
            else if (scoreManager != null)
            {
                if (suspectedIssueFound)
                {
                    scoreManager.AwardPoints(configuredScanReward);
                }
                else
                {
                    scoreManager.RegisterMistake(configuredScanPenalty);
                }
            }

            RefreshView(suspectedIssueFound
                ? IsBahasa()
                    ? $"{GetFindingDisplayName(SelectedFindingId)} ialah petunjuk pembiakan terbaik untuk {GetCowDisplayName(ActiveCowId)}."
                    : $"{GetFindingDisplayName(SelectedFindingId)} is the best breeding indicator for {GetCowDisplayName(ActiveCowId)}."
                : IsBahasa()
                    ? $"{GetFindingDisplayName(SelectedFindingId)} bukan petunjuk pembiakan terkuat untuk {GetCowDisplayName(ActiveCowId)}."
                    : $"{GetFindingDisplayName(SelectedFindingId)} is not the strongest breeding indicator for {GetCowDisplayName(ActiveCowId)}.");
        }

        public void ResetScan()
        {
            HasCompletedScan = false;
            ActiveCowId = string.Empty;
            SelectedFindingId = string.Empty;
            appliedScoreDelta = 0;
            appliedCorrectDelta = 0;
            appliedWrongDelta = 0;
            RefreshView();
        }

        private void RefreshView(string feedback = "")
        {
            if (selectedCowLabel != null)
            {
                var targetCow = string.IsNullOrWhiteSpace(ActiveCowId)
                    ? (sessionState != null ? sessionState.SelectedCowId : string.Empty)
                    : ActiveCowId;
                selectedCowLabel.text = string.IsNullOrWhiteSpace(targetCow)
                    ? IsBahasa() ? "Lembu Dipilih: Tiada" : "Selected Cow: None"
                    : IsBahasa() ? $"Lembu Dipilih: {GetCowDisplayName(targetCow)}" : $"Selected Cow: {GetCowDisplayName(targetCow)}";
            }

            if (statusLabel != null)
            {
                statusLabel.text = HasCompletedScan
                    ? IsBahasa() ? "Keputusan direkodkan" : "Decision recorded"
                    : IsBahasa() ? "Mulakan imbasan untuk memeriksa petunjuk birahi" : "Start the scan to inspect heat indicators";
            }

            if (findingsRoot != null)
            {
                findingsRoot.SetActive(!HasCompletedScan && !string.IsNullOrWhiteSpace(ActiveCowId));
            }

            if (proceedButtonRoot != null)
            {
                proceedButtonRoot.SetActive(HasCompletedScan);
            }

            if (feedbackLabel != null)
            {
                feedbackLabel.text = feedback;
            }
        }

        private bool IsBahasa()
        {
            return sessionState != null && sessionState.SelectedLanguage == SessionLanguage.BahasaMelayu;
        }

        private string GetCowDisplayName(string cowId)
        {
            if (contentCatalog != null)
            {
                return contentCatalog.GetCowDisplayName(cowId, sessionState != null ? sessionState.SelectedLanguage : SessionLanguage.English);
            }

            return cowId switch
            {
                "Cow_A" => IsBahasa() ? "Lembu A" : "Cow A",
                "Cow_B" => IsBahasa() ? "Lembu B" : "Cow B",
                "Cow_C" => IsBahasa() ? "Lembu C" : "Cow C",
                _ => cowId.Replace('_', ' ')
            };
        }

        private string GetFindingDisplayName(string findingId)
        {
            if (contentCatalog != null)
            {
                return contentCatalog.GetFindingDisplayName(findingId, sessionState != null ? sessionState.SelectedLanguage : SessionLanguage.English);
            }

            return findingId switch
            {
                "TailRaise" => IsBahasa() ? "Ekor Diangkat" : "Tail Raising",
                "MucusDischarge" => IsBahasa() ? "Lelehan Mukus" : "Mucus Discharge",
                "Restlessness" => IsBahasa() ? "Gelisah" : "Restlessness",
                _ => findingId
            };
        }
    }
}
