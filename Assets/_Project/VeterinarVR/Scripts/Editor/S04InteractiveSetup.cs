using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VeterinarVR.Gameplay;
using VeterinarVR.UI;

namespace VeterinarVR.Editor
{
    public static class S04InteractiveSetup
    {
        private const string ScenePath = "Assets/_Project/VeterinarVR/Scenes/S04_AIProcedure.unity";

        [MenuItem("Veterinar VR/Bootstrap/Setup S04 Interactivity")]
        public static void Setup()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var env = GameObject.Find("AIProcedureEnvironment")?.transform;
            if (env == null)
            {
                Debug.LogError("AIProcedureEnvironment not found in scene!");
                return;
            }

            Task1_MoveInteractivityToPistolette(env);
            Task2_AddTooltipLabels(env);
            Task3_AddTriggerZoneMeshes(env);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log("S04 Interactive setup complete!");
        }

        private static void Task1_MoveInteractivityToPistolette(Transform env)
        {
            var cgTrader = env.Find("CGTraderInseminiation");
            var aiTool = env.Find("AIProcedureTool");
            var pistolette = cgTrader?.Find("Pistolette");

            if (aiTool == null) { Debug.LogWarning("AIProcedureTool not found, skipping Task 1"); return; }
            if (pistolette == null) { Debug.LogWarning("Pistolette not found under CGTraderInseminiation, skipping Task 1"); return; }

            var pistoGo = pistolette.gameObject;

            Undo.RegisterCompleteObjectUndo(pistoGo, "Setup Pistolette interactivity");
            Undo.RegisterCompleteObjectUndo(aiTool.gameObject, "Disable old AIProcedureTool");

            // Rigidbody
            if (pistoGo.GetComponent<Rigidbody>() == null)
            {
                var rb = Undo.AddComponent<Rigidbody>(pistoGo);
                rb.mass = 0.25f;
                rb.linearDamping = 0.35f;
                rb.angularDamping = 0.8f;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }

            // Collider if none exists yet
            if (pistoGo.GetComponent<Collider>() == null)
            {
                Undo.AddComponent<CapsuleCollider>(pistoGo);
            }

            // XRGrabInteractable
            if (pistoGo.GetComponent<XRGrabInteractable>() == null)
            {
                var grab = Undo.AddComponent<XRGrabInteractable>(pistoGo);
                grab.throwOnDetach = true;
                grab.throwVelocityScale = 1.5f;
                grab.throwAngularVelocityScale = 1f;
                grab.trackPosition = true;
                grab.trackRotation = true;
                grab.matchAttachPosition = true;
                grab.matchAttachRotation = true;
                grab.snapToColliderVolume = true;
                grab.reinitializeDynamicAttachEverySingleGrab = true;
                grab.unparentTransformOnGrab = true;
                grab.retainTransformParent = true;
                grab.addDefaultGrabTransformers = true;
                grab.throwSmoothingDuration = 0.25f;
                // Set movement type via serialized property (avoids XRI API version issues)
                var grabSO = new SerializedObject(grab);
                var mt = grabSO.FindProperty("m_MovementType");
                if (mt != null) mt.enumValueIndex = 2; // VelocityTracking
                grabSO.ApplyModifiedProperties();
            }

            // AIProcedureToolTracker
            if (pistoGo.GetComponent<AIProcedureToolTracker>() == null)
            {
                var oldTracker = aiTool.GetComponent<AIProcedureToolTracker>();
                var tracker = Undo.AddComponent<AIProcedureToolTracker>(pistoGo);
                if (oldTracker != null)
                {
                    var trackerSO = new SerializedObject(tracker);
                    var oldSO = new SerializedObject(oldTracker);
                    var dProp = trackerSO.FindProperty("pickupDistanceThreshold");
                    var oldDProp = oldSO.FindProperty("pickupDistanceThreshold");
                    if (dProp != null && oldDProp != null)
                    {
                        dProp.floatValue = oldDProp.floatValue;
                        trackerSO.ApplyModifiedProperties();
                    }
                }
            }

            // AutoHand Grabbable (if available)
            var grabbableType = System.Type.GetType("Autohand.Grabbable, AutoHandAssembly");
            if (grabbableType != null)
            {
                var oldGrabbable = aiTool.GetComponent(grabbableType);
                if (oldGrabbable != null && pistoGo.GetComponent(grabbableType) == null)
                {
                    var newGrabbable = pistoGo.AddComponent(grabbableType);
                    EditorUtility.CopySerialized(oldGrabbable, newGrabbable);
                    // Fix the m_GameObject reference that CopySerialized overrode
                    var fixSo = new SerializedObject(newGrabbable);
                    fixSo.FindProperty("m_GameObject").objectReferenceValue = pistoGo;
                    fixSo.ApplyModifiedProperties();
                }
            }

            // Reparent ToolTip child
            var toolTip = aiTool.Find("ToolTip");
            if (toolTip != null)
            {
                toolTip.SetParent(pistolette, worldPositionStays: true);
            }

            // Disable old AIProcedureTool
            aiTool.gameObject.SetActive(false);
        }

