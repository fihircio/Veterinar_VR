using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VeterinarVR.Editor
{
    public static class CGTraderSceneComposer
    {
        private const string ScenesFolder = "Assets/_Project/VeterinarVR/Scenes";
        private const string AIProcedureScenePath = ScenesFolder + "/S04_AIProcedure.unity";
        private const string ValidationScenePath = ScenesFolder + "/S05_ValidationDashboard.unity";
        private const string ResultsScenePath = ScenesFolder + "/S06_ResultsScoreboard.unity";

        private const string MobileStandPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_mobile_stand.prefab";
        private const string SteelTablePrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_steel_table_storage.prefab";
        private const string SurgeryLampPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_surgery_lamp.prefab";
        private const string TrolleyPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_trolley.prefab";
        private const string IVPolePrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_iv_pole.prefab";
        private const string OxygenCylinderPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_oxygen_cylinder.prefab";
        private const string MedicineWallCupboardPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_medicine_wall_cupboard.prefab";
        private const string VialAndSyringePrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_vial_and_syringe.prefab";
        private const string InjectionProxyPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/CGTrader/pref_cgt_injection_proxy.prefab";
        private const string WarehousePrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Environment/CGTrader/pref_cgt_warehouse_modular_quest.prefab";
        private const string NatureTreeRoot = "Assets/Nature Pack 2/Nature/Models_trees";
        private const string NatureStoneRoot = "Assets/Nature Pack 2/Stones/Stones group/Prefabs (to use in editor)";
        private const string ToonFarmGrassRoot = "Assets/Toon Farm Pack/Prefabs/Vegetation/Grass";

        [MenuItem("Veterinar VR/Assets/Compose CGTrader Scenes")]
        public static void ComposeCgtraderScenes()
        {
            ComposeAIProcedureScene();
            ComposeWarehouseScene(ValidationScenePath, "ValidationEnvironment", "CGTraderWarehouseDressing");
            ComposeWarehouseScene(ResultsScenePath, "ResultsEnvironment", "CGTraderWarehouseDressing");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Veterinar VR/Assets/Add Warehouse Outdoor Backdrops")]
        public static void AddWarehouseOutdoorBackdrops()
        {
            ComposeOutdoorBackdrop(
                ValidationScenePath,
                "ValidationEnvironment",
                new[]
                {
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree4.prefab", "BackTreeA", new Vector3(-34f, 0f, 45f), new Vector3(0f, 18f, 0f), new Vector3(1.9f, 1.9f, 1.9f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree6.prefab", "BackTreeB", new Vector3(-16f, 0f, 51f), new Vector3(0f, -12f, 0f), new Vector3(1.65f, 1.65f, 1.65f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree12.prefab", "BackTreeC", new Vector3(10f, 0f, 48f), new Vector3(0f, 34f, 0f), new Vector3(1.8f, 1.8f, 1.8f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree8.prefab", "SideTreeL", new Vector3(-48f, 0f, 12f), new Vector3(0f, 70f, 0f), new Vector3(1.45f, 1.45f, 1.45f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Bush2.prefab", "BushClusterA", new Vector3(-23f, 0f, 38f), new Vector3(0f, 22f, 0f), new Vector3(1.4f, 1.4f, 1.4f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Bush5.prefab", "BushClusterB", new Vector3(28f, 0f, 36f), new Vector3(0f, -30f, 0f), new Vector3(1.35f, 1.35f, 1.35f)),
                    new OutdoorPlacement($"{ToonFarmGrassRoot}/TFP_Grass_Patch_03A.prefab", "GrassPatchA", new Vector3(-18f, 0.02f, 35f), Vector3.zero, new Vector3(2.1f, 2.1f, 2.1f)),
                    new OutdoorPlacement($"{ToonFarmGrassRoot}/TFP_Grass_Patch_05A.prefab", "GrassPatchB", new Vector3(18f, 0.02f, 35f), new Vector3(0f, 45f, 0f), new Vector3(2f, 2f, 2f)),
                    new OutdoorPlacement($"{NatureStoneRoot}/prefab_stone_gr2.prefab", "StoneClusterA", new Vector3(34f, 0f, 24f), new Vector3(0f, 15f, 0f), new Vector3(1.25f, 1.25f, 1.25f)),
                });

            ComposeOutdoorBackdrop(
                ResultsScenePath,
                "ResultsEnvironment",
                new[]
                {
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree3.prefab", "BackTreeA", new Vector3(-30f, 0f, 42f), new Vector3(0f, -26f, 0f), new Vector3(1.7f, 1.7f, 1.7f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree10.prefab", "BackTreeB", new Vector3(-8f, 0f, 48f), new Vector3(0f, 18f, 0f), new Vector3(1.95f, 1.95f, 1.95f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree14.prefab", "BackTreeC", new Vector3(18f, 0f, 44f), new Vector3(0f, 42f, 0f), new Vector3(1.55f, 1.55f, 1.55f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Tree7.prefab", "SideTreeR", new Vector3(45f, 0f, 10f), new Vector3(0f, -55f, 0f), new Vector3(1.5f, 1.5f, 1.5f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Bush1.prefab", "BushClusterA", new Vector3(-25f, 0f, 34f), new Vector3(0f, 20f, 0f), new Vector3(1.35f, 1.35f, 1.35f)),
                    new OutdoorPlacement($"{NatureTreeRoot}/Bush6.prefab", "BushClusterB", new Vector3(24f, 0f, 32f), new Vector3(0f, -15f, 0f), new Vector3(1.45f, 1.45f, 1.45f)),
                    new OutdoorPlacement($"{ToonFarmGrassRoot}/TFP_Grass_Patch_01A.prefab", "GrassPatchA", new Vector3(-16f, 0.02f, 31f), new Vector3(0f, 15f, 0f), new Vector3(2.05f, 2.05f, 2.05f)),
                    new OutdoorPlacement($"{ToonFarmGrassRoot}/TFP_Grass_Patch_06A.prefab", "GrassPatchB", new Vector3(16f, 0.02f, 31f), new Vector3(0f, -35f, 0f), new Vector3(1.9f, 1.9f, 1.9f)),
                    new OutdoorPlacement($"{NatureStoneRoot}/prefab_stone_gr4.prefab", "StoneClusterA", new Vector3(-36f, 0f, 20f), new Vector3(0f, -18f, 0f), new Vector3(1.2f, 1.2f, 1.2f)),
                });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Veterinar VR/Assets/Add S04 Additional Hospital Props")]
        public static void AddS04AdditionalHospitalProps()
        {
            var scene = EditorSceneManager.OpenScene(AIProcedureScenePath);
            var environment = GameObject.Find("AIProcedureEnvironment");
            if (environment == null)
            {
                Debug.LogWarning("Could not find AIProcedureEnvironment in S04.");
                return;
            }

            var root = GetOrCreateChild(environment.transform, "CGTraderHospitalAdditionalDressing");

            AddPrefabIfMissing(
                root.transform,
                TrolleyPrefabPath,
                "Trolley",
                new Vector3(-3.25f, 0f, 2.65f),
                Quaternion.Euler(0f, 90f, 0f),
                Vector3.one);

            AddPrefabIfMissing(
                root.transform,
                IVPolePrefabPath,
                "IVPole",
                new Vector3(2.95f, 0f, 1.85f),
                Quaternion.identity,
                Vector3.one);

            AddPrefabIfMissing(
                root.transform,
                OxygenCylinderPrefabPath,
                "OxygenCylinder",
                new Vector3(3.35f, 0f, 3.2f),
                Quaternion.Euler(0f, -20f, 0f),
                Vector3.one);

            AddPrefabIfMissing(
                root.transform,
                MedicineWallCupboardPrefabPath,
                "MedicineWallCupboard",
                new Vector3(-3.4f, 1.45f, 4.85f),
                Quaternion.identity,
                Vector3.one);

            AddPrefabIfMissing(
                root.transform,
                VialAndSyringePrefabPath,
                "VialAndSyringe",
                new Vector3(-2.2f, 1.05f, 2.85f),
                Quaternion.Euler(0f, 25f, 0f),
                Vector3.one * 0.35f);

            AddPrefabIfMissing(
                root.transform,
                InjectionProxyPrefabPath,
                "InjectionProxy",
                new Vector3(-2.65f, 1.05f, 2.95f),
                Quaternion.Euler(0f, -35f, 0f),
                Vector3.one * 0.25f);

            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ComposeAIProcedureScene()
        {
            var scene = EditorSceneManager.OpenScene(AIProcedureScenePath);
            var environment = GameObject.Find("AIProcedureEnvironment");
            if (environment == null)
            {
                Debug.LogWarning("Could not find AIProcedureEnvironment in S04.");
                return;
            }

            var root = CreateFreshChild(environment.transform, "CGTraderHospitalDressing");

            AddPrefab(
                root.transform,
                SteelTablePrefabPath,
                "SteelTableStorage",
                new Vector3(-2.15f, -0.92f, 3.25f),
                Quaternion.identity,
                Vector3.one * 4.72f);

            AddPrefab(
                root.transform,
                MobileStandPrefabPath,
                "MobileStand",
                new Vector3(2.35f, 0.81f, 3.15f),
                Quaternion.Euler(-90f, 0f, 0f),
                Vector3.one);

            AddPrefab(
                root.transform,
                SurgeryLampPrefabPath,
                "InspectionLamp",
                new Vector3(1.85f, 2.05f, 2.25f),
                Quaternion.Euler(-90f, 0f, 0f),
                Vector3.one);

            EditorSceneManager.SaveScene(scene);
        }

        private static void ComposeWarehouseScene(string scenePath, string environmentName, string dressingName)
        {
            var scene = EditorSceneManager.OpenScene(scenePath);
            var environment = GameObject.Find(environmentName);
            if (environment == null)
            {
                Debug.LogWarning($"Could not find {environmentName} in {scenePath}.");
                return;
            }

            var root = CreateFreshChild(environment.transform, dressingName);

            AddPrefab(
                root.transform,
                WarehousePrefabPath,
                "WarehouseShell",
                new Vector3(-10.41f, -1.32f, 0.44f),
                Quaternion.identity,
                Vector3.one * 0.956f);

            EditorSceneManager.SaveScene(scene);
        }

        private static void ComposeOutdoorBackdrop(string scenePath, string environmentName, OutdoorPlacement[] placements)
        {
            var scene = EditorSceneManager.OpenScene(scenePath);
            var environment = GameObject.Find(environmentName);
            if (environment == null)
            {
                Debug.LogWarning($"Could not find {environmentName} in {scenePath}.");
                return;
            }

            var root = GetOrCreateChild(environment.transform, "OutdoorWarehouseBackdrop");

            foreach (var placement in placements)
            {
                AddPrefabIfMissing(
                    root.transform,
                    placement.PrefabPath,
                    placement.Name,
                    placement.LocalPosition,
                    Quaternion.Euler(placement.LocalEulerAngles),
                    placement.LocalScale);
            }

            EditorSceneManager.SaveScene(scene);
        }

        private static GameObject CreateFreshChild(Transform parent, string childName)
        {
            var existing = parent.Find(childName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            var root = new GameObject(childName);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
            return root;
        }

        private static GameObject GetOrCreateChild(Transform parent, string childName)
        {
            var existing = parent.Find(childName);
            if (existing != null)
            {
                return existing.gameObject;
            }

            var root = new GameObject(childName);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = Vector3.zero;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one;
            return root;
        }

        private static GameObject AddPrefabIfMissing(
            Transform parent,
            string prefabPath,
            string instanceName,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            var existing = parent.Find(instanceName);
            if (existing != null)
            {
                return existing.gameObject;
            }

            return AddPrefab(parent, prefabPath, instanceName, localPosition, localRotation, localScale);
        }

        private static GameObject AddPrefab(
            Transform parent,
            string prefabPath,
            string instanceName,
            Vector3 localPosition,
            Quaternion localRotation,
            Vector3 localScale)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Could not load CGTrader prefab at '{prefabPath}'.");
                return null;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                return null;
            }

            instance.name = instanceName;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = localRotation;
            instance.transform.localScale = localScale;
            return instance;
        }

        private readonly struct OutdoorPlacement
        {
            public OutdoorPlacement(string prefabPath, string name, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
            {
                PrefabPath = prefabPath;
                Name = name;
                LocalPosition = localPosition;
                LocalEulerAngles = localEulerAngles;
                LocalScale = localScale;
            }

            public string PrefabPath { get; }
            public string Name { get; }
            public Vector3 LocalPosition { get; }
            public Vector3 LocalEulerAngles { get; }
            public Vector3 LocalScale { get; }
        }
    }
}
