using System.Reflection;
using UnityEngine;
using Unity.XR.CoreUtils;

namespace VeterinarVR.Gameplay
{
    public sealed class HandTrackingSetup : MonoBehaviour
    {
        [SerializeField] private GameObject leftHandRoot;
        [SerializeField] private GameObject rightHandRoot;

        private static readonly FieldInfo SkeletonTypeField =
            typeof(OVRSkeleton).GetField("_skeletonType", BindingFlags.Instance | BindingFlags.NonPublic);
        private static readonly FieldInfo HandTypeField =
            typeof(OVRHand).GetField("HandType", BindingFlags.Instance | BindingFlags.NonPublic);

        private void Start()
        {
            if (leftHandRoot == null || rightHandRoot == null)
            {
                CreateHandObjects();
            }
        }

        private void CreateHandObjects()
        {
            var xrOrigin = GetComponentInParent<XROrigin>();
            if (xrOrigin == null)
            {
                var camera = GetComponentInChildren<Camera>();
                if (camera != null)
                {
                    xrOrigin = camera.GetComponentInParent<XROrigin>();
                }
            }

            if (xrOrigin == null)
            {
                Debug.LogWarning("HandTrackingSetup: Could not find XROrigin in parent hierarchy.");
                return;
            }

            leftHandRoot = new GameObject("LeftHand");
            leftHandRoot.transform.SetParent(xrOrigin.transform, false);

            rightHandRoot = new GameObject("RightHand");
            rightHandRoot.transform.SetParent(xrOrigin.transform, false);

            SetupHand(leftHandRoot, OVRHand.Hand.HandLeft, OVRSkeleton.SkeletonType.HandLeft);
            SetupHand(rightHandRoot, OVRHand.Hand.HandRight, OVRSkeleton.SkeletonType.HandRight);
        }

        private static void SetupHand(GameObject handObject, OVRHand.Hand handType, OVRSkeleton.SkeletonType skeletonType)
        {
            var hand = handObject.AddComponent<OVRHand>();
            if (HandTypeField != null)
            {
                HandTypeField.SetValue(hand, handType);
            }

            var skeleton = handObject.AddComponent<OVRSkeleton>();
            if (SkeletonTypeField != null)
            {
                SkeletonTypeField.SetValue(skeleton, skeletonType);
            }

            handObject.AddComponent<OVRMesh>();
            handObject.AddComponent<OVRMeshRenderer>();
        }
    }
}
