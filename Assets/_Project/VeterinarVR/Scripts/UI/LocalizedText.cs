using UnityEngine;
using UnityEngine.UI;
using VeterinarVR.Core;

namespace VeterinarVR.UI
{
    [RequireComponent(typeof(Text))]
    public sealed class LocalizedText : MonoBehaviour
    {
        [SerializeField] private Text targetText;
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private string englishText = string.Empty;
        [SerializeField] private string bahasaText = string.Empty;

        private void Awake()
        {
            if (targetText == null)
            {
                targetText = GetComponent<Text>();
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
