using System.IO;
using UnityEditor;
using UnityEngine;

namespace VeterinarVR.Editor
{
    public static class CGTraderPrefabBuilder
    {
        private const string CleanedHospitalFolder = "Assets/ThirdParty/Free/CGTrader/Cleaned/Hospital";
        private const string RawHospitalFolder = "Assets/ThirdParty/Free/CGTrader/Hospital";
        private const string CleanedWarehouseFolder = "Assets/ThirdParty/Free/CGTrader/Cleaned/Warehouse";
        private const string PropsOutputFolder = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader";
        private const string EnvironmentOutputFolder = "Assets/_Project/VeterinarVR/Prefabs/Environment/CGTrader";
        private const string MaterialOutputFolder = "Assets/_Project/VeterinarVR/Art/Materials/CGTrader";

        [MenuItem("Veterinar VR/Assets/Build Cleaned CGTrader Prefabs")]
        public static void BuildCleanedCgtraderPrefabs()
        {
            EnsureFolder(PropsOutputFolder);
            EnsureFolder(EnvironmentOutputFolder);
            EnsureFolder(MaterialOutputFolder);

            var surgicalWhite = EnsureMaterial("mat_cgt_surgical_white", new Color(0.86f, 0.88f, 0.86f), 0.15f, 0.45f);
            var brushedMetal = EnsureMaterial("mat_cgt_brushed_metal", new Color(0.56f, 0.58f, 0.58f), 0.75f, 0.35f);
            var medicalBlue = EnsureMaterial("mat_cgt_medical_blue", new Color(0.18f, 0.43f, 0.62f), 0.1f, 0.45f);
            var utilityDark = EnsureMaterial("mat_cgt_utility_dark", new Color(0.12f, 0.14f, 0.15f), 0.35f, 0.5f);
            var concrete = EnsureMaterial("mat_cgt_concrete_floor", new Color(0.42f, 0.42f, 0.39f), 0.05f, 0.62f);
            var wallMaterial = EnsureMaterial("mat_cgt_warehouse_wall", new Color(0.64f, 0.66f, 0.63f), 0.05f, 0.55f);
            var roofMaterial = EnsureMaterial("mat_cgt_warehouse_roof", new Color(0.31f, 0.34f, 0.35f), 0.15f, 0.6f);
            var doorMaterial = EnsureMaterial("mat_cgt_warehouse_door", new Color(0.22f, 0.29f, 0.30f), 0.35f, 0.42f);

            BuildPropPrefab(
                CleanedHospitalFolder + "/MobileStand_UnityClean.fbx",
                PropsOutputFolder + "/pref_cgt_mobile_stand.prefab",
                "pref_cgt_mobile_stand",
                null);

            BuildPropPrefab(
                CleanedHospitalFolder + "/SteelTableOrStorage_UnityClean.fbx",
                PropsOutputFolder + "/pref_cgt_steel_table_storage.prefab",
                "pref_cgt_steel_table_storage",
                brushedMetal);

            BuildPropPrefab(
                CleanedHospitalFolder + "/SurgeryLamp_UnityClean.fbx",
                PropsOutputFolder + "/pref_cgt_surgery_lamp.prefab",
                "pref_cgt_surgery_lamp",
                surgicalWhite);

            BuildPropPrefab(
                RawHospitalFolder + "/Trolley/Fbx/Trolley.fbx",
                PropsOutputFolder + "/pref_cgt_trolley.prefab",
                "pref_cgt_trolley",
                brushedMetal);

            BuildPropPrefab(
                RawHospitalFolder + "/IVPole/Fbx/IVPole.fbx",
                PropsOutputFolder + "/pref_cgt_iv_pole.prefab",
                "pref_cgt_iv_pole",
                brushedMetal);

            BuildPropPrefab(
                RawHospitalFolder + "/OxygenCylinder/Mesh/Oxygen_Cylinder_Lp.fbx",
                PropsOutputFolder + "/pref_cgt_oxygen_cylinder.prefab",
                "pref_cgt_oxygen_cylinder",
                medicalBlue);

            BuildPropPrefab(
                RawHospitalFolder + "/MedicineWallCupboard/MedicineWallCupboard.fbx",
                PropsOutputFolder + "/pref_cgt_medicine_wall_cupboard.prefab",
                "pref_cgt_medicine_wall_cupboard",
                surgicalWhite);

            BuildPropPrefab(
                RawHospitalFolder + "/VialAndSyringe/VialAndSyringe.fbx",
                PropsOutputFolder + "/pref_cgt_vial_and_syringe.prefab",
                "pref_cgt_vial_and_syringe",
                null);

            BuildPropPrefab(
                RawHospitalFolder + "/Injection/Injection.obj",
                PropsOutputFolder + "/pref_cgt_injection_proxy.prefab",
                "pref_cgt_injection_proxy",
                utilityDark);

            BuildWarehousePrefab(
                CleanedWarehouseFolder + "/WarehouseInterior_ModularQuest.fbx",
                EnvironmentOutputFolder + "/pref_cgt_warehouse_modular_quest.prefab",
                concrete,
                wallMaterial,
                roofMaterial,
                doorMaterial);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void BuildPropPrefab(string sourcePath, string prefabPath, string prefabName, Material overrideMaterial)
        {
            var root = BuildWrapperRoot(sourcePath, prefabName, overrideMaterial);
            if (root == null)
            {
                return;
            }

            var bounds = GetRendererBounds(root);
            if (bounds.HasValue)
            {
                var collider = root.AddComponent<BoxCollider>();
                collider.center = root.transform.InverseTransformPoint(bounds.Value.center);
                collider.size = bounds.Value.size;
            }

            SavePrefab(root, prefabPath);
        }

        private static void BuildWarehousePrefab(
            string sourcePath,
            string prefabPath,
            Material floorMaterial,
            Material wallMaterial,
            Material roofMaterial,
            Material doorMaterial)
        {
            var root = BuildWrapperRoot(sourcePath, "pref_cgt_warehouse_modular_quest", null);
            if (root == null)
            {
                return;
            }

            foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
            {
                var lowerName = renderer.gameObject.name.ToLowerInvariant();
                if (lowerName.Contains("floor"))
                {
                    renderer.sharedMaterial = floorMaterial;
                }
                else if (lowerName.Contains("wall"))
                {
                    renderer.sharedMaterial = wallMaterial;
                }
                else if (lowerName.Contains("roof"))
                {
                    renderer.sharedMaterial = roofMaterial;
                }
                else if (lowerName.Contains("door"))
                {
                    renderer.sharedMaterial = doorMaterial;
                }
            }

            var bounds = GetRendererBounds(root);
            if (bounds.HasValue)
            {
                var floor = new GameObject("FloorCollider");
                floor.transform.SetParent(root.transform, false);

                var collider = floor.AddComponent<BoxCollider>();
                var localCenter = root.transform.InverseTransformPoint(bounds.Value.center);
                collider.center = new Vector3(localCenter.x, -0.03f, localCenter.z);
                collider.size = new Vector3(bounds.Value.size.x, 0.06f, bounds.Value.size.z);
            }

            SavePrefab(root, prefabPath);
        }

        private static GameObject BuildWrapperRoot(string sourcePath, string prefabName, Material overrideMaterial)
        {
            var source = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
            if (source == null)
            {
                Debug.LogWarning($"Could not load cleaned CGTrader source at '{sourcePath}'.");
                return null;
            }

            var root = new GameObject(prefabName);
            var visual = PrefabUtility.InstantiatePrefab(source) as GameObject;
            if (visual == null)
            {
                Object.DestroyImmediate(root);
                Debug.LogWarning($"Could not instantiate cleaned CGTrader source at '{sourcePath}'.");
                return null;
            }

            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;

            foreach (var collider in root.GetComponentsInChildren<Collider>(true))
            {
                Object.DestroyImmediate(collider);
            }

            if (overrideMaterial != null)
            {
                foreach (var renderer in root.GetComponentsInChildren<Renderer>(true))
                {
                    renderer.sharedMaterial = overrideMaterial;
                }
            }

            return root;
        }

        private static Bounds? GetRendererBounds(GameObject root)
        {
            var renderers = root.GetComponentsInChildren<Renderer>(true);
            if (renderers.Length == 0)
            {
                return null;
            }

            var bounds = renderers[0].bounds;
            for (var i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        private static void SavePrefab(GameObject root, string prefabPath)
        {
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }
        }

        private static Material EnsureMaterial(string materialName, Color baseColor, float metallic, float smoothness)
        {
            var materialPath = $"{MaterialOutputFolder}/{materialName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(FindLitShader());
                AssetDatabase.CreateAsset(material, materialPath);
            }

            material.color = baseColor;
            material.SetFloat("_Metallic", metallic);
            material.SetFloat("_Smoothness", smoothness);
            EditorUtility.SetDirty(material);
            return material;
        }

        private static Shader FindLitShader()
        {
            return Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        }

        private static void EnsureFolder(string folderPath)
        {
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                return;
            }

            var parent = Path.GetDirectoryName(folderPath)?.Replace("\\", "/");
            var folderName = Path.GetFileName(folderPath);

            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(folderName))
            {
                return;
            }

            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, folderName);
        }
    }
}
