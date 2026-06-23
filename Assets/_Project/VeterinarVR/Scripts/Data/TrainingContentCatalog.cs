using System;
using UnityEngine;
using VeterinarVR.Core;

namespace VeterinarVR.Data
{
    [CreateAssetMenu(fileName = "TrainingContentCatalog", menuName = "Veterinar VR/Training Content Catalog")]
    public sealed class TrainingContentCatalog : ScriptableObject
    {
        private const string ResourcePath = "TrainingContentCatalog";

        [SerializeField] private string correctCowId = "Cow_B";
        [SerializeField] private string correctFindingId = "MucusDischarge";
        [SerializeField] private string bredDecisionId = "Bred";
        [SerializeField] private string notBredDecisionId = "Not Bred";
        [SerializeField] private int herdCorrectReward = 25;
        [SerializeField] private int herdIncorrectPenalty = 10;
        [SerializeField] private int scanReward = 10;
        [SerializeField] private int scanMistakePenalty = 5;
        [SerializeField] private int procedureCompletionReward = 15;
        [SerializeField] private int procedureSkipPenalty = 5;
        [SerializeField] private string[] procedureStepsEnglish =
        {
            "Prepare the insemination tool",
            "Position the insemination gun",
            "Deliver the dose"
        };

        [SerializeField] private string[] procedureStepsBahasa =
        {
            "Sediakan alat inseminasi",
            "Posisikan pistol inseminasi",
            "Hantar dos"
        };

        [SerializeField] private LocalizedIdEntry[] cows =
        {
            new("Cow_A", "Cow A", "Lembu A"),
            new("Cow_B", "Cow B", "Lembu B"),
            new("Cow_C", "Cow C", "Lembu C")
        };

        [SerializeField] private LocalizedIdEntry[] findings =
        {
            new("TailRaise", "Tail Raising", "Ekor Diangkat"),
            new("MucusDischarge", "Mucus Discharge", "Lelehan Mukus"),
            new("Restlessness", "Restlessness", "Gelisah")
        };

        [SerializeField] private LocalizedIdEntry[] decisions =
        {
            new("Bred", "Bred", "Bunting"),
            new("Not Bred", "Not Bred", "Tidak Bunting")
        };

        private static TrainingContentCatalog cachedInstance;

        public string CorrectCowId => correctCowId;
        public string CorrectFindingId => correctFindingId;
        public string BredDecisionId => bredDecisionId;
        public string NotBredDecisionId => notBredDecisionId;
        public int HerdCorrectReward => herdCorrectReward;
        public int HerdIncorrectPenalty => herdIncorrectPenalty;
        public int ScanReward => scanReward;
        public int ScanMistakePenalty => scanMistakePenalty;
        public int ProcedureCompletionReward => procedureCompletionReward;
        public int ProcedureSkipPenalty => procedureSkipPenalty;

        public static TrainingContentCatalog LoadDefault()
        {
            if (cachedInstance == null)
            {
                cachedInstance = Resources.Load<TrainingContentCatalog>(ResourcePath);
            }

            return cachedInstance;
        }

        public string[] GetProcedureSteps(SessionLanguage language)
        {
            return language == SessionLanguage.BahasaMelayu && procedureStepsBahasa.Length > 0
                ? procedureStepsBahasa
                : procedureStepsEnglish;
        }

        public string GetCowDisplayName(string cowId, SessionLanguage language)
        {
            return GetDisplayName(cows, cowId, language);
        }

        public string GetFindingDisplayName(string findingId, SessionLanguage language)
        {
            return GetDisplayName(findings, findingId, language);
        }

        public string GetDecisionDisplayName(string decisionId, SessionLanguage language)
        {
            return GetDisplayName(decisions, decisionId, language);
        }

        private static string GetDisplayName(LocalizedIdEntry[] entries, string id, SessionLanguage language)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return string.Empty;
            }

            if (entries != null)
            {
                for (var index = 0; index < entries.Length; index++)
                {
                    var entry = entries[index];
                    if (string.Equals(entry.Id, id, StringComparison.OrdinalIgnoreCase))
                    {
                        return language == SessionLanguage.BahasaMelayu && !string.IsNullOrWhiteSpace(entry.Bahasa)
                            ? entry.Bahasa
                            : !string.IsNullOrWhiteSpace(entry.English)
                                ? entry.English
                                : id;
                    }
                }
            }

            return id.Replace('_', ' ');
        }
    }

    [Serializable]
    public struct LocalizedIdEntry
    {
        [SerializeField] private string id;
        [SerializeField] private string english;
        [SerializeField] private string bahasa;

        public string Id => id;
        public string English => english;
        public string Bahasa => bahasa;

        public LocalizedIdEntry(string id, string english, string bahasa)
        {
            this.id = id;
            this.english = english;
            this.bahasa = bahasa;
        }
    }
}
