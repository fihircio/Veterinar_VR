using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using VeterinarVR.Gameplay;

namespace VeterinarVR.Editor
{
    public static class S04ProcedurePipelineBuilder
    {
        private const string AIProcedureScenePath = "Assets/_Project/VeterinarVR/Scenes/S04_AIProcedure.unity";

        [MenuItem("Veterinar VR/Bootstrap/Build S04 Procedure Pipeline")]
        public static void BuildS04ProcedurePipeline()
        {
            var scene = EditorSceneManager.OpenScene(AIProcedureScenePath, OpenSceneMode.Single);
            if (scene == null)
            {
                Debug.LogError($"Could not open scene: {AIProcedureScenePath}");
                return;
            }

            var environmentRoot = GameObject.Find("AIProcedureEnvironment")?.transform;
            if (environmentRoot == null || environmentRoot.name != "AIProcedureEnvironment")
            {
                Debug.LogWarning("AIProcedureEnvironment not found in scene");
                return;
            }

            // Create materials using URP
            var urpLitShader = Shader.Find("Universal Render Pipeline/Lit");

            // 1. YDS-20 Cryogenic Semen Tank (based on Oxygen Cylinder)
            var nitrogenTank = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            nitrogenTank.name = "NitrogenTank";
            nitrogenTank.transform.SetParent(environmentRoot.transform, false);
            var tankRenderer = nitrogenTank.GetComponent<MeshRenderer>();
            var tankMaterial = new Material(urpLitShader);
            tankMaterial.color = new Color(0.7f, 0.85f, 1f, 1f);
            tankRenderer.material = tankMaterial;
            Undo.AddComponent<MeshCollider>(nitrogenTank).convex = true;
            // Scale to fat, stubby flask shape
            nitrogenTank.transform.localScale = new Vector3(2.5f, 0.8f, 2.5f);
            nitrogenTank.transform.localPosition = new Vector3(3.35f, 0.85f, 3.2f);

            // 2. Thawing Thermos Flask
            var thawingFlask = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thawingFlask.name = "ThawingThermosFlask";
            thawingFlask.transform.SetParent(environmentRoot.transform, false);
            var flaskRenderer = thawingFlask.GetComponent<MeshRenderer>();
            var flaskMaterial = new Material(urpLitShader);
            flaskMaterial.color = new Color(0.5f, 0.3f, 0.6f, 1f); // Deep purple
            flaskRenderer.material = flaskMaterial;
            Undo.AddComponent<MeshCollider>(thawingFlask).convex = true;
            thawingFlask.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
            thawingFlask.transform.localPosition = new Vector3(-1.5f, 1.0f, 4.0f);

            // 3. Warm Water Beaker (for thawing timer)
            var warmWaterBeaker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            warmWaterBeaker.name = "WarmWaterBeaker";
            warmWaterBeaker.transform.SetParent(environmentRoot.transform, false);
            var beakerRenderer = warmWaterBeaker.GetComponent<MeshRenderer>();
            var beakerMaterial = new Material(urpLitShader);
            beakerMaterial.color = new Color(0.8f, 0.8f, 0.85f, 1f);
            beakerRenderer.material = beakerMaterial;
            Undo.AddComponent<MeshCollider>(warmWaterBeaker).convex = true;
            warmWaterBeaker.transform.localScale = new Vector3(0.5f, 0.7f, 0.5f);
            warmWaterBeaker.transform.localPosition = new Vector3(-1.2f, 0.9f, 4.2f);

            // 4. Straw Cutter
            var strawCutter = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            strawCutter.name = "StrawCutter";
            strawCutter.transform.SetParent(environmentRoot.transform, false);
            var cutterRenderer = strawCutter.GetComponent<MeshRenderer>();
            var cutterMaterial = new Material(urpLitShader);
            cutterMaterial.color = new Color(0.6f, 0.6f, 0.7f, 1f);
            cutterRenderer.material = cutterMaterial;
            Undo.AddComponent<BoxCollider>(strawCutter);
            strawCutter.transform.localScale = new Vector3(0.1f, 0.15f, 0.3f);
            strawCutter.transform.localPosition = new Vector3(-1.0f, 0.9f, 4.2f);

            // 5. Bovine AI Gun / Pistolet
            var pistolette = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pistolette.name = "Pistolette";
            pistolette.transform.SetParent(environmentRoot.transform, false);
            var barrelRenderer = pistolette.GetComponent<MeshRenderer>();
            var barrelMaterial = new Material(urpLitShader);
            barrelMaterial.color = new Color(0.7f, 0.7f, 0.75f, 1f); // Silver
            barrelRenderer.material = barrelMaterial;
            var barrelCollider = Undo.AddComponent<CapsuleCollider>(pistolette);
            barrelCollider.height = 0.45f;
            barrelCollider.radius = 0.025f;
            barrelCollider.direction = 2; // Z-axis
            pistolette.transform.localPosition = new Vector3(-1.45f, 1.08f, 4.05f);
            pistolette.transform.localRotation = Quaternion.Euler(0, -45, 0);

            // Hub Ring (quick-release plastic adapter)
            var hubRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            hubRing.name = "HubRing";
            hubRing.transform.SetParent(pistolette.transform, false);
            var ringRenderer = hubRing.GetComponent<MeshRenderer>();
            var ringMaterial = new Material(urpLitShader);
            ringMaterial.color = new Color(0.2f, 0.6f, 0.8f, 1f); // Blue
            ringRenderer.material = ringMaterial;
            Undo.AddComponent<BoxCollider>(hubRing);
            hubRing.transform.localScale = new Vector3(0.8f, 0.1f, 0.8f);
            hubRing.transform.localPosition = new Vector3(0, -0.22f, 0);

            // Plunger
            var plunger = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            plunger.name = "Plunger";
            plunger.transform.SetParent(pistolette.transform, false);
            var plungerRenderer = plunger.GetComponent<MeshRenderer>();
            var plungerMaterial = new Material(urpLitShader);
            plungerMaterial.color = new Color(0.5f, 0.5f, 0.55f, 1f);
            plungerRenderer.material = plungerMaterial;
            plunger.transform.localScale = new Vector3(0.03f, 0.5f, 0.03f);
            plunger.transform.localPosition = new Vector3(0, 0, -0.22f);

            // Thumb Button
            var thumbButton = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            thumbButton.name = "ThumbButton";
            thumbButton.transform.SetParent(plunger.transform, false);
            var buttonRenderer = thumbButton.GetComponent<MeshRenderer>();
            var buttonMaterial = new Material(urpLitShader);
            buttonMaterial.color = new Color(0.8f, 0.8f, 0.85f, 1f);
            buttonRenderer.material = buttonMaterial;
            Undo.AddComponent<BoxCollider>(thumbButton);
            thumbButton.transform.localScale = new Vector3(0.15f, 0.05f, 0.15f);
            thumbButton.transform.localPosition = new Vector3(0, 0, 0.075f);

            // 6. Vulva Entry Zone (BoxCollider trigger)
            var vulvaEntryZone = new GameObject("VulvaEntryZone");
            vulvaEntryZone.transform.SetParent(environmentRoot.transform, false);
            var vulvaTrigger = Undo.AddComponent<BoxCollider>(vulvaEntryZone);
            vulvaTrigger.isTrigger = true;
            vulvaTrigger.size = new Vector3(0.5f, 0.5f, 0.5f);
            vulvaEntryZone.transform.localPosition = new Vector3(-1.87f, 1.03f, 3.88f);

            // 7. Cervical Deposit Zone
            var cervicalZone = new GameObject("CervicalDepositZone");
            cervicalZone.transform.SetParent(environmentRoot.transform, false);
            var cervicalTrigger = Undo.AddComponent<BoxCollider>(cervicalZone);
            cervicalTrigger.isTrigger = true;
            cervicalTrigger.size = new Vector3(0.3f, 0.2f, 0.3f);
            cervicalZone.transform.localPosition = new Vector3(-1.87f, 1.03f, 3.88f);

            // 8. Uterine Too Deep Zone
            var uterineZone = new GameObject("UterineTooDeepZone");
            uterineZone.transform.SetParent(environmentRoot.transform, false);
            var uterineTrigger = Undo.AddComponent<BoxCollider>(uterineZone);
            uterineTrigger.isTrigger = true;
            uterineTrigger.size = new Vector3(0.3f, 0.2f, 0.3f);
            uterineZone.transform.localPosition = new Vector3(-1.87f, 1.03f, 4.2f);

            // 9. Anatomy Diagram Panel (World-space canvas)
            var anatomyPanel = new GameObject("AnatomyDiagramPanel");
            anatomyPanel.transform.SetParent(environmentRoot.transform, false);
            var panelCanvas = Undo.AddComponent<Canvas>(anatomyPanel);
            panelCanvas.renderMode = RenderMode.WorldSpace;
            panelCanvas.planeDistance = 1f;
            var panelRect = anatomyPanel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(0.5f, 0.3f);
            panelRect.localPosition = new Vector3(-1.5f, 1.5f, 3.5f);
            var panelCanvasScaler = anatomyPanel.AddComponent<CanvasScaler>();
            panelCanvasScaler.scaleFactor = 100;

            // 10. Insertion HUD Warning Text
            var hudWarning = new GameObject("InsertionHUDWarning");
            hudWarning.transform.SetParent(environmentRoot.transform, false);
            var warningText = hudWarning.AddComponent<TMPro.TextMeshPro>();
            warningText.text = "Dongak lebih -- jangan mendatar!";
            warningText.fontSize = 24;
            warningText.alignment = TextAlignmentOptions.Center;
            warningText.color = Color.red;
            hudWarning.transform.localPosition = new Vector3(-1.5f, 2.0f, 3.5f);

            // 11. Semen Selection UI Buttons (on WorldSpaceCanvas)
            var worldSpaceCanvas = Object.FindFirstObjectByType<Canvas>();

            // Create Beef Semen Button
            var beefButtonGO = new GameObject("SemenBeefButton");
            beefButtonGO.transform.SetParent(worldSpaceCanvas.transform, false);
            var beefButton = beefButtonGO.AddComponent<Button>();
            var beefImage = beefButtonGO.AddComponent<Image>();
            beefImage.color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
            var beefRect = beefButtonGO.GetComponent<RectTransform>();
            beefRect.sizeDelta = new Vector2(150, 50);
            beefRect.anchoredPosition = new Vector2(-100, -100);
            var beefTextGO = new GameObject("Text (Beef)");
            beefTextGO.transform.SetParent(beefButtonGO.transform, false);
            var beefText = beefTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            beefText.text = "Semen Lembu Daging (Beef)";
            beefText.fontSize = 14;
            beefText.alignment = TextAlignmentOptions.Center;
            var beefTextRect = beefTextGO.GetComponent<RectTransform>();
            beefTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            beefTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            beefTextRect.anchoredPosition = Vector2.zero;

            // Create Dairy Semen Button
            var dairyButtonGO = new GameObject("SemenDairyButton");
            dairyButtonGO.transform.SetParent(worldSpaceCanvas.transform, false);
            var dairyButton = dairyButtonGO.AddComponent<Button>();
            var dairyImage = dairyButtonGO.AddComponent<Image>();
            dairyImage.color = new Color(0.4f, 0.6f, 0.8f, 0.9f);
            var dairyRect = dairyButtonGO.GetComponent<RectTransform>();
            dairyRect.sizeDelta = new Vector2(150, 50);
            dairyRect.anchoredPosition = new Vector2(100, -100);
            var dairyTextGO = new GameObject("Text (Dairy)");
            dairyTextGO.transform.SetParent(dairyButtonGO.transform, false);
            var dairyText = dairyTextGO.AddComponent<TMPro.TextMeshProUGUI>();
            dairyText.text = "Semen Lembu Susu (Dairy)";
            dairyText.fontSize = 14;
            dairyText.alignment = TextAlignmentOptions.Center;
            var dairyTextRect = dairyTextGO.GetComponent<RectTransform>();
            dairyTextRect.anchorMin = new Vector2(0.5f, 0.5f);
            dairyTextRect.anchorMax = new Vector2(0.5f, 0.5f);
            dairyTextRect.anchoredPosition = Vector2.zero;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Successfully created S04 AI Procedure pipeline objects in '{AIProcedureScenePath}'.");
        }
    }
}
