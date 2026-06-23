using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;

namespace VeterinarVR.UI
{
    public sealed class ValidationDashboardController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private CowData[] availableCows = new CowData[0];
        [SerializeField] private Text selectedCowLabel;
        [SerializeField] private Text selectedFindingLabel;
        [SerializeField] private Text procedureStatusLabel;
        [SerializeField] private Text scoreLabel;
        [SerializeField] private Text decisionLabel;
        [SerializeField] private Text statusLabel;
        [SerializeField] private GameObject proceedButtonRoot;

        private TrainingContentCatalog contentCatalog;
        public string SelectedCowId => sessionState != null ? sessionState.SelectedCowId : string.Empty;
        public int AvailableCowCount => availableCows.Length;

        private void Awake()
        {
            if (sessionState == null)
            {
                sessionState = FindFirstObjectByType<TrainingSessionState>();
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

        public void MarkBred()
        {
            SetDecision("Bred");
        }

        public void MarkNotBred()
        {
            SetDecision("Not Bred");
        }

        public void ProceedToResults()
        {
            if (sessionState == null || string.IsNullOrWhiteSpace(sessionState.ValidationDecision) || sceneLoader == null)
            {
                return;
            }

            AudioRuntimeDirector.PlayUiConfirm();
            sceneLoader.LoadResults();
        }

        public CowData GetSelectedCow()
        {
            if (sessionState == null)
            {
                return null;
            }

            var selectedCowId = sessionState.SelectedCowId;
            for (var i = 0; i < availableCows.Length; i++)
            {
                var cow = availableCows[i];
                if (cow != null && cow.CowId == selectedCowId)
                {
                    return cow;
                }
            }

            return null;
        }

        private void SetDecision(string decision)
        {
            if (sessionState == null)
            {
                return;
            }

            sessionState.SetValidationDecision(decision);
            AudioRuntimeDirector.PlayUiClick();
            RefreshView();
        }

        private void RefreshView()
        {
            if (selectedCowLabel != null)
            {
                var cowDisplay = GetCowDisplayName(SelectedCowId);
                selectedCowLabel.text = string.IsNullOrWhiteSpace(SelectedCowId)
                    ? IsBahasa() ? "Lembu Dipilih: Tiada" : "Selected Cow: None"
                    : IsBahasa() ? $"Lembu Dipilih: {cowDisplay}" : $"Selected Cow: {cowDisplay}";
            }

            if (scoreLabel != null)
            {
                scoreLabel.text = sessionState == null
                    ? IsBahasa() ? "Skor: 0" : "Score: 0"
                    : IsBahasa() ? $"Skor: {sessionState.TotalScore}" : $"Score: {sessionState.TotalScore}";
            }

            if (selectedFindingLabel != null)
            {
                var findingDisplay = GetFindingDisplayName(sessionState != null ? sessionState.SelectedFindingId : string.Empty);
                selectedFindingLabel.text = sessionState == null || string.IsNullOrWhiteSpace(sessionState.SelectedFindingId)
                    ? IsBahasa() ? "Petunjuk: Belum ada" : "Finding: Pending"
                    : IsBahasa() ? $"Petunjuk: {findingDisplay}" : $"Finding: {findingDisplay}";
            }

            if (procedureStatusLabel != null)
            {
                procedureStatusLabel.text = sessionState == null
                    ? IsBahasa() ? "Prosedur: 0/0 langkah" : "Procedure: 0/0 steps"
                    : sessionState.IsProcedureCompleted
                        ? IsBahasa()
                            ? $"Prosedur: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} langkah selesai"
                            : $"Procedure: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} steps complete"
                        : IsBahasa()
                            ? $"Prosedur: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} langkah"
                            : $"Procedure: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} steps";
            }

            if (decisionLabel != null)
            {
                var decisionDisplay = GetDecisionDisplayName(sessionState != null ? sessionState.ValidationDecision : string.Empty);
                decisionLabel.text = sessionState == null || string.IsNullOrWhiteSpace(sessionState.ValidationDecision)
                    ? IsBahasa() ? "Keputusan: Belum ada" : "Decision: Pending"
                    : IsBahasa() ? $"Keputusan: {decisionDisplay}" : $"Decision: {decisionDisplay}";
            }

            if (statusLabel != null)
            {
                statusLabel.text = sessionState == null || string.IsNullOrWhiteSpace(sessionState.ValidationDecision)
                    ? IsBahasa() ? "Semak rekod sesi dan pilih status pembiakan." : "Review the session record and choose the breeding status."
                    : IsBahasa() ? "Keputusan direkodkan. Teruskan ke paparan keputusan akhir." : "Decision recorded. Continue to the final results view.";
            }

            if (proceedButtonRoot != null)
            {
                proceedButtonRoot.SetActive(sessionState != null && !string.IsNullOrWhiteSpace(sessionState.ValidationDecision));
            }
        }

        private bool IsBahasa()
        {
            return sessionState != null && sessionState.SelectedLanguage == SessionLanguage.BahasaMelayu;
        }

        private string GetCowDisplayName(string cowId)
        {
            if (string.IsNullOrWhiteSpace(cowId))
            {
                return string.Empty;
            }

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
            if (string.IsNullOrWhiteSpace(findingId))
            {
                return string.Empty;
            }

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

        private string GetDecisionDisplayName(string decision)
        {
            if (string.IsNullOrWhiteSpace(decision))
            {
                return string.Empty;
            }

            if (contentCatalog != null)
            {
                return contentCatalog.GetDecisionDisplayName(decision, sessionState != null ? sessionState.SelectedLanguage : SessionLanguage.English);
            }

            return decision switch
            {
                "Bred" => IsBahasa() ? "Bunting" : "Bred",
                "Not Bred" => IsBahasa() ? "Tidak Bunting" : "Not Bred",
                _ => decision
            };
        }
    }
}
