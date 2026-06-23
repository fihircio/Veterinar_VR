using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using VeterinarVR.Core;

namespace VeterinarVR.Audio
{
    public sealed class AudioRuntimeDirector : MonoBehaviour
    {
        private const string ResourcePath = "AudioContentCatalog";

        public static AudioRuntimeDirector Instance { get; private set; }

        [SerializeField] private AudioContentCatalog catalog;
        [SerializeField] private float ambienceVolume = 0.14f;
        [SerializeField] private float accentVolume = 0.28f;
        [SerializeField] private float uiVolume = 0.55f;

        private AudioSource ambienceSource;
        private AudioSource oneShotSource;
        private Coroutine sceneAccentRoutine;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (catalog == null)
            {
                catalog = Resources.Load<AudioContentCatalog>(ResourcePath);
            }

            ambienceSource = CreateSource("AmbienceSource", true, false, ambienceVolume);
            oneShotSource = CreateSource("OneShotSource", false, false, uiVolume);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += HandleSceneLoaded;
        }

        private void Start()
        {
            HandleSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= HandleSceneLoaded;
        }

        public static void EnsureExists()
        {
            if (Instance != null)
            {
                return;
            }

            var root = new GameObject(nameof(AudioRuntimeDirector));
            root.AddComponent<AudioRuntimeDirector>();
        }

        public static void PlayUiClick()
        {
            Instance?.PlayOneShot(Instance.catalog != null ? Instance.catalog.UiClick : null, Instance.uiVolume);
        }

        public static void PlayUiConfirm()
        {
            Instance?.PlayOneShot(Instance.catalog != null ? Instance.catalog.UiConfirm : null, Instance.uiVolume);
        }

        public static void PlayCowAccent()
        {
            if (Instance == null || Instance.catalog == null || Instance.catalog.CowCalls.Length == 0)
            {
                return;
            }

            var clips = Instance.catalog.CowCalls;
            Instance.PlayOneShot(clips[Random.Range(0, clips.Length)], Instance.accentVolume);
        }

        public static void PlayProcedureFoley()
        {
            if (Instance == null || Instance.catalog == null || Instance.catalog.GrainHandling.Length == 0)
            {
                return;
            }

            var clips = Instance.catalog.GrainHandling;
            Instance.PlayOneShot(clips[Random.Range(0, clips.Length)], Instance.accentVolume * 0.9f);
        }

        public static void PlayBirdAccent()
        {
            if (Instance == null || Instance.catalog == null || Instance.catalog.BirdCalls.Length == 0)
            {
                return;
            }

            var clips = Instance.catalog.BirdCalls;
            Instance.PlayOneShot(clips[Random.Range(0, clips.Length)], Instance.accentVolume * 0.8f);
        }

        public static void PlayResultCue()
        {
            Instance?.PlayOneShot(Instance.catalog != null ? Instance.catalog.ResultCue : null, Instance.accentVolume * 1.05f);
        }

        private void HandleSceneLoaded(Scene scene, LoadSceneMode _)
        {
            if (catalog == null)
            {
                catalog = Resources.Load<AudioContentCatalog>(ResourcePath);
            }

            ConfigureSceneAudio(scene.name);
        }

        private void ConfigureSceneAudio(string sceneName)
        {
            if (sceneAccentRoutine != null)
            {
                StopCoroutine(sceneAccentRoutine);
                sceneAccentRoutine = null;
            }

            ambienceSource.Stop();
            ambienceSource.clip = null;

            if (catalog == null)
            {
                return;
            }

            switch (sceneName)
            {
                case SceneIds.Greeting:
                    StartAmbience(catalog.OutdoorLoop, ambienceVolume * 0.9f);
                    sceneAccentRoutine = StartCoroutine(PlayOutdoorLife(5f, 11f, 11f, 18f));
                    break;

                case SceneIds.HerdObservation:
                    StartAmbience(catalog.OutdoorLoop, ambienceVolume);
                    sceneAccentRoutine = StartCoroutine(PlayOutdoorLife(6f, 12f, 10f, 18f));
                    break;

                case SceneIds.CowScanDecision:
                    StartAmbience(catalog.OutdoorLoop, ambienceVolume * 0.88f);
                    sceneAccentRoutine = StartCoroutine(PlayOutdoorLife(10f, 18f, 14f, 24f));
                    break;

                case SceneIds.AIProcedure:
                    StartAmbience(catalog.IndoorLoop != null ? catalog.IndoorLoop : catalog.OutdoorLoop, ambienceVolume * 0.55f);
                    sceneAccentRoutine = StartCoroutine(PlayPeriodicCowAccents(18f, 28f));
                    break;

                case SceneIds.ValidationDashboard:
                    StartAmbience(catalog.IndoorLoop != null ? catalog.IndoorLoop : catalog.OutdoorLoop, ambienceVolume * 0.7f);
                    break;

                case SceneIds.ResultsScoreboard:
                    StartAmbience(catalog.IndoorLoop != null ? catalog.IndoorLoop : catalog.OutdoorLoop, ambienceVolume * 0.72f);
                    PlayResultCue();
                    break;
            }
        }

        private void StartAmbience(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            ambienceSource.clip = clip;
            ambienceSource.volume = volume;
            ambienceSource.Play();
        }

        private IEnumerator PlayPeriodicCowAccents(float minDelay, float maxDelay)
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
                PlayCowAccent();
            }
        }

        private IEnumerator PlayOutdoorLife(float minBirdDelay, float maxBirdDelay, float minCowDelay, float maxCowDelay)
        {
            var nextBirdDelay = Random.Range(minBirdDelay, maxBirdDelay);
            var nextCowDelay = Random.Range(minCowDelay, maxCowDelay);

            while (true)
            {
                yield return null;

                nextBirdDelay -= Time.unscaledDeltaTime;
                nextCowDelay -= Time.unscaledDeltaTime;

                if (nextBirdDelay <= 0f)
                {
                    PlayBirdAccent();
                    nextBirdDelay = Random.Range(minBirdDelay, maxBirdDelay);
                }

                if (nextCowDelay <= 0f)
                {
                    PlayCowAccent();
                    nextCowDelay = Random.Range(minCowDelay, maxCowDelay);
                }
            }
        }

        private void PlayOneShot(AudioClip clip, float volume)
        {
            if (clip == null || oneShotSource == null)
            {
                return;
            }

            oneShotSource.PlayOneShot(clip, volume);
        }

        private AudioSource CreateSource(string name, bool loop, bool playOnAwake, float volume)
        {
            var child = new GameObject(name);
            child.transform.SetParent(transform, false);
            var source = child.AddComponent<AudioSource>();
            source.loop = loop;
            source.playOnAwake = playOnAwake;
            source.spatialBlend = 0f;
            source.volume = volume;
            return source;
        }
    }
}
