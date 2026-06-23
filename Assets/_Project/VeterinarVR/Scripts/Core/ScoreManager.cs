using UnityEngine;

namespace VeterinarVR.Core
{
    public sealed class ScoreManager : MonoBehaviour
    {
        [SerializeField] private TrainingSessionState sessionState;

        private void Awake()
        {
            if (sessionState == null)
            {
                sessionState = FindFirstObjectByType<TrainingSessionState>();
            }
        }

        public void AwardPoints(int amount)
        {
            if (sessionState == null)
            {
                Debug.LogWarning("ScoreManager could not find TrainingSessionState.");
                return;
            }

            sessionState.AddScore(amount, true);
        }

        public void RegisterMistake(int penalty = 0)
        {
            if (sessionState == null)
            {
                Debug.LogWarning("ScoreManager could not find TrainingSessionState.");
                return;
            }

            sessionState.AddScore(-Mathf.Abs(penalty), false);
        }
    }
}
