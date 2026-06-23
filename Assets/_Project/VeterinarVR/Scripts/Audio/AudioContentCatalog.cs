using System;
using UnityEngine;

namespace VeterinarVR.Audio
{
    [CreateAssetMenu(fileName = "AudioContentCatalog", menuName = "Veterinar VR/Audio Content Catalog")]
    public sealed class AudioContentCatalog : ScriptableObject
    {
        [Header("Global UI")]
        [SerializeField] private AudioClip uiClick;
        [SerializeField] private AudioClip uiConfirm;

        [Header("Farm Ambience")]
        [SerializeField] private AudioClip outdoorLoop;
        [SerializeField] private AudioClip indoorLoop;

        [Header("Cow Vocals")]
        [SerializeField] private AudioClip[] cowCalls = Array.Empty<AudioClip>();

        [Header("Bird Accents")]
        [SerializeField] private AudioClip[] birdCalls = Array.Empty<AudioClip>();

        [Header("Procedure Foley")]
        [SerializeField] private AudioClip[] grainHandling = Array.Empty<AudioClip>();

        [Header("Results Accent")]
        [SerializeField] private AudioClip resultCue;

        public AudioClip UiClick => uiClick;
        public AudioClip UiConfirm => uiConfirm;
        public AudioClip OutdoorLoop => outdoorLoop;
        public AudioClip IndoorLoop => indoorLoop;
        public AudioClip[] CowCalls => cowCalls;
        public AudioClip[] BirdCalls => birdCalls;
        public AudioClip[] GrainHandling => grainHandling;
        public AudioClip ResultCue => resultCue;
    }
}
