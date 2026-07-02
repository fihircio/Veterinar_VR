using System.Collections;
using UnityEngine;
using VeterinarVR.Audio;

namespace VeterinarVR.Gameplay
{
    /// <summary>
    /// Manages the URS Fasa 1 (Thawing) and Fasa 2 (Equipment Assembly) sub-steps
    /// within S04_AIProcedure.
    ///
    /// Place this on a manager GameObject in the S04 scene.
    /// Wire it to the AIProcedureController and configure the sub-step references.
    ///
    /// Sub-step sequence:
    ///   Thawing:   1. Straw grabbed from N2 tank  → 2. Straw soaked in beaker (timer) → 3. Straw cut
    ///   Assembly:  4. Straw inserted into pistolette → 5. AI Sheath attached → 6. PD Glove equipped
    /// </summary>
    public sealed class ThawingStationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AIProcedureController procedureController;

        [Header("Thawing Settings")]
        [SerializeField] private float thawingDurationSeconds = 5f;
        [SerializeField] private UnityEngine.UI.Text thawingTimerLabel;
        [SerializeField] private GameObject nitrogenTankSnapZone;
        [SerializeField] private GameObject warmWaterBeaker;

        [Header("Assembly Settings")]
        [SerializeField] private GameObject pistoletteStrawtSnapZone;
        [SerializeField] private GameObject pistoletteAiSheathSnapZone;
        [SerializeField] private GameObject pdGloveVisual;   // SkinnedMeshRenderer on left controller hand

        public enum ThawingSubStep
        {
            WaitingForStrawGrab,
            WaitingForThaw,
            WaitingForStrawCut,
            WaitingForStrawInsert,
            WaitingForAiSheath,
            WaitingForGlove,
            Complete
        }

        public ThawingSubStep CurrentSubStep { get; private set; } = ThawingSubStep.WaitingForStrawGrab;

        private Coroutine thawTimerCoroutine;
        private float thawElapsed;

        private void Awake()
        {
            if (procedureController == null)
            {
                procedureController = FindFirstObjectByType<AIProcedureController>();
            }
        }

        private void Start()
        {
            if (pdGloveVisual != null) pdGloveVisual.SetActive(false);
        }

        // ─── Called by XR Grab Interactable events on the Straw prop ─────────────

        /// <summary>Call from the Straw XR Grab Interactable OnSelectEntered event.</summary>
        public void OnStrawGrabbedFromTank()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForStrawGrab) return;
            CurrentSubStep = ThawingSubStep.WaitingForThaw;
            AudioRuntimeDirector.PlayUiClick();
            Debug.Log("[ThawingStation] Straw grabbed from N2 tank. Soak now.");
        }

        /// <summary>Call from the WarmWaterBeaker OnTriggerEnter (straw enters beaker).</summary>
        public void OnStrawEnteredBeaker()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForThaw) return;
            if (thawTimerCoroutine != null) StopCoroutine(thawTimerCoroutine);
            thawTimerCoroutine = StartCoroutine(ThawTimerRoutine());
        }

        private IEnumerator ThawTimerRoutine()
        {
            thawElapsed = 0f;
            while (thawElapsed < thawingDurationSeconds)
            {
                thawElapsed += Time.deltaTime;
                float remaining = thawingDurationSeconds - thawElapsed;
                if (thawingTimerLabel != null)
                {
                    thawingTimerLabel.text = $"35–37°C  {remaining:F1}s";
                }
                yield return null;
            }

            if (thawingTimerLabel != null) thawingTimerLabel.text = "Thawing complete!";
            CurrentSubStep = ThawingSubStep.WaitingForStrawCut;
            AudioRuntimeDirector.PlayUiConfirm();
            Debug.Log("[ThawingStation] Thawing complete. Cut the straw now.");
        }

        /// <summary>Call from StrawCutter XR interaction (trigger press while holding straw cutter).</summary>
        public void OnStrawCut()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForStrawCut) return;
            CurrentSubStep = ThawingSubStep.WaitingForStrawInsert;
            AudioRuntimeDirector.PlayUiClick();
            Debug.Log("[ThawingStation] Straw cut. Insert into pistolette.");
        }

        // ─── Assembly steps ───────────────────────────────────────────────────────

        /// <summary>Call from Pistolette SnapZone OnSelectEntered (straw snapped in).</summary>
        public void OnStrawInsertedIntoPistolette()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForStrawInsert) return;
            CurrentSubStep = ThawingSubStep.WaitingForAiSheath;
            AudioRuntimeDirector.PlayUiClick();
            Debug.Log("[ThawingStation] Straw inserted. Attach AI Sheath.");
        }

        /// <summary>Call from AI Sheath SnapZone OnSelectEntered.</summary>
        public void OnAiSheathAttached()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForAiSheath) return;
            CurrentSubStep = ThawingSubStep.WaitingForGlove;
            AudioRuntimeDirector.PlayUiClick();
            Debug.Log("[ThawingStation] AI Sheath attached. Put on PD Glove.");
        }

        /// <summary>Call from XR input (left Grip held while in assembly phase).</summary>
        public void OnPdGloveEquipped()
        {
            if (CurrentSubStep != ThawingSubStep.WaitingForGlove) return;

            if (pdGloveVisual != null) pdGloveVisual.SetActive(true);

            CurrentSubStep = ThawingSubStep.Complete;
            AudioRuntimeDirector.PlayUiConfirm();
            Debug.Log("[ThawingStation] PD Glove equipped. Thawing & Assembly phases complete.");

            // Advance the procedure through both Thawing and Assembly phases
            procedureController?.AdvancePhase(true); // ThawingPrep → EquipmentAssembly
            procedureController?.AdvancePhase(true); // EquipmentAssembly → InsertionLogic
        }
    }
}
