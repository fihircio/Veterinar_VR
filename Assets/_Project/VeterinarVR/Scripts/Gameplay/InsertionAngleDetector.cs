using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VeterinarVR.Core;

namespace VeterinarVR.Gameplay
{
    public sealed class InsertionAngleDetector : MonoBehaviour
    {
        [SerializeField] private AIProcedureController procedureController;
        [SerializeField] private float minAllowedAngle = 30f;
        [SerializeField] private float maxAllowedAngle = 60f;
        [SerializeField] private float hapticInterval = 0.5f;

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
        private bool isInsideVulvaZone;
        private float lastHapticTime;

        private void Awake()
        {
            interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
            if (procedureController == null)
            {
                procedureController = FindFirstObjectByType<AIProcedureController>();
            }
        }

        private void Update()
        {
            if (!isInsideVulvaZone || interactable == null || !interactable.isSelected)
            {
                return;
            }

            // Calculate angle between pistolette forward direction and the world horizontal plane.
            // transform.forward projected onto horizontal plane vs transform.forward.
            Vector3 forward = transform.forward;
            Vector3 horizontalForward = new Vector3(forward.x, 0f, forward.z).normalized;
            float angle = Vector3.Angle(horizontalForward, forward);

            // Since we want a 45 degree upward incline (mendongak ke atas),
            // check if the forward vector is pointing upwards.
            bool isPointingUp = forward.y > 0f;

            if (!isPointingUp || angle < minAllowedAngle || angle > maxAllowedAngle)
            {
                // Angle is flat or wrong. Trigger haptic warning on selecting interactor if enough time passed.
                if (Time.time - lastHapticTime >= hapticInterval)
                {
                    lastHapticTime = Time.time;
                    SendHapticWarning();
                    SpatialErrorLog.Record("ANGLE_FLAT", $"Pistolette angle was too flat or downward: {angle:F1}° (isPointingUp: {isPointingUp})");
                }
            }
            else
            {
                // Correct insertion angle detected.
                if (procedureController != null)
                {
                    procedureController.CompletePickupStep(); // Proceed to next phase/step
                }
                isInsideVulvaZone = false; // Trigger once per entry
            }
        }

        private void SendHapticWarning()
        {
            if (interactable == null) return;
            
            foreach (var interactor in interactable.interactorsSelecting)
            {
                if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor controllerInteractor)
                {
                    controllerInteractor.SendHapticImpulse(0.8f, 0.2f);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("VulvaEntryZone") || other.name.Contains("VulvaEntryZone"))
            {
                isInsideVulvaZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("VulvaEntryZone") || other.name.Contains("VulvaEntryZone"))
            {
                isInsideVulvaZone = false;
            }
        }
    }
}
