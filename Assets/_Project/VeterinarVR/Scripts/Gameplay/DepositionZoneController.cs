using UnityEngine;
using VeterinarVR.Audio;

namespace VeterinarVR.Gameplay
{
    /// <summary>
    /// URS Fasa 4: Validasi Pelepasan (Deposition Validation).
    ///
    /// Attach this script to the pistolette GameObject.
    /// Set up three BoxCollider trigger volumes in the scene:
    ///   - CervicalDepositZone   (correct target)
    ///   - UterineTooDeepZone   (too deep — past last cervical ring)
    ///   - VulvaEntryZone       (managed by InsertionAngleDetector)
    ///
    /// Call TryReleaseDeposition() when the player presses the release trigger action.
    /// </summary>
    public sealed class DepositionZoneController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AIProcedureController procedureController;

        [Header("Zone Tags or Names")]
        [SerializeField] private string cervicalZoneTag = "CervicalDepositZone";
        [SerializeField] private string tooDeepZoneTag = "UterineTooDeepZone";

        private bool isInCervicalZone;
        private bool isInTooDeepZone;
        private bool hasReleased;

        private void Awake()
        {
            if (procedureController == null)
            {
                procedureController = FindFirstObjectByType<AIProcedureController>();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(cervicalZoneTag) || other.name.Contains(cervicalZoneTag))
            {
                isInCervicalZone = true;
            }
            else if (other.CompareTag(tooDeepZoneTag) || other.name.Contains(tooDeepZoneTag))
            {
                isInTooDeepZone = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(cervicalZoneTag) || other.name.Contains(cervicalZoneTag))
            {
                isInCervicalZone = false;
            }
            else if (other.CompareTag(tooDeepZoneTag) || other.name.Contains(tooDeepZoneTag))
            {
                isInTooDeepZone = false;
            }
        }

        /// <summary>
        /// Call this from the XR controller input (trigger press) when the player
        /// intends to release the semen dose.
        /// </summary>
        public void TryReleaseDeposition()
        {
            if (hasReleased) return;
            if (procedureController == null) return;
            if (procedureController.CurrentPhase != AIProcedurePhase.DepositionValidation) return;

            hasReleased = true;

            if (isInTooDeepZone)
            {
                // Released too deep into uterine horn
                SpatialErrorLog.Record("RELEASE_TOO_DEEP",
                    "Semen released past the last cervical ring into the uterine horn.");
                AudioRuntimeDirector.PlayUiClick();
                Debug.LogWarning("[DepositionZone] RELEASE_TOO_DEEP — semen past last cervical ring.");
                procedureController.AdvancePhase(false); // penalised
            }
            else if (isInCervicalZone)
            {
                // Correct deposit — within cervical canal or ≤1 cm past last ring
                AudioRuntimeDirector.PlayUiConfirm();
                Debug.Log("[DepositionZone] Correct deposition — cervical zone confirmed.");
                procedureController.AdvancePhase(true); // rewarded
            }
            else
            {
                // Released before reaching the cervix
                SpatialErrorLog.Record("RELEASE_TOO_EARLY",
                    "Semen released before passing through the cervical rings.");
                AudioRuntimeDirector.PlayUiClick();
                Debug.LogWarning("[DepositionZone] RELEASE_TOO_EARLY — pistolette not in cervical zone.");
                procedureController.AdvancePhase(false); // penalised
            }
        }

        public void ResetDeposition()
        {
            hasReleased = false;
            isInCervicalZone = false;
            isInTooDeepZone = false;
        }
    }
}
