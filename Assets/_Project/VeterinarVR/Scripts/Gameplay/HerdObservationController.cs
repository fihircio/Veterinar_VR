using System;
using UnityEngine;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;
using UnityEngine.UI;

namespace VeterinarVR.Gameplay
{
    public sealed class HerdObservationController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private string fallbackCowId = "Cow_01";
        [SerializeField] private string correctCowId = "Cow_B";
        [SerializeField] private bool autoSelectOnStart;
        [SerializeField] private int correctSelectionReward = 25;
        [SerializeField] private int incorrectSelectionPenalty = 10;
        [SerializeField] private GameObject proceedButtonRoot;
        [SerializeField] private Text feedbackLabel;

        private TrainingContentCatalog contentCatalog;
        private int appliedScoreDelta;
        private int appliedCorrectDelta;
        private int appliedWrongDelta;

        public string CurrentCowId { get; private set; } = string.Empty;
        public bool HasResolvedSelection { get; private set; }

        public event Action<string> CowSelectionChanged;

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
            RefreshView(GetSelectedLanguage() == SessionLanguage.BahasaMelayu
                ? "Pilih lembu secara terus di kawasan ragut."
                : "Select a cow directly in the field.");

            if (autoSelectOnStart && !string.IsNullOrWhiteSpace(fallbackCowId))
            {
                ResolveSelection(fallbackCowId);
            }
        }

        public void ResolveSelection(string cowId)
        {
            var nextCowId = string.IsNullOrWhiteSpace(cowId) ? fallbackCowId : cowId.Trim();
            if (HasResolvedSelection && string.Equals(CurrentCowId, nextCowId, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (HasResolvedSelection && sessionState != null)
            {
                sessionState.ApplyScoreDelta(-appliedScoreDelta, -appliedCorrectDelta, -appliedWrongDelta);
            }

            CurrentCowId = nextCowId;
            HasResolvedSelection = true;

            if (sessionState != null)
            {
                sessionState.SelectCow(CurrentCowId);
            }

            var configuredCorrectCowId = contentCatalog != null ? contentCatalog.CorrectCowId : correctCowId;
            var selectionReward = contentCatalog != null ? contentCatalog.HerdCorrectReward : correctSelectionReward;
            var selectionPenalty = contentCatalog != null ? contentCatalog.HerdIncorrectPenalty : incorrectSelectionPenalty;
            var isCorrect = string.Equals(CurrentCowId, configuredCorrectCowId, StringComparison.OrdinalIgnoreCase);
            appliedScoreDelta = isCorrect ? selectionReward : -Mathf.Abs(selectionPenalty);
            appliedCorrectDelta = isCorrect ? 1 : 0;
            appliedWrongDelta = isCorrect ? 0 : 1;

            if (sessionState != null)
            {
                sessionState.ApplyScoreDelta(appliedScoreDelta, appliedCorrectDelta, appliedWrongDelta);
            }
            else if (scoreManager != null)
            {
                if (isCorrect)
                {
                    scoreManager.AwardPoints(selectionReward);
                }
                else
                {
                    scoreManager.RegisterMistake(selectionPenalty);
                }
            }

            CowSelectionChanged?.Invoke(CurrentCowId);
            AudioRuntimeDirector.PlayCowAccent();
            RefreshView(isCorrect
                ? GetSelectedLanguage() == SessionLanguage.BahasaMelayu
                    ? $"{GetCowDisplayName(CurrentCowId)} menunjukkan tanda birahi yang paling jelas."
                    : $"{GetCowDisplayName(CurrentCowId)} shows the clearest heat signs."
                : GetSelectedLanguage() == SessionLanguage.BahasaMelayu
                    ? $"{GetCowDisplayName(CurrentCowId)} bukan calon terbaik untuk tetingkap pembiakan."
                    : $"{GetCowDisplayName(CurrentCowId)} is not the best breeding-window candidate.");
        }

        public void ProceedToResults()
        {
            if (!HasResolvedSelection || sceneLoader == null)
            {
                return;
            }

            AudioRuntimeDirector.PlayUiConfirm();
            sceneLoader.LoadCowScanDecision();
        }

        public void ClearSelection()
        {
            CurrentCowId = string.Empty;
            HasResolvedSelection = false;
            appliedScoreDelta = 0;
            appliedCorrectDelta = 0;
            appliedWrongDelta = 0;
            CowSelectionChanged?.Invoke(CurrentCowId);
            RefreshView(string.Empty);
        }

        private void RefreshView(string feedback)
        {
            if (proceedButtonRoot != null)
            {
                proceedButtonRoot.SetActive(HasResolvedSelection);
            }

            if (feedbackLabel != null)
            {
                feedbackLabel.text = feedback;
            }
        }

        private SessionLanguage GetSelectedLanguage()
        {
            return sessionState != null ? sessionState.SelectedLanguage : SessionLanguage.English;
        }

        private string GetCowDisplayName(string cowId)
        {
            if (contentCatalog != null)
            {
                return contentCatalog.GetCowDisplayName(cowId, GetSelectedLanguage());
            }

            return cowId switch
            {
                "Cow_A" => GetSelectedLanguage() == SessionLanguage.BahasaMelayu ? "Lembu A" : "Cow A",
                "Cow_B" => GetSelectedLanguage() == SessionLanguage.BahasaMelayu ? "Lembu B" : "Cow B",
                "Cow_C" => GetSelectedLanguage() == SessionLanguage.BahasaMelayu ? "Lembu C" : "Cow C",
                _ => cowId.Replace('_', ' ')
            };
        }
    }
}
