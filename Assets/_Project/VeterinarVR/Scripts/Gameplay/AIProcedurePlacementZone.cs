using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VeterinarVR.Gameplay
{
    public sealed class AIProcedurePlacementZone : MonoBehaviour
    {
        [SerializeField] private AIProcedureController procedureController;
        [SerializeField] private Transform snapPoint;
        [SerializeField] private Renderer zoneRenderer;
        [SerializeField] private Color idleColor = new(0.22f, 0.33f, 0.42f, 1f);
        [SerializeField] private Color activeColor = new(0.2f, 0.58f, 0.31f, 1f);
        [SerializeField] private Color completeColor = new(0.66f, 0.5f, 0.18f, 1f);

        private bool placementCompleted;
        private Material zoneMaterialInstance;

        private void Awake()
        {
            if (procedureController == null)
            {
                procedureController = FindAnyObjectByType<AIProcedureController>();
            }

            if (snapPoint == null)
            {
                var snapTransform = transform.Find("SnapPoint");
                if (snapTransform != null)
                {
                    snapPoint = snapTransform;
                }
            }

            if (zoneRenderer == null)
            {
                zoneRenderer = GetComponent<Renderer>();
            }

            if (zoneRenderer != null)
            {
                zoneMaterialInstance = zoneRenderer.material;
                zoneMaterialInstance.color = idleColor;
            }
        }

        private void Update()
        {
            if (zoneMaterialInstance == null)
            {
                return;
            }

            zoneMaterialInstance.color = placementCompleted
                ? completeColor
                : procedureController != null && procedureController.IsAwaitingPlacementInteraction
                    ? activeColor
                    : idleColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (placementCompleted || procedureController == null || !procedureController.IsAwaitingPlacementInteraction)
            {
                return;
            }

            var interactable = other.GetComponentInParent<XRGrabInteractable>();
            if (interactable == null)
            {
                return;
            }

            SnapTool(interactable);
            placementCompleted = true;
            procedureController.CompletePlacementStep();
        }

        private void SnapTool(XRGrabInteractable interactable)
        {
            var toolTransform = interactable.transform;
            if (snapPoint != null)
            {
                toolTransform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
            }

            var rigidBody = interactable.GetComponent<Rigidbody>();
            if (rigidBody != null)
            {
                rigidBody.isKinematic = true;
                rigidBody.useGravity = false;
            }

            interactable.enabled = false;
        }
    }
}
