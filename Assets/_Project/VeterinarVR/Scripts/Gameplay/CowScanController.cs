using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Audio;
using VeterinarVR.Core;

namespace VeterinarVR.Gameplay
{
    public sealed class CowScanController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private Text selectedCowLabel;
        [SerializeField] private Text statusLabel;
        [SerializeField] private Text feedbackLabel;
        [SerializeField] private GameObject proceedButtonRoot;
        [SerializeField] private Text measurementLabel;
        [SerializeField] private GameObject vulvaRulerObject;

        public bool HasCompletedScan { get; private set; }
        public string ActiveCowId { get; private set; } = string.Empty;

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
        }

        private void Start()
        {
            ActiveCowId = sessionState != null ? sessionState.SelectedCowId : string.Empty;
            RefreshView();
        }

        public void OnVulvaMeasured(float sizeCm)
        {
            HasCompletedScan = true;
            AudioRuntimeDirector.PlayCowAccent();

            if (measurementLabel != null)
            {
                measurementLabel.text = IsBahasa()
                    ? $"Saiz Vulva Direkodkan: {sizeCm} cm"
                    : $"Vulva Size Recorded: {sizeCm} cm";
            }

            RefreshView(IsBahasa()
                ? $"Ukuran {sizeCm} cm telah disimpan."
                : $"Measurement of {sizeCm} cm recorded.");
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
                    ? IsBahasa() ? "Ukuran selesai" : "Measurement complete"
                    : IsBahasa() ? "Cubit ibu jari & jari telunjuk berhampiran vulva untuk mengukur" : "Pinch thumb & index finger near the vulva to measure";
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
            if (cowId.Contains("A"))
            {
                return IsBahasa() ? "Lembu A" : "Cow A";
            }
            else if (cowId.Contains("B"))
            {
                return IsBahasa() ? "Lembu B" : "Cow B";
            }
            else if (cowId.Contains("C"))
            {
                return IsBahasa() ? "Lembu C" : "Cow C";
            }

            return cowId.Replace('_', ' ');
        }
    }
}