        private static void Task2_AddTooltipLabels(Transform env)
        {
            var cgTrader = env.Find("CGTraderInseminiation");
            if (cgTrader == null) { Debug.LogWarning("CGTraderInseminiation not found, skipping Task 2"); return; }

            var tooltipData = new Dictionary<string, (string en, string ms)>
            {
                { "StrawCutter", ("Straw Cutter", "Pemotong Straw") },
                { "InnerCanister", ("Inner Canister", "Kanister Dalam") },
                { "Pistolette", ("Pistolette", "Pistolet") },
                { "Forceps", ("Forceps", "Forsep") },
                { "ThawingThermosFlask", ("Thawing Flask", "Kelalang Cair") },
                { "NitrogenTank", ("Nitrogen Tank", "Tangki Nitrogen") },
            };

            var fontPath = AssetDatabase.GUIDToAssetPath("8f586378b4e144a9851e7b34d9b748ee");
            var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);

            foreach (var child in cgTrader)
            {
                var childTransform = child as Transform;
                if (childTransform == null) continue;
                if (!tooltipData.TryGetValue(childTransform.name, out var texts)) continue;

                var existingLabel = childTransform.Find("Label");
                if (existingLabel != null) continue;

                var labelGo = new GameObject("Label");
                Undo.RegisterCreatedObjectUndo(labelGo, "Create tooltip label");
                labelGo.transform.SetParent(childTransform, false);

                var rect = labelGo.AddComponent<RectTransform>();
                rect.localPosition = new Vector3(0, 0.3f, 0);
                rect.localRotation = Quaternion.identity;
                rect.localScale = new Vector3(0.12f, 0.12f, 0.12f);
                rect.sizeDelta = new Vector2(260, 60);
                rect.anchorMin = new Vector2(0.5f, 0.5f);
                rect.anchorMax = new Vector2(0.5f, 0.5f);
                rect.pivot = new Vector2(0.5f, 0.5f);

                var tmp = labelGo.AddComponent<TMPro.TextMeshPro>();
                tmp.text = texts.en;
                tmp.font = font != null ? font : tmp.font;
                tmp.fontSize = 22;
                tmp.fontStyle = TMPro.FontStyles.Bold;
                tmp.alignment = TMPro.TextAlignmentOptions.Center;
                tmp.color = Color.white;
                tmp.raycastTarget = false;

                var localized = labelGo.AddComponent<LocalizedTextMeshPro>();
                var locSO = new SerializedObject(localized);
                locSO.FindProperty("targetText").objectReferenceValue = tmp;
                locSO.FindProperty("englishText").stringValue = texts.en;
                locSO.FindProperty("bahasaText").stringValue = texts.ms;
                locSO.ApplyModifiedProperties();
            }
        }

        private static void Task3_AddTriggerZoneMeshes(Transform env)
        {
            var zoneNames = new[] { "VulvaEntryZone", "CervicalDepositZone", "UterineTooDeepZone" };
            var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLitShader == null) { Debug.LogWarning("URP Lit shader not found, skipping Task 3"); return; }

            var matPath = AssetDatabase.GUIDToAssetPath("31321ba15b8f8eb4c954353edc038b1d");
            var baseMat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (baseMat == null)
            {
                baseMat = new Material(urpLitShader);
                baseMat.color = new Color(0.22f, 0.33f, 0.42f, 0.5f);
            }

            foreach (var zoneName in zoneNames)
            {
                var zone = env.Find(zoneName);
                if (zone == null) { Debug.LogWarning($"{zoneName} not found, skipping"); continue; }

                var zoneGo = zone.gameObject;
                Undo.RegisterCompleteObjectUndo(zoneGo, $"Setup {zoneName}");

                // MeshFilter (cube)
                if (zoneGo.GetComponent<MeshFilter>() == null)
                {
                    var temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    var cubeMesh = temp.GetComponent<MeshFilter>().sharedMesh;
                    Object.DestroyImmediate(temp);

                    var mf = Undo.AddComponent<MeshFilter>(zoneGo);
                    mf.sharedMesh = cubeMesh;
                }

                // MeshRenderer with semi-transparent material
                if (zoneGo.GetComponent<MeshRenderer>() == null)
                {
                    var mr = Undo.AddComponent<MeshRenderer>(zoneGo);
                    mr.sharedMaterial = baseMat;
                    mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }

                // AIProcedurePlacementZone
                if (zoneGo.GetComponent<AIProcedurePlacementZone>() == null)
                {
                    Undo.AddComponent<AIProcedurePlacementZone>(zoneGo);
                }

                // Ensure BoxCollider is a trigger
                var bc = zoneGo.GetComponent<BoxCollider>();
                if (bc != null)
                {
                    bc.isTrigger = true;
                }
            }
        }
    }
}
