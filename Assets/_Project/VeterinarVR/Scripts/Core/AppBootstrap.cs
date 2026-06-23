using UnityEngine;
using VeterinarVR.Audio;

namespace VeterinarVR.Core
{
    public sealed class AppBootstrap : MonoBehaviour
    {
        [SerializeField] private bool createMissingManagers = true;
        [SerializeField] private TrainingSessionState sessionStatePrefab;
        [SerializeField] private ScoreManager scoreManagerPrefab;

        private void Awake()
        {
            if (!createMissingManagers)
            {
                return;
            }

            EnsureSessionState();
            EnsureScoreManager();
            EnsureAudioDirector();
        }

        private void EnsureSessionState()
        {
            if (TrainingSessionState.Instance != null)
            {
                return;
            }

            if (sessionStatePrefab != null)
            {
                Instantiate(sessionStatePrefab);
                return;
            }

            var sessionRoot = new GameObject(nameof(TrainingSessionState));
            sessionRoot.AddComponent<TrainingSessionState>();
        }

        private void EnsureScoreManager()
        {
            if (FindFirstObjectByType<ScoreManager>() != null)
            {
                return;
            }

            if (scoreManagerPrefab != null)
            {
                Instantiate(scoreManagerPrefab);
                return;
            }

            var scoreRoot = new GameObject(nameof(ScoreManager));
            scoreRoot.AddComponent<ScoreManager>();
        }

        private void EnsureAudioDirector()
        {
            AudioRuntimeDirector.EnsureExists();
        }
    }
}
