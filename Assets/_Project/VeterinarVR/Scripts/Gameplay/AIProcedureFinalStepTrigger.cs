using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VeterinarVR.Gameplay
{
    public sealed class AIProcedureFinalStepTrigger : MonoBehaviour
    {
        [SerializeField] private AIProcedureController procedureController;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private CapsuleCollider targetCollider;
        [SerializeField] private Color idleColor = new(0.2f, 0.24f, 0.3f, 1f);
        [SerializeField] private Color activeColor = new(0.74f, 0.3f, 0.22f, 1f);
        [SerializeField] private Color completeColor = new(0.22f, 0.58f, 0.31f, 1f);
        [SerializeField] private float minimumClickableHeight = 1.25f;
        [SerializeField] private float minimumClickableRadius = 0.45f;

        private bool stepCompleted;
        private Material materialInstance;
        private XRSimpleInteractable interactable;

        private void Awake()
        {
            if (procedureController == null)
            {
                procedureController = FindFirstObjectByType<AIProcedureController>();
            }

            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }

            if (targetCollider == null)
            {
                targetCollider = GetComponent<CapsuleCollider>();
            }

            EnsureRuntimeMaterial();
            RefreshCollider();

            interactable = GetComponent<XRSimpleInteractable>();
            if (interactable == null)
            {
                interactable = gameObject.AddComponent<XRSimpleInteractable>();
            }

            interactable.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnValidate()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }

            if (targetCollider == null)
            {
                targetCollider = GetComponent<CapsuleCollider>();
            }

            RefreshCollider();
        }

        private void Start()
        {
            EnsureRuntimeMaterial();
            RefreshCollider();
        }

        private void OnDestroy()
        {
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener(OnSelectEntered);
            }
        }

        private void EnsureRuntimeMaterial()
        {
            if (targetRenderer == null)
            {
                return;
            }

            var shader = Shader.Find("Universal Render Pipeline/Lit") ??
                Shader.Find("Standard") ??
                Shader.Find("Sprites/Default");

            if (shader != null)
            {
                materialInstance = new Material(shader)
                {
                    color = idleColor
                };

                targetRenderer.sharedMaterial = materialInstance;
            }
            else
            {
                materialInstance = targetRenderer.material;
                materialInstance.color = idleColor;
            }
        }

        private void RefreshCollider()
        {
            if (targetCollider == null)
            {
                return;
            }

            targetCollider.height = Mathf.Max(targetCollider.height, minimumClickableHeight);
            targetCollider.radius = Mathf.Max(targetCollider.radius, minimumClickableRadius);
            targetCollider.center = new Vector3(0f, targetCollider.height * 0.5f, 0f);
        }

        private void Update()
        {
            if (materialInstance == null)
            {
                return;
            }

            materialInstance.color = stepCompleted
                ? completeColor
                : procedureController != null && procedureController.IsAwaitingFinalCheckpointInteraction
                    ? activeColor
                    : idleColor;
        }

        private void OnMouseDown()
        {
            TryCompleteFinalStep();
        }

        private void OnSelectEntered(SelectEnterEventArgs _)
        {
            TryCompleteFinalStep();
        }

        private void TryCompleteFinalStep()
        {
            if (stepCompleted || procedureController == null || !procedureController.IsAwaitingFinalCheckpointInteraction)
            {
                return;
            }

            stepCompleted = true;
            procedureController.CompleteFinalCheckpointStep();
        }
    }
}
