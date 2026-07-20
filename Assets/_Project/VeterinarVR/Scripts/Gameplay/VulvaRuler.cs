using UnityEngine;
using VeterinarVR.Core;

namespace VeterinarVR.Gameplay
{
    public sealed class VulvaRuler : MonoBehaviour
    {
        [SerializeField] private OVRSkeleton leftHandSkeleton;
        [SerializeField] private OVRSkeleton rightHandSkeleton;
        [SerializeField] private OVRHand leftHand;
        [SerializeField] private OVRHand rightHand;
        [SerializeField] private Transform vulvaCenter;
        [SerializeField] private float activationRadius = 0.25f;
        [SerializeField] private float lockHoldDuration = 1.5f;

        [Header("Ruler Visual")]
        [SerializeField] private GameObject rulerLinePrefab;
        [SerializeField] private LineRenderer rulerLine;

        [Header("Events")]
        [SerializeField] private CowScanController scanController;

        private bool isMeasuring;
        private bool isPinching;
        private float pinchTimer;
        private float currentMeasurement;
        private GameObject rulerInstance;

        public float CurrentMeasurementCm => currentMeasurement;
        public bool IsMeasurementActive => isMeasuring || isPinching;

        private void Awake()
        {
            if (scanController == null)
            {
                scanController = FindFirstObjectByType<CowScanController>();
            }

            if (vulvaCenter == null)
            {
                vulvaCenter = transform;
            }
        }

        private void Start()
        {
            if (rulerLine == null && rulerLinePrefab != null)
            {
                rulerInstance = Instantiate(rulerLinePrefab, transform);
                rulerLine = rulerInstance.GetComponent<LineRenderer>();
            }

            if (rulerLine == null)
            {
                rulerLine = gameObject.AddComponent<LineRenderer>();
                rulerLine.useWorldSpace = true;
                rulerLine.positionCount = 2;
                rulerLine.startWidth = 0.002f;
                rulerLine.endWidth = 0.002f;
                rulerLine.material = new Material(Shader.Find("Sprites/Default"));
                rulerLine.startColor = new Color(0f, 1f, 0.6f, 1f);
                rulerLine.endColor = new Color(0f, 1f, 0.6f, 1f);
            }

            rulerLine.enabled = false;
        }

        private void Update()
        {
            ResolveSkeletonReferences();

            bool leftTracked = leftHand != null && leftHand.IsTracked;
            bool rightTracked = rightHand != null && rightHand.IsTracked;

            if (!leftTracked || !rightTracked)
            {
                ResetRuler();
                return;
            }

            var leftIndexTip = GetBonePosition(leftHandSkeleton, OVRSkeleton.BoneId.Hand_IndexTip);
            var leftThumbTip = GetBonePosition(leftHandSkeleton, OVRSkeleton.BoneId.Hand_ThumbTip);

            bool leftIndexPinching = leftHand.GetFingerIsPinching(OVRHand.HandFinger.Index);

            if (leftIndexPinching)
            {
                isPinching = true;
                isMeasuring = false;

                float pinchToVulva = vulvaCenter != null
                    ? Vector3.Distance(leftIndexTip, vulvaCenter.position)
                    : float.MaxValue;

                if (pinchToVulva < activationRadius)
                {
                    isMeasuring = true;

                    rulerLine.SetPosition(0, leftIndexTip);
                    rulerLine.SetPosition(1, leftThumbTip);
                    rulerLine.enabled = true;

                    float distanceM = Vector3.Distance(leftIndexTip, leftThumbTip);
                    float distanceCm = distanceM * 100f;
                    currentMeasurement = Mathf.Round(distanceCm * 10f) / 10f;
                }
                else
                {
                    rulerLine.enabled = false;
                }
            }
            else
            {
                if (isPinching)
                {
                    rulerLine.enabled = false;
                    isPinching = false;
                    isMeasuring = false;
                    pinchTimer = 0f;

                    if (currentMeasurement > 0f)
                    {
                        LockMeasurement();
                    }
                }
            }

            if (isMeasuring)
            {
                pinchTimer += Time.deltaTime;
                if (pinchTimer >= lockHoldDuration)
                {
                    LockMeasurement();
                    pinchTimer = 0f;
                }
            }
        }

        private void LockMeasurement()
        {
            if (currentMeasurement <= 0f)
            {
                return;
            }

            var sessionState = TrainingSessionState.Instance;
            if (sessionState != null)
            {
                sessionState.SetMeasuredVulvaSize(currentMeasurement);
            }

            pinchTimer = 0f;
            isMeasuring = false;
            isPinching = false;
            rulerLine.enabled = false;

            if (scanController != null)
            {
                scanController.OnVulvaMeasured(currentMeasurement);
            }

            Debug.Log($"Vulva measurement locked: {currentMeasurement} cm");
        }

        private void ResetRuler()
        {
            isMeasuring = false;
            isPinching = false;
            pinchTimer = 0f;
            currentMeasurement = 0f;
            rulerLine.enabled = false;
        }

        private Vector3 GetBonePosition(OVRSkeleton skeleton, OVRSkeleton.BoneId boneId)
        {
            if (skeleton == null || skeleton.Bones == null)
            {
                return Vector3.zero;
            }

            foreach (var bone in skeleton.Bones)
            {
                if (bone.Id == boneId)
                {
                    return bone.Transform != null ? bone.Transform.position : Vector3.zero;
                }
            }

            return Vector3.zero;
        }

        private void ResolveSkeletonReferences()
        {
            if (leftHandSkeleton == null || rightHandSkeleton == null || leftHand == null || rightHand == null)
            {
                var hands = FindObjectsByType<OVRHand>(FindObjectsSortMode.None);
                foreach (var hand in hands)
                {
                    var handTypeField = typeof(OVRHand).GetField("HandType",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                    if (handTypeField == null) continue;

                    var handType = (OVRHand.Hand)handTypeField.GetValue(hand);

                    if (handType == OVRHand.Hand.HandLeft)
                    {
                        leftHand = hand;
                        leftHandSkeleton = hand.GetComponent<OVRSkeleton>();
                    }
                    else if (handType == OVRHand.Hand.HandRight)
                    {
                        rightHand = hand;
                        rightHandSkeleton = hand.GetComponent<OVRSkeleton>();
                    }
                }
            }
        }
    }
}
