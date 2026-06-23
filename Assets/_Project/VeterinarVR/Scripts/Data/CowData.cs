using System.Collections.Generic;
using UnityEngine;

namespace VeterinarVR.Data
{
    [CreateAssetMenu(
        fileName = "CowData",
        menuName = "Veterinar VR/Data/Cow",
        order = 11)]
    public sealed class CowData : ScriptableObject
    {
        [SerializeField] private string cowId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private string herdGroup = string.Empty;
        [SerializeField] private string notes = string.Empty;
        [SerializeField] private QuestionData[] questionSet = new QuestionData[0];

        public string CowId => cowId;
        public string DisplayName => displayName;
        public string HerdGroup => herdGroup;
        public string Notes => notes;
        public IReadOnlyList<QuestionData> QuestionSet => questionSet;
    }
}
