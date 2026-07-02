using System.Collections;
using TMPro;
using UnityEngine;

namespace VeterinarVR.XR
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class GuideNarrator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private Animator animator;
        
        [Header("Subtitles")]
        [SerializeField] private TMP_Text subtitleText;
        [SerializeField] private GameObject subtitlePanel; // Optional panel to toggle visibility

        [Header("Animation Settings")]
        [SerializeField] private string talkTriggerName = "Talk";
        [SerializeField] private string idleStateName = "Idle";
        [SerializeField] private string talkStateName = "Talk";
        [SerializeField] private float charactersPerSecondFallback = 15f; // Used if clip is null

        private Coroutine _speakCoroutine;

        private void Awake()
        {
            if (audioSource == null)
            {
                audioSource = GetComponent<AudioSource>();
            }

            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }

            // Hide subtitles by default on awake
            SetSubtitleActive(false);
        }

        /// <summary>
        /// Instructs the narrator avatar to speak the given text and play the corresponding audio clip.
        /// </summary>
        /// <param name="text">The text content to speak and display as subtitles.</param>
        /// <param name="clip">Optional audio clip to play.</param>
        public void Speak(string text, AudioClip clip = null)
        {
            if (_speakCoroutine != null)
            {
                StopCoroutine(_speakCoroutine);
            }

            _speakCoroutine = StartCoroutine(SpeakRoutine(text, clip));
        }

        private IEnumerator SpeakRoutine(string text, AudioClip clip)
        {
            // 1. Show subtitles
            if (subtitleText != null)
            {
                subtitleText.text = text;
            }
            SetSubtitleActive(true);

            // 2. Determine duration
            float duration = 0f;
            if (clip != null)
            {
                duration = clip.length;
                if (audioSource != null)
                {
                    audioSource.clip = clip;
                    audioSource.Play();
                }
            }
            else
            {
                // Fallback duration based on text length
                duration = Mathf.Max(1.5f, text.Length / charactersPerSecondFallback);
            }

            // 3. Trigger talking animation
            if (animator != null)
            {
                // Set the Talk trigger to initiate transition
                animator.SetTrigger(talkTriggerName);
            }

            // 4. Wait for audio/duration to complete
            float elapsed = 0f;
            while (elapsed < duration)
            {
                // If audio finished early, break (but only if we actually had a clip playing)
                if (clip != null && audioSource != null && !audioSource.isPlaying && elapsed > 0.5f)
                {
                    break;
                }

                // If animator transitions away from talk state too early but audio is still playing,
                // we can re-trigger or crossfade to keep the talk animation looping.
                if (animator != null && clip != null && elapsed + 0.5f < duration)
                {
                    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName(idleStateName) && !animator.IsInTransition(0))
                    {
                        animator.SetTrigger(talkTriggerName);
                    }
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 5. Speak finished - stop audio, return to idle, hide subtitles
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }

            if (animator != null)
            {
                animator.ResetTrigger(talkTriggerName);
                animator.CrossFadeInFixedTime(idleStateName, 0.2f);
            }

            SetSubtitleActive(false);
            _speakCoroutine = null;
        }

        private void SetSubtitleActive(bool active)
        {
            if (subtitlePanel != null)
            {
                subtitlePanel.SetActive(active);
            }
            else if (subtitleText != null)
            {
                subtitleText.gameObject.SetActive(active);
            }
        }

        private void OnDisable()
        {
            if (_speakCoroutine != null)
            {
                StopCoroutine(_speakCoroutine);
                _speakCoroutine = null;
            }
            
            if (audioSource != null && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
