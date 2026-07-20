using UnityEngine;

namespace VeterinarVR.Core
{
    public sealed class TrainingSessionState : MonoBehaviour
    {
        public static TrainingSessionState Instance { get; private set; }

        [field: SerializeField]
        public SessionLanguage SelectedLanguage { get; private set; } = SessionLanguage.English;

        [field: SerializeField]
        public string SelectedCowId { get; private set; } = string.Empty;

        [field: SerializeField]
        public int TotalScore { get; private set; }

        [field: SerializeField]
        public int CorrectAnswers { get; private set; }

        [field: SerializeField]
        public int WrongAnswers { get; private set; }

        [field: SerializeField]
        public string ValidationDecision { get; private set; } = string.Empty;

        [field: SerializeField]
        public string SelectedFindingId { get; private set; } = string.Empty;

        [field: SerializeField]
        public int CompletedProcedureSteps { get; private set; }

        [field: SerializeField]
        public int TotalProcedureSteps { get; private set; }

        [field: SerializeField]
        public bool IsProcedureCompleted { get; private set; }

        public bool HasLanguageSelection { get; private set; }

        [field: SerializeField]
        public float ElapsedTime { get; private set; }

        [field: SerializeField]
        public string SelectedSemenType { get; private set; } = string.Empty;

        [field: SerializeField]
        public int SpatialErrorCount { get; private set; }

        [field: SerializeField]
        public float MeasuredVulvaSize { get; private set; }

        [field: SerializeField]
        public float MeasuredMucusDischarge { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (HasLanguageSelection && !IsProcedureCompleted)
            {
                ElapsedTime += Time.deltaTime;
            }
        }

        public void SetLanguage(SessionLanguage language)
        {
            SelectedLanguage = language;
            HasLanguageSelection = true;
        }

        public void SelectCow(string cowId)
        {
            SelectedCowId = cowId ?? string.Empty;
        }

        public void AddScore(int amount, bool answeredCorrectly)
        {
            TotalScore += amount;

            if (answeredCorrectly)
            {
                CorrectAnswers++;
            }
            else
            {
                WrongAnswers++;
            }
        }

        public void ApplyScoreDelta(int scoreDelta, int correctDelta, int wrongDelta)
        {
            TotalScore += scoreDelta;
            CorrectAnswers = Mathf.Max(0, CorrectAnswers + correctDelta);
            WrongAnswers = Mathf.Max(0, WrongAnswers + wrongDelta);
        }

        public void SetValidationDecision(string decision)
        {
            ValidationDecision = decision ?? string.Empty;
        }

        public void SetScanFinding(string findingId)
        {
            SelectedFindingId = findingId ?? string.Empty;
        }

        public void SetProcedureProgress(int completedSteps, int totalSteps, bool isComplete)
        {
            CompletedProcedureSteps = Mathf.Max(0, completedSteps);
            TotalProcedureSteps = Mathf.Max(0, totalSteps);
            IsProcedureCompleted = isComplete;
        }

        public void SelectSemen(string semenType)
        {
            SelectedSemenType = semenType ?? string.Empty;
        }

        public void SetMeasuredVulvaSize(float sizeCm)
        {
            MeasuredVulvaSize = Mathf.Max(0f, sizeCm);
        }

        public void SetMeasuredMucusDischarge(float sizeCm)
        {
            MeasuredMucusDischarge = Mathf.Max(0f, sizeCm);
        }

        public void IncrementSpatialErrors()
        {
            SpatialErrorCount++;
        }

        public void ResetSession()
        {
            SelectedLanguage = SessionLanguage.English;
            SelectedCowId = string.Empty;
            TotalScore = 0;
            CorrectAnswers = 0;
            WrongAnswers = 0;
            ValidationDecision = string.Empty;
            SelectedFindingId = string.Empty;
            CompletedProcedureSteps = 0;
            TotalProcedureSteps = 0;
            IsProcedureCompleted = false;
            HasLanguageSelection = false;
            ElapsedTime = 0f;
            SelectedSemenType = string.Empty;
            SpatialErrorCount = 0;
            MeasuredVulvaSize = 0f;
            MeasuredMucusDischarge = 0f;
        }
    }
}
