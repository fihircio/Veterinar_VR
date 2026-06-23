using TMPro;
using UnityEngine;
using VeterinarVR.Core;

namespace VeterinarVR.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public sealed class LocalizedTextMeshPro : MonoBehaviour
    {
        [SerializeField] private TMP_Text targetText;
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private string englishText = string.Empty;
        [SerializeField] private string bahasaText = string.Empty;

        private void Awake()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }

            if (sessionState == null)
            {
                sessionState = TrainingSessionState.Instance ?? FindFirstObjectByType<TrainingSessionState>();
            }
        }

        private void OnEnable()
        {
            Apply();
        }

        public void Apply()
        {
            if (targetText == null)
            {
                return;
            }

            var isBahasa = sessionState != null && sessionState.SelectedLanguage == SessionLanguage.BahasaMelayu;
            targetText.text = isBahasa && !string.IsNullOrWhiteSpace(bahasaText)
                ? bahasaText
                : englishText;
        }
    }
}
