using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace VeterinarVR.XR
{
    public sealed class StreamojiTtsClient : MonoBehaviour
    {
        [Header("API Settings")]
        [SerializeField] private string baseUrl = "https://us-central1-streamoji-265f4.cloudfunctions.net";
        [SerializeField] private string getAuthTokenPath = "/getAuthToken";
        [SerializeField] private string avatarTtsPath = "/avatar_tts";

        [Header("Credentials")]
        [SerializeField] private string clientId = "default_client_id";
        [SerializeField] private string clientSecret = "default_client_secret";

        [Header("Audio Settings")]
        [SerializeField] private AudioType audioType = AudioType.MPEG;
        [SerializeField] private string defaultVoice = "en-US-Standard-A";

        // Cached authorization token
        private string _cachedToken = string.Empty;
        private bool _isAuthenticating = false;

        public string ClientId
        {
            get => clientId;
            set => clientId = value;
        }

        public string ClientSecret
        {
            get => clientSecret;
            set => clientSecret = value;
        }

        /// <summary>
        /// Requests generated speech audio for the given text.
        /// </summary>
        /// <param name="text">The text to speak.</param>
        /// <param name="onSuccess">Callback with the downloaded AudioClip.</param>
        /// <param name="onError">Callback with the error message.</param>
        public void RequestSpeech(string text, Action<AudioClip> onSuccess, Action<string> onError)
        {
            StartCoroutine(RequestSpeechRoutine(text, onSuccess, onError));
        }

        private IEnumerator RequestSpeechRoutine(string text, Action<AudioClip> onSuccess, Action<string> onError)
        {
            // 1. Ensure authenticated
            if (string.IsNullOrEmpty(_cachedToken))
            {
                yield return StartCoroutine(AuthenticateRoutine(
                    token => { _cachedToken = token; },
                    err => { onError?.Invoke($"Authentication failed: {err}"); }
                ));

                if (string.IsNullOrEmpty(_cachedToken))
                {
                    yield break;
                }
            }

            // 2. Request TTS from avatar_tts endpoint
            string ttsUrl = baseUrl + avatarTtsPath;
            var requestData = new TtsRequest { text = text, voice = defaultVoice };
            string jsonPayload = JsonUtility.ToJson(requestData);

            using (UnityWebRequest request = new UnityWebRequest(ttsUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {_cachedToken}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    // If unauthorized, token might have expired. Clear token and retry once.
                    if (request.responseCode == 401)
                    {
                        Debug.LogWarning("Streamoji Token expired/unauthorized. Retrying authentication...");
                        _cachedToken = string.Empty;
                        yield return StartCoroutine(RequestSpeechRoutine(text, onSuccess, onError));
                    }
                    else
                    {
                        onError?.Invoke($"TTS Request failed: {request.error} (Code: {request.responseCode})");
                    }
                    yield break;
                }

                // 3. Process TTS response
                string contentType = request.GetResponseHeader("Content-Type");
                if (!string.IsNullOrEmpty(contentType) && contentType.Contains("application/json"))
                {
                    // Response is a JSON containing the URL to download the audio clip
                    string responseText = request.downloadHandler.text;
                    TtsResponse ttsResponse = null;
                    try
                    {
                        ttsResponse = JsonUtility.FromJson<TtsResponse>(responseText);
                    }
                    catch (Exception ex)
                    {
                        onError?.Invoke($"Failed to parse TTS JSON response: {ex.Message}");
                        yield break;
                    }

                    string audioUrl = !string.IsNullOrEmpty(ttsResponse.audioUrl) ? ttsResponse.audioUrl : ttsResponse.url;
                    if (string.IsNullOrEmpty(audioUrl))
                    {
                        onError?.Invoke("TTS Response JSON did not contain a valid audio URL.");
                        yield break;
                    }

                    yield return StartCoroutine(DownloadAudioClipRoutine(audioUrl, onSuccess, onError));
                }
                else
                {
                    // Response is the binary audio file itself
                    // We need to fetch it from the buffer using DownloadHandlerAudioClip or recreate request
                    yield return StartCoroutine(DownloadAudioClipFromBufferRoutine(request, ttsUrl, onSuccess, onError));
                }
            }
        }

        private IEnumerator AuthenticateRoutine(Action<string> onAuthSuccess, Action<string> onAuthError)
        {
            while (_isAuthenticating)
            {
                yield return null;
            }

            if (!string.IsNullOrEmpty(_cachedToken))
            {
                onAuthSuccess?.Invoke(_cachedToken);
                yield break;
            }

            _isAuthenticating = true;
            string authUrl = baseUrl + getAuthTokenPath;
            var requestData = new TokenRequest { clientId = clientId, clientSecret = clientSecret };
            string jsonPayload = JsonUtility.ToJson(requestData);

            using (UnityWebRequest request = new UnityWebRequest(authUrl, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                _isAuthenticating = false;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onAuthError?.Invoke($"HTTP Error: {request.error} (Code: {request.responseCode})");
                    yield break;
                }

                string responseText = request.downloadHandler.text;
                TokenResponse tokenResponse = null;
                try
                {
                    tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);
                }
                catch (Exception ex)
                {
                    onAuthError?.Invoke($"JSON Parse Error: {ex.Message}");
                    yield break;
                }

                string token = !string.IsNullOrEmpty(tokenResponse.token) ? tokenResponse.token : tokenResponse.accessToken;
                if (string.IsNullOrEmpty(token))
                {
                    onAuthError?.Invoke("Response did not contain a token.");
                    yield break;
                }

                onAuthSuccess?.Invoke(token);
            }
        }

        private IEnumerator DownloadAudioClipRoutine(string url, Action<AudioClip> onSuccess, Action<string> onError)
        {
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"Failed to download audio clip from URL: {request.error}");
                    yield break;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip == null)
                {
                    onError?.Invoke("Downloaded clip is null.");
                }
                else
                {
                    onSuccess?.Invoke(clip);
                }
            }
        }

        private IEnumerator DownloadAudioClipFromBufferRoutine(UnityWebRequest originalRequest, string url, Action<AudioClip> onSuccess, Action<string> onError)
        {
            // Unity's DownloadHandlerAudioClip works best when we use the specialized UnityWebRequestMultimedia.GetAudioClip.
            // Since originalRequest used DownloadHandlerBuffer, we re-fetch or construct the clip.
            // Re-fetching with GetAudioClip using the same headers / token is safest and cleanest in Unity.
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, audioType))
            {
                request.SetRequestHeader("Authorization", $"Bearer {_cachedToken}");
                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"Failed to download audio clip directly: {request.error}");
                    yield break;
                }

                AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                if (clip == null)
                {
                    onError?.Invoke("Downloaded clip content is null.");
                }
                else
                {
                    onSuccess?.Invoke(clip);
                }
            }
        }

        #region Serialized Helper Classes
        [Serializable]
        private class TokenRequest
        {
            public string clientId;
            public string clientSecret;
        }

        [Serializable]
        private class TokenResponse
        {
            public string token;
            public string accessToken;
        }

        [Serializable]
        private class TtsRequest
        {
            public string text;
            public string voice;
        }

        [Serializable]
        private class TtsResponse
        {
            public string audioUrl;
            public string url;
        }
        #endregion
    }
}
