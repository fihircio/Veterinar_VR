using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;
using VeterinarVR.XR;

namespace VeterinarVR.UI
{
    public sealed class ResultsPanelController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameObject resultsRoot;
        [SerializeField] private Text selectedCowLabel;
        [SerializeField] private Text findingLabel;
        [SerializeField] private Text procedureLabel;
        [SerializeField] private Text scoreLabel;
        [SerializeField] private Text correctLabel;
        [SerializeField] private Text wrongLabel;
        [SerializeField] private Text decisionLabel;
        [SerializeField] private Text outcomeLabel;
        [SerializeField] private Text badgeLabel;
        [SerializeField] private Text recommendationLabel;
        [SerializeField] private Text timeLabel;

        private TrainingContentCatalog contentCatalog;
        public int TotalScore => sessionState != null ? sessionState.TotalScore : 0;
        public int CorrectAnswers => sessionState != null ? sessionState.CorrectAnswers : 0;
        public int WrongAnswers => sessionState != null ? sessionState.WrongAnswers : 0;

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
            AudioRuntimeDirector.PlayResultCue();
            RefreshView();
            TriggerGuideFeedback();
        }

        public void ShowResults()
        {
            if (resultsRoot != null)
            {
                resultsRoot.SetActive(true);
            }

            RefreshView();
            TriggerGuideFeedback();
        }

        private void TriggerGuideFeedback()
        {
            var narrator = FindFirstObjectByType<GuideNarrator>();
            if (narrator == null || sessionState == null)
            {
                return;
            }

            string badge = GetBadgeTier();
            string text = "";

            if (IsBahasa())
            {
                if (badge == "Emas" || sessionState.TotalScore >= 60)
                {
                    text = "Tahniah! Anda telah menunjukkan kecekapan luar biasa dalam pembiakan ruminan. Anda bersedia untuk menerokai ladang fizikal di MAHA!";
                }
                else if (badge == "Perak" || sessionState.TotalScore >= 35)
                {
                    text = "Syabas! Anda kompeten dalam pembiakan ruminan. Latih lagi untuk mencapai ketepatan sempurna.";
                }
                else
                {
                    text = "Anda telah menyelesaikan latihan, tetapi masih ada ruang untuk penambahbaikan. Sila ulangi modul untuk memperkemas kemahiran anda.";
                }
            }
            else
            {
                if (badge == "Gold" || sessionState.TotalScore >= 60)
                {
                    text = "Congratulations! You have demonstrated exceptional mastery in ruminant breeding. You are ready to explore the physical farm at MAHA!";
                }
                else if (badge == "Silver" || sessionState.TotalScore >= 35)
                {
                    text = "Great job! You are competent in ruminant breeding. Practice a bit more to achieve perfect accuracy.";
                }
                else
                {
                    text = "You've completed the training, but there is room for improvement. Please repeat the modules to refine your skills.";
                }
            }

            narrator.Speak(text);
        }

        public void HideResults()
        {
            if (resultsRoot != null)
            {
                resultsRoot.SetActive(false);
            }
        }

        public void RestartExperience()
        {
            if (sessionState != null)
            {
                sessionState.ResetSession();
            }

            if (sceneLoader != null)
            {
                AudioRuntimeDirector.PlayUiConfirm();
                sceneLoader.LoadGreeting();
            }
        }

        private void RefreshView()
        {
            if (sessionState == null)
            {
                return;
            }

            if (selectedCowLabel != null)
            {
                var cowDisplay = GetCowDisplayName(sessionState.SelectedCowId);
                selectedCowLabel.text = string.IsNullOrWhiteSpace(sessionState.SelectedCowId)
                    ? IsBahasa() ? "Lembu Dipilih: Tiada" : "Selected Cow: None"
                    : IsBahasa() ? $"Lembu Dipilih: {cowDisplay}" : $"Selected Cow: {cowDisplay}";
            }

            if (scoreLabel != null)
            {
                scoreLabel.text = IsBahasa() ? $"Skor: {sessionState.TotalScore}" : $"Score: {sessionState.TotalScore}";
            }

            if (findingLabel != null)
            {
                var findingDisplay = GetFindingDisplayName(sessionState.SelectedFindingId);
                findingLabel.text = string.IsNullOrWhiteSpace(sessionState.SelectedFindingId)
                    ? IsBahasa() ? "Petunjuk: Belum ada" : "Finding: Pending"
                    : IsBahasa() ? $"Petunjuk: {findingDisplay}" : $"Finding: {findingDisplay}";
            }

            if (procedureLabel != null)
            {
                procedureLabel.text = sessionState.TotalProcedureSteps <= 0
                    ? IsBahasa() ? "Prosedur: Belum bermula" : "Procedure: Not started"
                    : sessionState.IsProcedureCompleted
                        ? IsBahasa()
                            ? $"Prosedur: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} langkah selesai"
                            : $"Procedure: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} steps complete"
                        : IsBahasa()
                            ? $"Prosedur: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} langkah"
                            : $"Procedure: {sessionState.CompletedProcedureSteps}/{sessionState.TotalProcedureSteps} steps";
            }

            if (correctLabel != null)
            {
                correctLabel.text = IsBahasa() ? $"Betul: {sessionState.CorrectAnswers}" : $"Correct: {sessionState.CorrectAnswers}";
            }

            if (wrongLabel != null)
            {
                wrongLabel.text = IsBahasa() ? $"Salah: {sessionState.WrongAnswers}" : $"Wrong: {sessionState.WrongAnswers}";
            }

            if (decisionLabel != null)
            {
                var decisionDisplay = GetDecisionDisplayName(sessionState.ValidationDecision);
                decisionLabel.text = string.IsNullOrWhiteSpace(sessionState.ValidationDecision)
                    ? IsBahasa() ? "Keputusan: Belum ada" : "Decision: Pending"
                    : IsBahasa() ? $"Keputusan: {decisionDisplay}" : $"Decision: {decisionDisplay}";
            }

            if (outcomeLabel != null)
            {
                outcomeLabel.text = IsBahasa()
                    ? $"Tahap: {GetOutcomeBand()}"
                    : $"Outcome: {GetOutcomeBand()}";
            }

            if (badgeLabel != null)
            {
                badgeLabel.text = IsBahasa()
                    ? $"Lencana: {GetBadgeTier()}"
                    : $"Badge: {GetBadgeTier()}";
            }

            if (recommendationLabel != null)
            {
                recommendationLabel.text = GetRecommendation();
            }

            if (timeLabel != null)
            {
                int minutes = Mathf.FloorToInt(sessionState.ElapsedTime / 60f);
                int seconds = Mathf.FloorToInt(sessionState.ElapsedTime % 60f);
                timeLabel.text = IsBahasa()
                    ? $"Masa: {minutes:00}:{seconds:00}"
                    : $"Time: {minutes:00}:{seconds:00}";
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

        private string GetOutcomeBand()
        {
            if (sessionState == null)
            {
                return IsBahasa() ? "Belum Dinilai" : "Not Assessed";
            }

            if (sessionState.TotalScore >= 60 && sessionState.WrongAnswers == 0 && sessionState.IsProcedureCompleted)
            {
                return IsBahasa() ? "Cemerlang" : "Excellent";
            }

            if (sessionState.TotalScore >= 35 && sessionState.IsProcedureCompleted)
            {
                return IsBahasa() ? "Kompeten" : "Competent";
            }

            return IsBahasa() ? "Perlu Latihan" : "Needs Practice";
        }

        private string GetBadgeTier()
        {
            if (sessionState == null)
            {
                return "-";
            }

            if (sessionState.TotalScore >= 60)
            {
                return IsBahasa() ? "Emas" : "Gold";
            }

            if (sessionState.TotalScore >= 35)
            {
                return IsBahasa() ? "Perak" : "Silver";
            }

            return IsBahasa() ? "Gangsa" : "Bronze";
        }

        private string GetRecommendation()
        {
            if (sessionState == null)
            {
                return string.Empty;
            }

            if (!sessionState.IsProcedureCompleted)
            {
                return IsBahasa()
                    ? "Lengkapkan semua langkah prosedur AI sebelum pengesahan akhir."
                    : "Complete all AI procedure steps before final validation.";
            }

            if (sessionState.TotalScore >= 60 && string.Equals(sessionState.ValidationDecision, "Bred"))
            {
                return IsBahasa()
                    ? "Prestasi sangat baik. Teruskan dengan kes yang lebih kompleks."
                    : "Strong performance. Advance to a more complex breeding case.";
            }

            if (sessionState.TotalScore >= 35)
            {
                return IsBahasa()
                    ? "Prestasi memadai. Ulangi latihan untuk meningkatkan ketepatan pengesanan."
                    : "Performance is acceptable. Repeat the training to improve detection accuracy.";
            }

            return IsBahasa()
                ? "Ulang semula modul pemilihan lembu, imbasan, dan prosedur sebelum penilaian seterusnya."
                : "Repeat the herd selection, scan, and procedure modules before the next assessment.";
        }
    }
}
