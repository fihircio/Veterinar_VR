using UnityEngine;

namespace VeterinarVR.Core
{
    public sealed class PlaceholderSceneNote : MonoBehaviour
    {
        [SerializeField] private string label = "Placeholder Scene";

        public string Label => label;

        public void SetLabel(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                label = value.Trim();
            }
        }
    }
}
