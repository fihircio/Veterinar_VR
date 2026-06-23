using System;
using System.Collections.Generic;
using UnityEngine;

namespace VeterinarVR.Data
{
    [CreateAssetMenu(
        fileName = "QuestionData",
        menuName = "Veterinar VR/Data/Question",
        order = 10)]
    public sealed class QuestionData : ScriptableObject
    {
        [SerializeField] private string questionId = string.Empty;
        [SerializeField] private string prompt = string.Empty;
        [SerializeField] private string[] answerOptions = Array.Empty<string>();
        [SerializeField] private int correctAnswerIndex;
        [SerializeField] private int scoreValue = 10;

        public string QuestionId => questionId;
        public string Prompt => prompt;
        public IReadOnlyList<string> AnswerOptions => answerOptions;
        public int CorrectAnswerIndex => correctAnswerIndex;
        public int ScoreValue => scoreValue;

        public bool IsValidAnswerIndex(int index)
        {
            return index >= 0 && index < answerOptions.Length;
        }
    }
}
