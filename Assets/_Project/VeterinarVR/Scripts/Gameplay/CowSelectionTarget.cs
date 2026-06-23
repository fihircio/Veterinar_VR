using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace VeterinarVR.Gameplay
{
    public sealed class CowSelectionTarget : MonoBehaviour
    {
        [SerializeField] private HerdObservationController observationController;
        [SerializeField] private string cowId = "Cow_01";
        [SerializeField] private bool requireExplicitConfirmation = true;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private Color idleColor = new(0.62f, 0.56f, 0.46f, 1f);
        [SerializeField] private Color hoverColor = new(0.78f, 0.82f, 0.42f, 1f);
        [SerializeField] private Color selectedColor = new(0.2f, 0.68f, 0.34f, 1f);

        private XRSimpleInteractable simpleInteractable;
        private Material materialInstance;
        private bool isHovered;
        private bool isSelected;

        public string CowId => cowId;
        public bool RequireExplicitConfirmation => requireExplicitConfirmation;

        private void Awake()
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponentInChildren<Renderer>();
            }

            if (targetRenderer != null)
            {
                materialInstance = targetRenderer.material;
                materialInstance.color = idleColor;
            }

            simpleInteractable = GetComponent<XRSimpleInteractable>();
            if (simpleInteractable == null)
            {
                simpleInteractable = gameObject.AddComponent<XRSimpleInteractable>();
            }
        }

        private void OnEnable()
        {
            TryBindController();

            if (simpleInteractable != null)
            {
                simpleInteractable.hoverEntered.AddListener(OnHoverEntered);
                simpleInteractable.hoverExited.AddListener(OnHoverExited);
                simpleInteractable.selectEntered.AddListener(OnSelectEntered);
            }

            if (observationController != null)
            {
                observationController.CowSelectionChanged += HandleSelectionChanged;
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

            if (observationController != null)
            {
                observationController.CowSelectionChanged -= HandleSelectionChanged;
            }
        }

        public void PreviewSelection()
        {
            if (!requireExplicitConfirmation)
            {
                ConfirmSelection();
            }
        }

        public void ConfirmSelection()
        {
            TryBindController();

            if (observationController != null)
            {
                observationController.ResolveSelection(cowId);
            }
        }

        private void OnMouseDown()
        {
            ConfirmSelection();
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
            ConfirmSelection();
        }

        private void HandleSelectionChanged(string selectedCowId)
        {
            isSelected = string.Equals(selectedCowId, cowId, System.StringComparison.OrdinalIgnoreCase);
            RefreshVisual();
        }

        private void TryBindController()
        {
            if (observationController == null)
            {
                observationController = FindFirstObjectByType<HerdObservationController>();
            }
        }

        private void RefreshVisual()
        {
            if (materialInstance == null)
            {
                return;
            }

            materialInstance.color = isSelected
                ? selectedColor
                : isHovered
                    ? hoverColor
                    : idleColor;
        }
    }
}
