using UnityEngine;

namespace VeterinarVR.XR
{
    public sealed class XRBootstrapNotes : MonoBehaviour
    {
        [SerializeField] private bool logOnAwake = true;
        [SerializeField] private string xrSmokeTestSceneName = "S00_XRSmokeTest";
        [SerializeField] private string expectedRigName = "XR Origin (VR)";

        public string XRSmokeTestSceneName => xrSmokeTestSceneName;
        public string ExpectedRigName => expectedRigName;

        private void Awake()
        {
            if (logOnAwake)
            {
                Debug.Log($"XRBootstrapNotes expects scene '{xrSmokeTestSceneName}' with rig '{expectedRigName}'.", this);
            }
        }

        public bool IsConfiguredForScene(string sceneName)
        {
            return !string.IsNullOrWhiteSpace(sceneName) && sceneName == xrSmokeTestSceneName;
        }
    }
}
