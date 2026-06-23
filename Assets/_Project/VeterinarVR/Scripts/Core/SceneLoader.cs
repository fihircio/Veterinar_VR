using UnityEngine;
using UnityEngine.SceneManagement;

namespace VeterinarVR.Core
{
    public sealed class SceneLoader : MonoBehaviour
    {
        [SerializeField] private bool logSceneChanges = true;

        public void LoadScene(string sceneName)
        {
            if (string.IsNullOrWhiteSpace(sceneName))
            {
                Debug.LogWarning("SceneLoader received an empty scene name.");
                return;
            }

            if (logSceneChanges)
            {
                Debug.Log($"Loading scene '{sceneName}'.", this);
            }

            SceneManager.LoadScene(sceneName);
        }

        public void LoadSmokeTest()
        {
            LoadScene(SceneIds.XRSmokeTest);
        }

        public void LoadGreeting()
        {
            LoadScene(SceneIds.Greeting);
        }

        public void LoadHerdObservation()
        {
            LoadScene(SceneIds.HerdObservation);
        }

        public void LoadCowScanDecision()
        {
            LoadScene(SceneIds.CowScanDecision);
        }

        public void LoadAIProcedure()
        {
            LoadScene(SceneIds.AIProcedure);
        }

        public void LoadValidationDashboard()
        {
            LoadScene(SceneIds.ValidationDashboard);
        }

        public void LoadResults()
        {
            LoadScene(SceneIds.ResultsScoreboard);
        }
    }
}
