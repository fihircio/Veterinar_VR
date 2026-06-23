using UnityEngine;

namespace VeterinarVR.Gameplay
{
    public sealed class AIProcedureToolTracker : MonoBehaviour
    {
        [SerializeField] private AIProcedureController procedureController;
        [SerializeField] private float pickupDistanceThreshold = 0.2f;

        private Vector3 startPosition;
        private bool pickupCompleted;

        private void Awake()
        {
            if (procedureController == null)
            {
                procedureController = FindFirstObjectByType<AIProcedureController>();
            }

            startPosition = transform.position;
        }

        private void Update()
        {
            if (pickupCompleted || procedureController == null || !procedureController.IsAwaitingToolPickupInteraction)
            {
                return;
            }

            if (Vector3.Distance(startPosition, transform.position) < pickupDistanceThreshold)
            {
                return;
            }

            pickupCompleted = true;
            procedureController.CompletePickupStep();
        }
    }
}
