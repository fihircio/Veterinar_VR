using System;
using System.Collections.Generic;
using UnityEngine;
using VeterinarVR.Core;

namespace VeterinarVR.Gameplay
{
    public static class SpatialErrorLog
    {
        public struct ErrorEntry
        {
            public float Timestamp;
            public string ErrorCode;
            public string Detail;

            public override string ToString()
            {
                return $"[{Timestamp:F1}s] {ErrorCode}: {Detail}";
            }
        }

        private static readonly List<ErrorEntry> entries = new List<ErrorEntry>();
        public static IReadOnlyList<ErrorEntry> Entries => entries;

        public static event Action<ErrorEntry> ErrorRecorded;

        public static void Record(string errorCode, string detail)
        {
            float time = 0f;
            var session = TrainingSessionState.Instance;
            if (session != null)
            {
                time = session.ElapsedTime;
                session.IncrementSpatialErrors();
            }

            var entry = new ErrorEntry
            {
                Timestamp = time,
                ErrorCode = errorCode,
                Detail = detail
            };

            entries.Add(entry);
            Debug.LogWarning($"[SPATIAL_ERROR] {entry}");
            ErrorRecorded?.Invoke(entry);
        }

        public static void Clear()
        {
            entries.Clear();
        }
    }
}
