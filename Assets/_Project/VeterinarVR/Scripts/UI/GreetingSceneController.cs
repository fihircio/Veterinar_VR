using UnityEngine;
using VeterinarVR.Audio;
using VeterinarVR.Core;

namespace VeterinarVR.UI
{
    public sealed class GreetingSceneController : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField] private GameObject proceedButtonRoot;
        [SerializeField] private GameObject englishSelectedState;
        [SerializeField] private GameObject bahasaSelectedState;
        [SerializeField] private bool resetSessionOnSceneEnter = true;

        public SessionLanguage SelectedLanguage => sessionState != null
            ? sessionState.SelectedLanguage
            : SessionLanguage.English;

        private void Awake()
        {
            if (sessionState == null)
            {
                sessionState = TrainingSessionState.Instance ?? FindFirstObjectByType<TrainingSessionState>();
            }

            if (sceneLoader == null)
            {
                sceneLoader = FindFirstObjectByType<SceneLoader>();
            }
        }

        private void Start()
        {
            if (sessionState != null && resetSessionOnSceneEnter)
            {
                sessionState.ResetSession();
            }

            RefreshView();
        }

        public void SelectEnglish()
        {
            SetLanguage(SessionLanguage.English);
        }

        public void SelectBahasaMelayu()
        {
            SetLanguage(SessionLanguage.BahasaMelayu);
        }

        public void ProceedToScene02()
        {
            if (sessionState == null || !sessionState.HasLanguageSelection)
            {
                Debug.LogWarning("Proceed blocked until a language is selected.", this);
                return;
            }

            if (sceneLoader == null)
            {
                Debug.LogWarning("GreetingSceneController could not find SceneLoader.", this);
                return;
            }

            AudioRuntimeDirector.PlayUiConfirm();
            sceneLoader.LoadHerdObservation();
        }

        private void SetLanguage(SessionLanguage language)
        {
            if (sessionState == null)
            {
                Debug.LogWarning("GreetingSceneController could not find TrainingSessionState.", this);
                return;
            }

            sessionState.SetLanguage(language);
            AudioRuntimeDirector.PlayUiClick();
            RefreshView();
        }

        private void RefreshView()
        {
            var hasSelection = sessionState != null && sessionState.HasLanguageSelection;
            var isEnglish = sessionState != null && sessionState.SelectedLanguage == SessionLanguage.English;
            var isBahasa = sessionState != null && sessionState.SelectedLanguage == SessionLanguage.BahasaMelayu;

            if (proceedButtonRoot != null)
            {
                proceedButtonRoot.SetActive(hasSelection);
            }

            if (englishSelectedState != null)
            {
                englishSelectedState.SetActive(hasSelection && isEnglish);
            }

            if (bahasaSelectedState != null)
            {
                bahasaSelectedState.SetActive(hasSelection && isBahasa);
            }
        }
    }
}
