using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VeterinarVR.Gameplay
{
    public sealed class ScanFindingTarget : MonoBehaviour
    {
        [SerializeField] private string findingId = string.Empty;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private SphereCollider targetCollider;
        [SerializeField] private Color idleColor = new(0.24f, 0.38f, 0.62f, 1f);
        [SerializeField] private Color hoverColor = new(0.8f, 0.78f, 0.28f, 1f);
        [SerializeField] private float colliderPadding = 1.35f;

        private XRSimpleInteractable simpleInteractable;
        private Material materialInstance;
        private bool isHovered;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetCollider == null)
            {
                targetCollider = GetComponent<SphereCollider>();
            }

            EnsureRuntimeMaterial();
            RefreshCollider();

            simpleInteractable = GetComponent<XRSimpleInteractable>();
            if (simpleInteractable == null)
            {
                simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            }
        }

        private void OnValidate()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetCollider == null)
            {
                targetCollider = GetComponent<SphereCollider>();
            }

            RefreshCollider();
        }

        private void Start()
        {
            EnsureRuntimeMaterial();
            RefreshCollider();
            RefreshVisual();
        }

        private void EnsureRuntimeMaterial()
        {
            if (targetRenderer != null)
            {
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
        }

        private void OnEnable()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
                simpleInteractable.hoverExited.AddListener(OnHoverExited);
                simpleInteractable.selectEntered.AddListener(OnSelectEntered);
            }
        }

        private void OnDisable()
        {
            if (simpleInteractable != null)
            {
                simpleInteractable.hoverEntered.RemoveListener(OnHoverEntered);
                simpleInteractable.hoverExited.RemoveListener(OnHoverExited);
                simpleInteractable.selectEntered.RemoveListener(OnSelectEntered);
            }
        }

        private void OnMouseDown()
        {
            ConfirmFinding();
        }

        private void OnMouseEnter()
        {
            isHovered = true;
            RefreshVisual();
        }

        private void OnMouseExit()
        {
            isHovered = false;
            RefreshVisual();
        }

        public void ConfirmFinding()
        {
            Debug.Log($"ScanFindingTarget '{findingId}' selected.");
        }

        private void OnHoverEntered(HoverEnterEventArgs _)
        {
            isHovered = true;
            RefreshVisual();
        }

        private void OnHoverExited(HoverExitEventArgs _)
        {
            isHovered = false;
            RefreshVisual();
        }

        private void OnSelectEntered(SelectEnterEventArgs _)
        {
            ConfirmFinding();
        }

        private void RefreshCollider()
        {
            if (targetCollider == null || targetRenderer == null)
            {
                return;
            }

            var extents = targetRenderer.localBounds.extents;
            var maxExtent = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z));
            if (maxExtent <= 0f)
            {
                maxExtent = 0.5f;
            }

            targetCollider.center = targetRenderer.localBounds.center;
            targetCollider.radius = Mathf.Max(0.5f, maxExtent * colliderPadding);
        }

        private void RefreshVisual()
        {
            if (materialInstance == null)
            {
                return;
            }

            materialInstance.color = isHovered ? hoverColor : idleColor;
        }
    }
}
