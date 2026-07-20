using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using VeterinarVR.Audio;
using VeterinarVR.Core;
using VeterinarVR.Data;
using VeterinarVR.Gameplay;
using VeterinarVR.UI;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace VeterinarVR.Editor
{
    public static class VeterinarProjectBootstrap
    {
        private const string ScenesFolder = "Assets/_Project/VeterinarVR/Scenes";
        private const string GreetingScenePath = ScenesFolder + "/S01_Greeting.unity";
        private const string HerdScenePath = ScenesFolder + "/S02_HerdObservation.unity";
        private const string CowScanScenePath = ScenesFolder + "/S03_CowScanDecision.unity";
        private const string AIProcedureScenePath = ScenesFolder + "/S04_AIProcedure.unity";
        private const string ValidationScenePath = ScenesFolder + "/S05_ValidationDashboard.unity";
        private const string ResultsScenePath = ScenesFolder + "/S06_ResultsScoreboard.unity";
        private const string RenderingFolder = "Assets/_Project/VeterinarVR/Rendering";
        private const string ResourcesFolder = "Assets/_Project/VeterinarVR/Resources";
        private const string EnvironmentPrefabsFolder = "Assets/_Project/VeterinarVR/Prefabs/Environment";
        private const string ToonFarmSceneDressingsFolder = EnvironmentPrefabsFolder + "/ToonFarm";
        private const string AnimalPackCharactersFolder = "Assets/_Project/VeterinarVR/Prefabs/Characters/AnimalsFullPack";
        private const string AnimalPackEnvironmentFolder = EnvironmentPrefabsFolder + "/AnimalsFullPack";
        private const string NaturePackEnvironmentFolder = EnvironmentPrefabsFolder + "/NaturePack2";
        private const string XrOriginPrefabPath = "Assets/Samples/XR Interaction Toolkit/3.5.0/Starter Assets/Prefabs/XR Origin (XR Rig).prefab";
        private const string FreeCowModelPath = "Assets/ThirdParty/Free/Cows/realistic_holstein_cow_-_game_ready_asset_UnityClean.fbx";
        private const string CowPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Characters/pref_cow_hero.prefab";
        private const string CleanChuteModelPath = "Assets/ThirdParty/Free/Procedure/Node0_UnityClean.fbx";
        private const string ChutePrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Props/pref_cattle_chute.prefab";
        private const string UrpAssetPath = RenderingFolder + "/VeterinarVR_URP.asset";
        private const string UrpRendererPath = RenderingFolder + "/VeterinarVR_ForwardRenderer.asset";
        private const string GlobalVolumeProfilePath = RenderingFolder + "/VeterinarVR_GlobalVolumeProfile.asset";
        private const string DaySkyboxPath = RenderingFolder + "/VeterinarVR_DaySkybox.mat";
        private const string TrainingContentCatalogPath = ResourcesFolder + "/TrainingContentCatalog.asset";
        private const string AudioContentCatalogPath = ResourcesFolder + "/AudioContentCatalog.asset";
        private const string QuestionsFolder = "Assets/_Project/VeterinarVR/ScriptableObjects/Questions";
        private const string CowsFolder = "Assets/_Project/VeterinarVR/ScriptableObjects/Cows";
        private const string ToonFarmRoot = "Assets/Toon Farm Pack/Prefabs";
        private const string AnimalPackRoot = "Assets/Animals Full Pack/Farm Animals Pack";
        private const string AllSkyDayBlueSkyPath = "Assets/AllSkyFree/Cartoon Base BlueSky/Day_BlueSky_Nothing.mat";
        private const string AllSkyOvercastPath = "Assets/AllSkyFree/Overcast Low/AllSky_Overcast4_Low.mat";
        private const string GroundGrassMaterialPath = RenderingFolder + "/VeterinarVR_GroundGrass.mat";
        private const string GroundDirtMaterialPath = RenderingFolder + "/VeterinarVR_GroundDirt.mat";
        private const string GroundIndoorMaterialPath = RenderingFolder + "/VeterinarVR_GroundIndoor.mat";
        private const string GreetingDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s01_toonfarm_dressing.prefab";
        private const string HerdDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s02_toonfarm_dressing.prefab";
        private const string ScanDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s03_toonfarm_dressing.prefab";
        private const string ProcedureDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s04_toonfarm_dressing.prefab";
        private const string ValidationDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s05_toonfarm_dressing.prefab";
        private const string ResultsDressingPrefabPath = ToonFarmSceneDressingsFolder + "/pref_s06_toonfarm_dressing.prefab";
        private const string AnimalPackCow1PrefabPath = AnimalPackCharactersFolder + "/pref_afp_cow1_pbr.prefab";
        private const string AnimalPackCow2PrefabPath = AnimalPackCharactersFolder + "/pref_afp_cow2_pbr.prefab";
        private const string AnimalPackCow3PrefabPath = AnimalPackCharactersFolder + "/pref_afp_cow3_pbr.prefab";
        private const string AnimalPackGoatPrefabPath = AnimalPackCharactersFolder + "/pref_afp_goat_pbr.prefab";
        private const string AnimalPackSheepPrefabPath = AnimalPackCharactersFolder + "/pref_afp_sheep_pbr.prefab";
        private const string AnimalPackChickenPrefabPath = AnimalPackCharactersFolder + "/pref_afp_chicken_pbr.prefab";
        private const string AnimalPackHerdDressingPrefabPath = AnimalPackEnvironmentFolder + "/pref_s02_animalsfullpack_background.prefab";
        private const string AnimalPackGreetingDressingPrefabPath = AnimalPackEnvironmentFolder + "/pref_s01_animalsfullpack_background.prefab";
        private const string NaturePackRoot = "Assets/Nature Pack 2";
        private const string NaturePackGreetingDressingPrefabPath = NaturePackEnvironmentFolder + "/pref_s01_naturepack2_dressing.prefab";
        private const string NaturePackHerdDressingPrefabPath = NaturePackEnvironmentFolder + "/pref_s02_naturepack2_dressing.prefab";
        private const string NaturePackScanDressingPrefabPath = NaturePackEnvironmentFolder + "/pref_s03_naturepack2_dressing.prefab";
        private const string NaturePackResultsDressingPrefabPath = NaturePackEnvironmentFolder + "/pref_s06_naturepack2_dressing.prefab";
        // Guide avatar uses Streamoji's purpose-built humanoid avatar assets.
        // Masculine.fbx ships with a pre-tuned humanDescription (no auto-generation,
        // no muscle guessing) which is what prevents the limb deformation we hit when
        // hand-rigging the converted GLB.
        private const string GuideAvatarSourceFbxPath = "Assets/Streamoji/AnimationAvatars/Masculine.fbx";
        // The talking clip lives on the combined avatar FBX (Blender 3.5 merge output).
        // It is rig-native (62 bones, frames 0-234) so it binds to the Streamoji armature
        // without retargeting. Blender 5 changed the Action API (no .fcurves), so the
        // standalone-clip conversion is blocked; the combined FBX already has the merged clip.
        private const string GuideTalkClipFbxPath = "Assets/ThirdParty/Free/Avatars/Cleaned/avatar_guide_with_talk.fbx";
        private const string GuideAvatarPrefabPath = "Assets/_Project/VeterinarVR/Prefabs/Characters/pref_guide_avatar.prefab";
        private const string GuideAnimatorControllerPath = "Assets/_Project/VeterinarVR/Animations/GuideAnimatorController.controller";
        private const string GuideTalkStateName = "Talk";
        // Final placement matches the user's idle reference avatar (avatar_guide_a_Unity in S01).
        private const float GuideAvatarPosX = -1.12f;
        private const float GuideAvatarPosY = 0.01f;
        private const float GuideAvatarPosZ = 1.75f;
        private const float GuideAvatarRotY = -218.973f;
        private static readonly string[] ProgressSceneIds =
        {
            SceneIds.HerdObservation,
            SceneIds.CowScanDecision,
            SceneIds.AIProcedure,
            SceneIds.ValidationDashboard,
            SceneIds.ResultsScoreboard
        };

        private static readonly (string English, string Bahasa)[] ProgressLabels =
        {
            ("Observe", "Pemerhatian"),
            ("Scan", "Imbas"),
            ("Procedure", "Prosedur"),
            ("Validate", "Sahkan"),
            ("Results", "Keputusan")
        };

        [MenuItem("Veterinar VR/Bootstrap/Sync Scene Set")]
        public static void SyncSceneSetFromMenu()
        {
            SyncSceneSet();
        }

        [MenuItem("Veterinar VR/Bootstrap/Rebuild Flow Scenes")]
        public static void RebuildFlowScenesFromMenu()
        {
            BuildHerdObservationScene();
            BuildCowScanDecisionScene();
            BuildAIProcedureScene();
            BuildValidationDashboardScene();
            BuildResultsScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Greeting Scene")]
        public static void BuildGreetingSceneFromMenu()
        {
            BuildGreetingScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Herd Observation Scene")]
        public static void BuildHerdObservationSceneFromMenu()
        {
            BuildHerdObservationScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Results Scene")]
        public static void BuildResultsSceneFromMenu()
        {
            BuildResultsScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Cow Scan Scene")]
        public static void BuildCowScanDecisionSceneFromMenu()
        {
            BuildCowScanDecisionScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Full Rebuild S03 (Scene + Dressings + Cow)")]
        public static void FullRebuildS03FromMenu()
        {
            FullRebuildS03();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build AI Procedure Scene")]
        public static void BuildAIProcedureSceneFromMenu()
        {
            BuildAIProcedureScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Validation Dashboard Scene")]
        public static void BuildValidationDashboardSceneFromMenu()
        {
            BuildValidationDashboardScene();
        }

        [MenuItem("Veterinar VR/Bootstrap/Setup URP Rendering")]
        public static void SetupUrpRenderingFromMenu()
        {
            SetupUrpRendering();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Cattle Chute Prefab")]
        public static void BuildCattleChutePrefabFromMenu()
        {
            BuildCattleChutePrefab();
        }

        [MenuItem("Veterinar VR/Bootstrap/Build Cow Hero Prefab")]
        public static void BuildCowHeroPrefabFromMenu()
        {
            BuildCowHeroPrefab();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync Training Content Catalog")]
        public static void SyncTrainingContentCatalogFromMenu()
        {
            EnsureTrainingContentCatalogAsset();
            EnsureCowAndQuestionDataAssets();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync Audio Content Catalog")]
        public static void SyncAudioContentCatalogFromMenu()
        {
            EnsureAudioContentCatalogAsset();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync Toon Farm Dressings")]
        public static void SyncToonFarmDressingsFromMenu()
        {
            EnsureToonFarmSceneDressings();
            ApplyToonFarmDressingsToScenes();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync AllSky And Animal Pack")]
        public static void SyncAllSkyAndAnimalPackFromMenu()
        {
            UpgradeAnimalPackMaterialsForUrp();
            EnsureGroundMaterials();
            EnsureToonFarmSceneDressings();
            EnsureAnimalPackWrappers();
            EnsureAnimalPackSceneDressings();
            ApplyAllSkyToCurrentScenes();
            ApplyToonFarmDressingsToScenes();
            ApplyAnimalPackDressingsToScenes();
            ApplyGroundMaterialsToCurrentScenes();
            ReplaceGameplayCowVisualsWithAnimalPack();
        }

        [MenuItem("Veterinar VR/Bootstrap/Repair Animal Materials And Grounds")]
        public static void RepairAnimalMaterialsAndGroundsFromMenu()
        {
            UpgradeAnimalPackMaterialsForUrp();
            EnsureGroundMaterials();
            ApplyGroundMaterialsToCurrentScenes();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync Cow Replacement And Greeting Animals")]
        public static void SyncCowReplacementAndGreetingAnimalsFromMenu()
        {
            UpgradeAnimalPackMaterialsForUrp();
            EnsureAnimalPackWrappers();
            EnsureAnimalPackSceneDressings();
            ApplyAnimalPackDressingsToScenes();
            ReplaceGameplayCowVisualsWithAnimalPack();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync AutoHand And Nature Pack 2")]
        public static void SyncAutoHandAndNaturePack2FromMenu()
        {
            ConfigureAutoHandProcedureObjects();
            UpgradeNaturePackMaterialsForUrp();
            UpgradeNaturePackTreePrefabMaterialsForUrp();
            EnsureNaturePackSceneDressings();
            ApplyNaturePackDressingsToScenes();
        }

        [MenuItem("Veterinar VR/Bootstrap/Sync Guide Avatar")]
        public static void SyncGuideAvatarFromMenu()
        {
            EnsureGuideAvatarImport();
            EnsureGuideAnimatorController();
            BuildGuideAvatarPrefab();
            PlaceGuideAvatarInGreetingScene();
        }

        public static void SyncSceneSet()
        {
            EnsureFolder(ScenesFolder);

            CreatePlaceholderScene(SceneIds.Greeting, "Scene 01: Greeting");
            CreatePlaceholderScene(SceneIds.HerdObservation, "Scene 02: Herd Observation");
            CreatePlaceholderScene(SceneIds.CowScanDecision, "Scene 03: Cow Scan Decision");
            CreatePlaceholderScene(SceneIds.AIProcedure, "Scene 04: AI Procedure");
            CreatePlaceholderScene(SceneIds.ValidationDashboard, "Scene 05: Validation Dashboard");
            CreatePlaceholderScene(SceneIds.ResultsScoreboard, "Scene 06: Results Scoreboard");

            EditorBuildSettings.scenes = new[]
            {
                BuildScene(SceneIds.XRSmokeTest),
                BuildScene(SceneIds.Greeting),
                BuildScene(SceneIds.HerdObservation),
                BuildScene(SceneIds.CowScanDecision),
                BuildScene(SceneIds.AIProcedure),
                BuildScene(SceneIds.ValidationDashboard),
                BuildScene(SceneIds.ResultsScoreboard)
            };

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildGreetingScene()
        {
            EnsureFolder(ScenesFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.Greeting;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateGreetingEnvironment();
            CreateGreetingCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, GreetingScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildHerdObservationScene()
        {
            EnsureFolder(ScenesFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.HerdObservation;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateHerdEnvironment();
            CreateHerdCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, HerdScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildResultsScene()
        {
            EnsureFolder(ScenesFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.ResultsScoreboard;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateResultsEnvironment();
            CreateResultsCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, ResultsScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildCowScanDecisionScene()
        {
            EnsureFolder(ScenesFolder);

            EnableHandTracking();

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.CowScanDecision;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateCowScanEnvironment();
            CreateCowScanCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, CowScanScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void FullRebuildS03()
        {
            EnsureToonFarmSceneDressings();
            EnsureGroundMaterials();

            BuildCowScanDecisionScene();

            ApplyDressingToScene(CowScanScenePath, "CowScanEnvironment", ScanDressingPrefabPath);
            ApplyDressingToScene(CowScanScenePath, "CowScanEnvironment", NaturePackScanDressingPrefabPath, "NaturePack2Dressing");

            var daySkybox = AssetDatabase.LoadAssetAtPath<Material>(AllSkyDayBlueSkyPath);
            ApplySkyboxToScene(CowScanScenePath, daySkybox);

            ReplaceCowVisualsInScene(CowScanScenePath, new[]
            {
                ("ScanCow", AnimalPackCow2PrefabPath)
            });

            var grass = AssetDatabase.LoadAssetAtPath<Material>(GroundGrassMaterialPath);
            if (grass != null)
            {
                ApplyGroundMaterialToScene(CowScanScenePath, grass);
            }

            Debug.Log("S03 full rebuild complete: scene + dressings + skybox + cow + ground.");
        }

        public static void BuildAIProcedureScene()
        {
            EnsureFolder(ScenesFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.AIProcedure;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateAIProcedureEnvironment();
            CreateAIProcedureCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, AIProcedureScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildValidationDashboardScene()
        {
            EnsureFolder(ScenesFolder);

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = SceneIds.ValidationDashboard;

            CreateLighting();
            ApplySharedSceneRenderingSettings();
            CreateAppRoot();
            CreateXrOrigin();
            CreateValidationEnvironment();
            CreateValidationCanvas();
            CreateEventSystem();

            EditorSceneManager.SaveScene(scene, ValidationScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildCattleChutePrefab()
        {
            EnsureFolder("Assets/_Project/VeterinarVR/Prefabs/Props");

            var sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>(CleanChuteModelPath);
            if (sourceModel == null)
            {
                Debug.LogWarning($"Could not load cleaned chute model at '{CleanChuteModelPath}'.");
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(sourceModel) as GameObject;
            if (instance == null)
            {
                Debug.LogWarning("Could not instantiate cleaned chute model.");
                return;
            }

            instance.name = "pref_cattle_chute";
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            var rootCollider = instance.GetComponent<BoxCollider>();
            if (rootCollider == null)
            {
                rootCollider = instance.AddComponent<BoxCollider>();
            }

            rootCollider.center = new Vector3(0f, 0.5f, 0f);
            rootCollider.size = new Vector3(0.6f, 1.0f, 2.1f);

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, ChutePrefabPath);
            Object.DestroyImmediate(instance);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void SetupUrpRendering()
        {
            EnsureFolder(RenderingFolder);

            var rendererData = EnsureUrpRendererData();
            var pipelineAsset = EnsureUrpAsset(rendererData);
            var volumeProfile = EnsureGlobalVolumeProfile();
            var skyboxMaterial = EnsureDaySkyboxMaterial();

            pipelineAsset.supportsHDR = true;
            pipelineAsset.msaaSampleCount = 2;
            pipelineAsset.renderScale = 1.0f;
            pipelineAsset.supportsCameraDepthTexture = true;
            pipelineAsset.supportsCameraOpaqueTexture = false;
            pipelineAsset.shadowDistance = 25f;
            pipelineAsset.volumeProfile = volumeProfile;
            ConfigureUrpLightingSettings(pipelineAsset);

            EditorUtility.SetDirty(rendererData);
            EditorUtility.SetDirty(volumeProfile);
            EditorUtility.SetDirty(skyboxMaterial);
            EditorUtility.SetDirty(pipelineAsset);

            GraphicsSettings.defaultRenderPipeline = pipelineAsset;
            ApplyPipelineToAllQualityLevels(pipelineAsset);
            GraphicsSettings.lightsUseLinearIntensity = true;
            GraphicsSettings.lightsUseColorTemperature = true;

            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.76f, 0.82f, 0.88f, 1f);
            RenderSettings.fogStartDistance = 18f;
            RenderSettings.fogEndDistance = 42f;
            RenderSettings.subtractiveShadowColor = new Color(0.42f, 0.47f, 0.52f, 1f);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void BuildCowHeroPrefab()
        {
            EnsureFolder("Assets/_Project/VeterinarVR/Prefabs/Characters");

            var sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>(FreeCowModelPath);
            if (sourceModel == null)
            {
                Debug.LogWarning($"Could not load free cow model at '{FreeCowModelPath}'.");
                return;
            }

            var instance = new GameObject("pref_cow_hero");
            var visualInstance = PrefabUtility.InstantiatePrefab(sourceModel) as GameObject;
            if (visualInstance == null)
            {
                Object.DestroyImmediate(instance);
                Debug.LogWarning("Could not instantiate free cow model.");
                return;
            }

            visualInstance.name = "Visual";
            visualInstance.transform.SetParent(instance.transform, false);
            visualInstance.transform.localPosition = Vector3.zero;
            visualInstance.transform.localRotation = Quaternion.identity;
            visualInstance.transform.localScale = Vector3.one;

            var bounds = CalculateCombinedBounds(instance);
            visualInstance.transform.position += new Vector3(0f, -bounds.min.y, 0f);
            bounds = CalculateCombinedBounds(instance);

            var rootCollider = instance.AddComponent<BoxCollider>();
            rootCollider.center = instance.transform.InverseTransformPoint(bounds.center);
            rootCollider.size = bounds.size;

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, CowPrefabPath);
            Object.DestroyImmediate(instance);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void CreatePlaceholderScene(string sceneId, string label)
        {
            var path = $"{ScenesFolder}/{sceneId}.unity";
            if (File.Exists(path))
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            scene.name = sceneId;

            var root = new GameObject("SceneNotes");
            var note = root.AddComponent<PlaceholderSceneNote>();
            note.SetLabel(label);

            EditorSceneManager.SaveScene(scene, path);
        }

        private static TrainingContentCatalog EnsureTrainingContentCatalogAsset()
        {
            EnsureFolder(ResourcesFolder);

            var existing = AssetDatabase.LoadAssetAtPath<TrainingContentCatalog>(TrainingContentCatalogPath);
            if (existing != null)
            {
                return existing;
            }

            var asset = ScriptableObject.CreateInstance<TrainingContentCatalog>();
            AssetDatabase.CreateAsset(asset, TrainingContentCatalogPath);
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private static AudioContentCatalog EnsureAudioContentCatalogAsset()
        {
            EnsureFolder(ResourcesFolder);

            var asset = AssetDatabase.LoadAssetAtPath<AudioContentCatalog>(AudioContentCatalogPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<AudioContentCatalog>();
                AssetDatabase.CreateAsset(asset, AudioContentCatalogPath);
            }

            SetPrivateObject(asset, "uiClick", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Samples/XR Interaction Toolkit/3.5.0/Starter Assets/DemoAssets/Audio/Button Pop.wav"));
            SetPrivateObject(asset, "uiConfirm", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Samples/XR Interaction Toolkit/3.5.0/Starter Assets/DemoAssets/Audio/Button Pop.wav"));
            SetPrivateObject(asset, "outdoorLoop", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Bonus/Forest Loop.wav"));
            SetPrivateObject(asset, "indoorLoop", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Bonus/Stream Moderate Loop 1.wav"));
            SetPrivateAudioClipArray(asset, "cowCalls", new AudioClip[]
            {
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Cow/Cow 02.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Cow/Cow 08.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Cow/Cow 12.wav")
            });
            SetPrivateAudioClipArray(asset, "birdCalls", new AudioClip[]
            {
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Birds/Bird 3_02.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Birds/Bird 5_03.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Birds/Bird 11_02.wav")
            });
            SetPrivateAudioClipArray(asset, "grainHandling", new AudioClip[]
            {
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Bonus/Grains 03.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Bonus/Grains 06.wav"),
                AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Bonus/Grains 09.wav")
            });
            SetPrivateObject(asset, "resultCue", AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Farm Animal Sounds/Animals/Birds/Bird 12_02.wav"));

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return asset;
        }

        private static void EnsureToonFarmSceneDressings()
        {
            EnsureFolder(EnvironmentPrefabsFolder);
            EnsureFolder(ToonFarmSceneDressingsFolder);

            BuildGreetingToonFarmDressing();
            BuildHerdToonFarmDressing();
            BuildScanToonFarmDressing();
            BuildProcedureToonFarmDressing();
            BuildValidationToonFarmDressing();
            BuildResultsToonFarmDressing();
        }

        private static void ApplyToonFarmDressingsToScenes()
        {
            ApplyDressingToScene(GreetingScenePath, "GreetingEnvironment", GreetingDressingPrefabPath);
            ApplyDressingToScene(HerdScenePath, "HerdEnvironment", HerdDressingPrefabPath);
            ApplyDressingToScene(CowScanScenePath, "CowScanEnvironment", ScanDressingPrefabPath);
            ApplyDressingToScene(AIProcedureScenePath, "AIProcedureEnvironment", ProcedureDressingPrefabPath);
            ApplyDressingToScene(ValidationScenePath, "ValidationEnvironment", ValidationDressingPrefabPath);
            ApplyDressingToScene(ResultsScenePath, "ResultsEnvironment", ResultsDressingPrefabPath);
        }

        private static void EnsureAnimalPackWrappers()
        {
            EnsureFolder(AnimalPackCharactersFolder);

            BuildAnimalWrapper($"{AnimalPackRoot}/Cow/Prefabs/Cow1_PBR.prefab", AnimalPackCow1PrefabPath, "pref_afp_cow1_pbr", new Vector3(1f, 1f, 1f));
            BuildAnimalWrapper($"{AnimalPackRoot}/Cow/Prefabs/Cow2_PBR.prefab", AnimalPackCow2PrefabPath, "pref_afp_cow2_pbr", new Vector3(1f, 1f, 1f));
            BuildAnimalWrapper($"{AnimalPackRoot}/Cow/Prefabs/Cow3_PBR.prefab", AnimalPackCow3PrefabPath, "pref_afp_cow3_pbr", new Vector3(1f, 1f, 1f));
            BuildAnimalWrapper($"{AnimalPackRoot}/Goat/Prefabs/Goat_PBR.prefab", AnimalPackGoatPrefabPath, "pref_afp_goat_pbr", new Vector3(1f, 1f, 1f));
            BuildAnimalWrapper($"{AnimalPackRoot}/Sheep/Prefabs/Sheep_PBR.prefab", AnimalPackSheepPrefabPath, "pref_afp_sheep_pbr", new Vector3(1f, 1f, 1f));
            BuildAnimalWrapper($"{AnimalPackRoot}/Chicken/Prefabs/Chicken1_PBR.prefab", AnimalPackChickenPrefabPath, "pref_afp_chicken_pbr", new Vector3(1f, 1f, 1f));
        }

        private static void EnsureAnimalPackSceneDressings()
        {
            EnsureFolder(AnimalPackEnvironmentFolder);

            BuildSceneDressingPrefab(AnimalPackHerdDressingPrefabPath, root =>
            {
                var group = new GameObject("LicensedAnimatedBackgroundAnimals");
                group.transform.SetParent(root, false);

                AddProjectPrefab(group.transform, "BackgroundCowGrazingA", AnimalPackCow1PrefabPath, new Vector3(-7.8f, 0f, 9.8f), new Vector3(0f, 34f, 0f), Vector3.one * 0.82f);
                AddProjectPrefab(group.transform, "BackgroundCowGrazingB", AnimalPackCow2PrefabPath, new Vector3(7.7f, 0f, 9.6f), new Vector3(0f, -28f, 0f), Vector3.one * 0.78f);
                AddProjectPrefab(group.transform, "BackgroundSheep", AnimalPackSheepPrefabPath, new Vector3(-5.8f, 0f, 10.9f), new Vector3(0f, 18f, 0f), Vector3.one * 0.72f);
                AddProjectPrefab(group.transform, "BackgroundGoat", AnimalPackGoatPrefabPath, new Vector3(5.9f, 0f, 10.7f), new Vector3(0f, -14f, 0f), Vector3.one * 0.7f);
                AddProjectPrefab(group.transform, "BackgroundChicken", AnimalPackChickenPrefabPath, new Vector3(-3.6f, 0f, 3.2f), new Vector3(0f, 140f, 0f), Vector3.one * 0.38f);

                DisableColliders(group);
            });

            BuildSceneDressingPrefab(AnimalPackGreetingDressingPrefabPath, root =>
            {
                var group = new GameObject("LicensedGreetingAnimalLife");
                group.transform.SetParent(root, false);

                AddProjectPrefab(group.transform, "GreetingChickenA", AnimalPackChickenPrefabPath, new Vector3(-2.7f, 0f, 3.7f), new Vector3(0f, 125f, 0f), Vector3.one * 0.38f);
                AddProjectPrefab(group.transform, "GreetingChickenB", AnimalPackChickenPrefabPath, new Vector3(2.8f, 0f, 3.9f), new Vector3(0f, -115f, 0f), Vector3.one * 0.36f);
                AddProjectPrefab(group.transform, "GreetingChickenC", AnimalPackChickenPrefabPath, new Vector3(-4.9f, 0f, 6.2f), new Vector3(0f, 70f, 0f), Vector3.one * 0.34f);
                AddProjectPrefab(group.transform, "GreetingChickenD", AnimalPackChickenPrefabPath, new Vector3(4.6f, 0f, 6.5f), new Vector3(0f, -70f, 0f), Vector3.one * 0.34f);
                AddProjectPrefab(group.transform, "GreetingSheepLeft", AnimalPackSheepPrefabPath, new Vector3(-6.6f, 0f, 8.4f), new Vector3(0f, 28f, 0f), Vector3.one * 0.62f);
                AddProjectPrefab(group.transform, "GreetingSheepRight", AnimalPackSheepPrefabPath, new Vector3(6.9f, 0f, 8.8f), new Vector3(0f, -28f, 0f), Vector3.one * 0.62f);
                AddProjectPrefab(group.transform, "GreetingGoatDistant", AnimalPackGoatPrefabPath, new Vector3(0.9f, 0f, 8.6f), new Vector3(0f, -12f, 0f), Vector3.one * 0.58f);

                DisableColliders(group);
            });
        }

        private static void ApplyAllSkyToCurrentScenes()
        {
            var daySkybox = AssetDatabase.LoadAssetAtPath<Material>(AllSkyDayBlueSkyPath);
            var overcastSkybox = AssetDatabase.LoadAssetAtPath<Material>(AllSkyOvercastPath);

            ApplySkyboxToScene(GreetingScenePath, daySkybox);
            ApplySkyboxToScene(HerdScenePath, daySkybox);
            ApplySkyboxToScene(CowScanScenePath, daySkybox);
            ApplySkyboxToScene(ResultsScenePath, daySkybox);
            ApplySkyboxToScene(AIProcedureScenePath, overcastSkybox != null ? overcastSkybox : daySkybox);
            ApplySkyboxToScene(ValidationScenePath, overcastSkybox != null ? overcastSkybox : daySkybox);
        }

        private static void ApplyAnimalPackDressingsToScenes()
        {
            ApplyDressingToScene(GreetingScenePath, "GreetingEnvironment", AnimalPackGreetingDressingPrefabPath, "AnimalsFullPackDressing");
            ApplyDressingToScene(HerdScenePath, "HerdEnvironment", AnimalPackHerdDressingPrefabPath, "AnimalsFullPackDressing");
        }

        private static void EnsureNaturePackSceneDressings()
        {
            EnsureFolder(NaturePackEnvironmentFolder);

            BuildSceneDressingPrefab(NaturePackGreetingDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "NatureGreetingTreeLeft", $"{NaturePackRoot}/Nature/Models_trees/Tree4.prefab", new Vector3(-10.5f, 0f, 12.5f), new Vector3(0f, 25f, 0f), Vector3.one * 0.82f);
                AddVendorPrefab(root, "NatureGreetingTreeRight", $"{NaturePackRoot}/Nature/Models_trees/Tree6.prefab", new Vector3(10.3f, 0f, 13.2f), new Vector3(0f, -35f, 0f), Vector3.one * 0.78f);
                AddVendorPrefab(root, "NatureGreetingTreeBack", $"{NaturePackRoot}/Nature/Models_trees/Tree11.prefab", new Vector3(0f, 0f, 17.5f), new Vector3(0f, 12f, 0f), Vector3.one * 0.7f);
                AddVendorPrefab(root, "NatureGreetingBushLeft", $"{NaturePackRoot}/Nature/Models_trees/Bush2.prefab", new Vector3(-6.4f, 0f, 7.2f), new Vector3(0f, 45f, 0f), Vector3.one * 0.9f);
                AddVendorPrefab(root, "NatureGreetingBushRight", $"{NaturePackRoot}/Nature/Models_trees/Bush5.prefab", new Vector3(6.3f, 0f, 7.4f), new Vector3(0f, -25f, 0f), Vector3.one * 0.82f);
                AddVendorPrefab(root, "NatureGreetingRockLeft", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr3.prefab", new Vector3(-4.7f, 0f, 4.8f), new Vector3(0f, 35f, 0f), Vector3.one * 0.48f);
                AddVendorPrefab(root, "NatureGreetingRockRight", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr4.prefab", new Vector3(4.6f, 0f, 4.9f), new Vector3(0f, -30f, 0f), Vector3.one * 0.45f);
                DisableColliders(root.gameObject);
            });

            BuildSceneDressingPrefab(NaturePackHerdDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "NatureHerdTreeFarLeft", $"{NaturePackRoot}/Nature/Models_trees/Tree12.prefab", new Vector3(-13.5f, 0f, 14.5f), new Vector3(0f, 20f, 0f), Vector3.one * 0.82f);
                AddVendorPrefab(root, "NatureHerdTreeFarRight", $"{NaturePackRoot}/Nature/Models_trees/Tree13.prefab", new Vector3(13.2f, 0f, 14.2f), new Vector3(0f, -25f, 0f), Vector3.one * 0.8f);
                AddVendorPrefab(root, "NatureHerdTreeBackLeft", $"{NaturePackRoot}/Nature/Models_trees/Tree7.prefab", new Vector3(-7.5f, 0f, 17.5f), new Vector3(0f, 8f, 0f), Vector3.one * 0.74f);
                AddVendorPrefab(root, "NatureHerdTreeBackRight", $"{NaturePackRoot}/Nature/Models_trees/Tree8.prefab", new Vector3(7.6f, 0f, 17.7f), new Vector3(0f, -12f, 0f), Vector3.one * 0.72f);
                AddVendorPrefab(root, "NatureHerdBushLeft", $"{NaturePackRoot}/Nature/Models_trees/Bush1.prefab", new Vector3(-8.2f, 0f, 8.6f), new Vector3(0f, 35f, 0f), Vector3.one * 0.75f);
                AddVendorPrefab(root, "NatureHerdBushRight", $"{NaturePackRoot}/Nature/Models_trees/Bush4.prefab", new Vector3(8.4f, 0f, 8.7f), new Vector3(0f, -35f, 0f), Vector3.one * 0.75f);
                AddVendorPrefab(root, "NatureHerdRocksLeft", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr7.prefab", new Vector3(-7.4f, 0f, 3.6f), new Vector3(0f, 25f, 0f), Vector3.one * 0.42f);
                AddVendorPrefab(root, "NatureHerdRocksRight", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr8.prefab", new Vector3(7.2f, 0f, 3.7f), new Vector3(0f, -20f, 0f), Vector3.one * 0.42f);
                DisableColliders(root.gameObject);
            });

            BuildSceneDressingPrefab(NaturePackScanDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "NatureScanTreeLeft", $"{NaturePackRoot}/Nature/Models_trees/Tree3.prefab", new Vector3(-9.4f, 0f, 12.6f), new Vector3(0f, 18f, 0f), Vector3.one * 0.74f);
                AddVendorPrefab(root, "NatureScanTreeRight", $"{NaturePackRoot}/Nature/Models_trees/Tree5.prefab", new Vector3(9.5f, 0f, 12.9f), new Vector3(0f, -18f, 0f), Vector3.one * 0.72f);
                AddVendorPrefab(root, "NatureScanBushLeft", $"{NaturePackRoot}/Nature/Models_trees/Bush3.prefab", new Vector3(-5.8f, 0f, 6.8f), new Vector3(0f, 30f, 0f), Vector3.one * 0.7f);
                AddVendorPrefab(root, "NatureScanBushRight", $"{NaturePackRoot}/Nature/Models_trees/Bush6.prefab", new Vector3(5.9f, 0f, 6.9f), new Vector3(0f, -28f, 0f), Vector3.one * 0.68f);
                AddVendorPrefab(root, "NatureScanRock", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr2.prefab", new Vector3(6.2f, 0f, 3.4f), new Vector3(0f, 15f, 0f), Vector3.one * 0.36f);
                DisableColliders(root.gameObject);
            });

            BuildSceneDressingPrefab(NaturePackResultsDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "NatureResultsTreeLeft", $"{NaturePackRoot}/Nature/Models_trees/Tree14.prefab", new Vector3(-10.2f, 0f, 12.8f), new Vector3(0f, 30f, 0f), Vector3.one * 0.78f);
                AddVendorPrefab(root, "NatureResultsTreeRight", $"{NaturePackRoot}/Nature/Models_trees/Tree15.prefab", new Vector3(10.4f, 0f, 12.9f), new Vector3(0f, -32f, 0f), Vector3.one * 0.78f);
                AddVendorPrefab(root, "NatureResultsTreeBack", $"{NaturePackRoot}/Nature/Models_trees/Tree16.prefab", new Vector3(0f, 0f, 16.8f), new Vector3(0f, 5f, 0f), Vector3.one * 0.66f);
                AddVendorPrefab(root, "NatureResultsBushLeft", $"{NaturePackRoot}/Nature/Models_trees/Bush1.prefab", new Vector3(-6.6f, 0f, 7.4f), new Vector3(0f, 22f, 0f), Vector3.one * 0.78f);
                AddVendorPrefab(root, "NatureResultsBushRight", $"{NaturePackRoot}/Nature/Models_trees/Bush2.prefab", new Vector3(6.7f, 0f, 7.5f), new Vector3(0f, -22f, 0f), Vector3.one * 0.78f);
                AddVendorPrefab(root, "NatureResultsRocks", $"{NaturePackRoot}/Stones/Stones group/Prefabs (to use in editor)/prefab_stone_gr1.prefab", new Vector3(0f, 0f, 3.2f), new Vector3(0f, 12f, 0f), Vector3.one * 0.32f);
                DisableColliders(root.gameObject);
            });
        }

        private static void ApplyNaturePackDressingsToScenes()
        {
            ApplyDressingToScene(GreetingScenePath, "GreetingEnvironment", NaturePackGreetingDressingPrefabPath, "NaturePack2Dressing");
            ApplyDressingToScene(HerdScenePath, "HerdEnvironment", NaturePackHerdDressingPrefabPath, "NaturePack2Dressing");
            ApplyDressingToScene(CowScanScenePath, "CowScanEnvironment", NaturePackScanDressingPrefabPath, "NaturePack2Dressing");
            ApplyDressingToScene(ResultsScenePath, "ResultsEnvironment", NaturePackResultsDressingPrefabPath, "NaturePack2Dressing");
        }

        private static void ConfigureAutoHandProcedureObjects()
        {
            var grabbableType = FindType("Autohand.Grabbable");
            var placePointType = FindType("Autohand.PlacePoint");
            if (grabbableType == null)
            {
                Debug.LogWarning("AutoHand Grabbable type was not found. Skipping AutoHand S04 integration.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(AIProcedureScenePath, OpenSceneMode.Single);
            var tool = FindSceneObjectByName("AIProcedureTool");
            if (tool != null)
            {
                ConfigureAutoHandGrabbable(tool, grabbableType);
            }
            else
            {
                Debug.LogWarning("Could not find AIProcedureTool in S04 for AutoHand setup.");
            }

            var placementZone = FindSceneObjectByName("PlacementZone");
            if (placementZone != null && placePointType != null)
            {
                ConfigureAutoHandPlacePoint(placementZone, placePointType);
            }

            var finalTrigger = FindSceneObjectByName("FinalDeliveryTrigger");
            if (finalTrigger != null)
            {
                ConfigureAutoHandFinalTrigger(finalTrigger);
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ConfigureAutoHandGrabbable(GameObject tool, System.Type grabbableType)
        {
            var rigidBody = tool.GetComponent<Rigidbody>();
            if (rigidBody == null)
            {
                rigidBody = tool.AddComponent<Rigidbody>();
            }

            rigidBody.mass = 0.25f;
            rigidBody.linearDamping = 0.35f;
            rigidBody.angularDamping = 0.8f;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
            EditorUtility.SetDirty(rigidBody);

            if (tool.GetComponent(grabbableType) == null)
            {
                tool.AddComponent(grabbableType);
            }

            var grabbable = tool.GetComponent(grabbableType);
            SetPublicField(grabbable, "singleHandOnly", true);
            SetPublicField(grabbable, "instantGrab", false);
            SetPublicField(grabbable, "useGentleGrab", true);
            SetPublicField(grabbable, "parentOnGrab", true);
            SetPublicField(grabbable, "heldNoFriction", true);
            SetPublicField(grabbable, "ignoreWeight", true);
            SetPublicField(grabbable, "throwPower", 0.1f);
            SetPublicField(grabbable, "jointBreakForce", 1800f);
            SetPublicField(grabbable, "makeChildrenGrabbable", true);
            SetPublicField(grabbable, "grabPriorityWeight", 2.5f);
            SetPublicField(grabbable, "ignoreReleaseTime", 0.15f);
            SetPublicField(grabbable, "minHeldDrag", 2f);
            SetPublicField(grabbable, "minHeldAngleDrag", 4f);
            SetPublicField(grabbable, "minHeldMass", 0.1f);
            SetPublicField(grabbable, "maxHeldVelocity", 3.5f);
            SetPublicField(grabbable, "heldPositionOffset", new Vector3(0f, 0f, 0.02f));
            SetPublicField(grabbable, "heldRotationOffset", new Vector3(0f, 0f, 0f));
            EditorUtility.SetDirty(grabbable);
        }

        private static void ConfigureAutoHandPlacePoint(GameObject placementZone, System.Type placePointType)
        {
            if (placementZone.GetComponent(placePointType) == null)
            {
                placementZone.AddComponent(placePointType);
            }

            var placePoint = placementZone.GetComponent(placePointType);
            SetPublicField(placePoint, "ignoreMe", false);
            SetPublicField(placePoint, "placeRadius", 0.45f);
            SetPublicField(placePoint, "placeSize", new Vector3(0.6f, 0.6f, 1.1f));
            SetPublicField(placePoint, "shapeOffset", Vector3.zero);
            SetPublicField(placePoint, "grabbablePlacePoint", false);
            SetPublicField(placePoint, "forcePlace", false);
            SetPublicField(placePoint, "parentOnPlace", false);
            SetPublicField(placePoint, "matchPosition", false);
            SetPublicField(placePoint, "matchRotation", false);
            SetPublicField(placePoint, "resizeOnPlace", false);
            SetPublicField(placePoint, "disableRigidbodyOnPlace", false);
            SetPublicField(placePoint, "disableGrabOnPlace", false);
            SetPublicField(placePoint, "disablePlacePointOnPlace", false);
            SetPublicField(placePoint, "makePlacedKinematic", false);
            SetPublicField(placePoint, "heldPlaceOnly", false);
            EditorUtility.SetDirty(placePoint);
        }

        private static void ConfigureAutoHandFinalTrigger(GameObject finalTrigger)
        {
            var collider = finalTrigger.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = true;
                EditorUtility.SetDirty(collider);
            }
        }

        private static void UpgradeNaturePackMaterialsForUrp()
        {
            if (!AssetDatabase.IsValidFolder(NaturePackRoot))
            {
                Debug.LogWarning($"Nature Pack 2 folder was not found at '{NaturePackRoot}'.");
                return;
            }

            var litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null)
            {
                Debug.LogWarning("Could not find 'Universal Render Pipeline/Lit'. Nature Pack 2 materials were not upgraded.");
                return;
            }

            var materialGuids = AssetDatabase.FindAssets("t:Material", new[]
            {
                $"{NaturePackRoot}/Nature",
                $"{NaturePackRoot}/Stones",
                $"{NaturePackRoot}/Terrain"
            });

            foreach (var guid in materialGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    continue;
                }

                var baseMap = GetFirstTexture(material, "_BaseMap", "_MainTex", "_BaseColorMap");
                var normalMap = GetFirstTexture(material, "_BumpMap", "_NormalMap");
                var baseColor = GetFirstColor(material, Color.white, "_BaseColor", "_Color");
                var isFoliage = path.Contains("/Materials_trees/Tree_l", System.StringComparison.OrdinalIgnoreCase) ||
                    path.Contains("/Materials_trees/Bush_", System.StringComparison.OrdinalIgnoreCase);

                material.shader = litShader;
                SetTextureIfPresent(material, "_BaseMap", baseMap);
                SetTextureIfPresent(material, "_BumpMap", normalMap);
                SetColorIfPresent(material, "_BaseColor", baseColor);
                SetFloatIfPresent(material, "_Metallic", 0f);
                SetFloatIfPresent(material, "_Smoothness", isFoliage ? 0.08f : 0.25f);

                if (isFoliage)
                {
                    SetFloatIfPresent(material, "_AlphaClip", 1f);
                    SetFloatIfPresent(material, "_Cutoff", 0.35f);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.renderQueue = (int)RenderQueue.AlphaTest;
                    material.SetOverrideTag("RenderType", "TransparentCutout");
                    SetColorIfPresent(material, "_BaseColor", new Color(baseColor.r, baseColor.g, baseColor.b, 1f));
                }

                EditorUtility.SetDirty(material);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void UpgradeNaturePackTreePrefabMaterialsForUrp()
        {
            if (!AssetDatabase.IsValidFolder($"{NaturePackRoot}/Nature/Models_trees"))
            {
                Debug.LogWarning("Nature Pack 2 tree model folder was not found.");
                return;
            }

            var litShader = Shader.Find("Universal Render Pipeline/Lit");
            if (litShader == null)
            {
                Debug.LogWarning("Could not find 'Universal Render Pipeline/Lit'. Nature Pack 2 prefab materials were not upgraded.");
                return;
            }

            var prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { $"{NaturePackRoot}/Nature/Models_trees" });
            foreach (var guid in prefabGuids)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(guid);
                var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
                var changed = false;

                var prefabName = Path.GetFileNameWithoutExtension(prefabPath);
                var diffuseTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{NaturePackRoot}/Nature/Models_trees/{prefabName}_Textures/diffuse.png");
                var normalTexture = AssetDatabase.LoadAssetAtPath<Texture2D>($"{NaturePackRoot}/Nature/Models_trees/{prefabName}_Textures/normal_specular.png");

                foreach (var renderer in prefabRoot.GetComponentsInChildren<Renderer>(true))
                {
                    var materials = renderer.sharedMaterials;
                    for (var index = 0; index < materials.Length; index++)
                    {
                        var material = materials[index];
                        if (material == null)
                        {
                            continue;
                        }

                        var materialName = material.name;
                        var isLeaf = materialName.Contains("Leaf", System.StringComparison.OrdinalIgnoreCase) ||
                            materialName.Contains("Tree_l", System.StringComparison.OrdinalIgnoreCase) ||
                            materialName.Contains("Bush", System.StringComparison.OrdinalIgnoreCase);
                        var isBark = materialName.Contains("Bark", System.StringComparison.OrdinalIgnoreCase) ||
                            materialName.Contains("Tree_bark", System.StringComparison.OrdinalIgnoreCase);

                        if (!isLeaf && !isBark)
                        {
                            continue;
                        }

                        var existingBaseMap = GetFirstTexture(material, "_BaseMap", "_MainTex", "_TranslucencyMap");
                        var existingNormalMap = GetFirstTexture(material, "_BumpMap", "_NormalMap");

                        material.shader = litShader;
                        SetTextureIfPresent(material, "_BaseMap", existingBaseMap != null ? existingBaseMap : diffuseTexture);
                        SetTextureIfPresent(material, "_BumpMap", existingNormalMap != null ? existingNormalMap : normalTexture);
                        SetColorIfPresent(material, "_BaseColor", Color.white);
                        SetFloatIfPresent(material, "_Metallic", 0f);
                        SetFloatIfPresent(material, "_Smoothness", isLeaf ? 0.08f : 0.22f);

                        if (isLeaf)
                        {
                            SetFloatIfPresent(material, "_AlphaClip", 1f);
                            SetFloatIfPresent(material, "_Cutoff", 0.35f);
                            SetFloatIfPresent(material, "_Cull", 0f);
                            material.EnableKeyword("_ALPHATEST_ON");
                            material.renderQueue = (int)RenderQueue.AlphaTest;
                            material.SetOverrideTag("RenderType", "TransparentCutout");
                        }

                        EditorUtility.SetDirty(material);
                        changed = true;
                    }

                    renderer.sharedMaterials = materials;
                    EditorUtility.SetDirty(renderer);
                }

                if (changed)
                {
                    PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
                }

                PrefabUtility.UnloadPrefabContents(prefabRoot);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void ReplaceGameplayCowVisualsWithAnimalPack()
        {
            ReplaceCowVisualsInScene(HerdScenePath, new[]
            {
                ("Cow_A", AnimalPackCow1PrefabPath),
                ("Cow_B", AnimalPackCow2PrefabPath),
                ("Cow_C", AnimalPackCow3PrefabPath)
            });

            ReplaceCowVisualsInScene(CowScanScenePath, new[]
            {
                ("ScanCow", AnimalPackCow2PrefabPath)
            });
        }

        private static void ReplaceCowVisualsInScene(string scenePath, (string RootName, string VisualPrefabPath)[] replacements)
        {
            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var changed = false;

            foreach (var replacement in replacements)
            {
                var cowRoot = FindSceneObjectByName(replacement.RootName);
                if (cowRoot == null)
                {
                    Debug.LogWarning($"Could not find cow root '{replacement.RootName}' in scene '{scenePath}'.");
                    continue;
                }

                if (ReplaceCowVisual(cowRoot, replacement.VisualPrefabPath))
                {
                    changed = true;
                }
            }

            if (changed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        private static bool ReplaceCowVisual(GameObject cowRoot, string visualPrefabPath)
        {
            var visualPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(visualPrefabPath);
            if (visualPrefab == null)
            {
                Debug.LogWarning($"Could not load replacement cow visual '{visualPrefabPath}'.");
                return false;
            }

            var oldVisual = cowRoot.transform.Find("Visual");
            var boundsSource = oldVisual != null ? oldVisual : cowRoot.transform;
            var hasTargetBounds = TryCalculateRendererBounds(boundsSource, out var targetBounds);

            if (oldVisual != null)
            {
                Object.DestroyImmediate(oldVisual.gameObject);
            }

            var visual = PrefabUtility.InstantiatePrefab(visualPrefab) as GameObject;
            if (visual == null)
            {
                return false;
            }

            visual.name = "Visual";
            visual.transform.SetParent(cowRoot.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = Vector3.one;
            DisableColliders(visual);

            if (hasTargetBounds && TryCalculateRendererBounds(visual.transform, out var replacementBounds))
            {
                var targetHeight = Mathf.Max(0.01f, targetBounds.size.y);
                var replacementHeight = Mathf.Max(0.01f, replacementBounds.size.y);
                var scale = Mathf.Clamp(targetHeight / replacementHeight, 0.05f, 20f);
                visual.transform.localScale *= scale;

                if (TryCalculateRendererBounds(visual.transform, out replacementBounds))
                {
                    var offset = new Vector3(
                        targetBounds.center.x - replacementBounds.center.x,
                        targetBounds.min.y - replacementBounds.min.y,
                        targetBounds.center.z - replacementBounds.center.z);
                    visual.transform.position += offset;
                }
            }

            RefreshCowInteractionCollider(cowRoot);
            RefreshCowTargetRenderer(cowRoot);
            return true;
        }

        private static void RefreshCowInteractionCollider(GameObject cowRoot)
        {
            if (!TryCalculateRendererBounds(cowRoot.transform.Find("Visual") ?? cowRoot.transform, out var bounds))
            {
                return;
            }

            var collider = cowRoot.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = cowRoot.AddComponent<BoxCollider>();
            }

            var localBounds = TransformBoundsToLocal(cowRoot.transform, bounds);
            collider.center = localBounds.center;
            collider.size = localBounds.size;
            collider.isTrigger = false;
            EditorUtility.SetDirty(collider);
        }

        private static void RefreshCowTargetRenderer(GameObject cowRoot)
        {
            var selectionTarget = cowRoot.GetComponent<CowSelectionTarget>();
            if (selectionTarget == null)
            {
                return;
            }

            var visual = cowRoot.transform.Find("Visual");
            var renderer = visual != null
                ? visual.GetComponentInChildren<Renderer>(true)
                : cowRoot.GetComponentInChildren<Renderer>(true);

            if (renderer != null)
            {
                SetPrivateObject(selectionTarget, "targetRenderer", renderer);
                EditorUtility.SetDirty(selectionTarget);
            }
        }

        private static GameObject FindSceneObjectByName(string objectName)
        {
            var activeScene = EditorSceneManager.GetActiveScene();
            foreach (var root in activeScene.GetRootGameObjects())
            {
                var found = FindChildByName(root.transform, objectName);
                if (found != null)
                {
                    return found.gameObject;
                }
            }

            return null;
        }

        private static Transform FindChildByName(Transform root, string objectName)
        {
            if (root.name == objectName)
            {
                return root;
            }

            for (var index = 0; index < root.childCount; index++)
            {
                var found = FindChildByName(root.GetChild(index), objectName);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static bool TryCalculateRendererBounds(Transform target, out Bounds bounds)
        {
            var renderers = target.GetComponentsInChildren<Renderer>(true);
            var hasBounds = false;
            bounds = new Bounds(target.position, Vector3.one);

            foreach (var renderer in renderers)
            {
                if (renderer.GetComponent<TextMeshPro>() != null)
                {
                    continue;
                }

                if (!hasBounds)
                {
                    bounds = renderer.bounds;
                    hasBounds = true;
                }
                else
                {
                    bounds.Encapsulate(renderer.bounds);
                }
            }

            return hasBounds;
        }

        private static Bounds TransformBoundsToLocal(Transform localRoot, Bounds worldBounds)
        {
            var center = worldBounds.center;
            var extents = worldBounds.extents;
            var corners = new[]
            {
                center + new Vector3(extents.x, extents.y, extents.z),
                center + new Vector3(extents.x, extents.y, -extents.z),
                center + new Vector3(extents.x, -extents.y, extents.z),
                center + new Vector3(extents.x, -extents.y, -extents.z),
                center + new Vector3(-extents.x, extents.y, extents.z),
                center + new Vector3(-extents.x, extents.y, -extents.z),
                center + new Vector3(-extents.x, -extents.y, extents.z),
                center + new Vector3(-extents.x, -extents.y, -extents.z)
            };

            var localBounds = new Bounds(localRoot.InverseTransformPoint(corners[0]), Vector3.zero);
            for (var index = 1; index < corners.Length; index++)
            {
                localBounds.Encapsulate(localRoot.InverseTransformPoint(corners[index]));
            }

            return localBounds;
        }

        private static void ApplyGroundMaterialsToCurrentScenes()
        {
            var grass = AssetDatabase.LoadAssetAtPath<Material>(GroundGrassMaterialPath);
            var dirt = AssetDatabase.LoadAssetAtPath<Material>(GroundDirtMaterialPath);
            var indoor = AssetDatabase.LoadAssetAtPath<Material>(GroundIndoorMaterialPath);

            ApplyGroundMaterialToScene(GreetingScenePath, grass);
            ApplyGroundMaterialToScene(HerdScenePath, grass);
            ApplyGroundMaterialToScene(CowScanScenePath, grass);
            ApplyGroundMaterialToScene(ResultsScenePath, grass);
            ApplyGroundMaterialToScene(AIProcedureScenePath, dirt != null ? dirt : indoor);
            ApplyGroundMaterialToScene(ValidationScenePath, indoor != null ? indoor : dirt);
        }

        private static void ApplyGroundMaterialToScene(string scenePath, Material material)
        {
            if (material == null)
            {
                Debug.LogWarning($"Could not apply ground material to '{scenePath}' because material was not found.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            foreach (var root in scene.GetRootGameObjects())
            {
                foreach (var ground in root.GetComponentsInChildren<Transform>(true))
                {
                    if (ground.name != "Ground")
                    {
                        continue;
                    }

                    var renderer = ground.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sharedMaterial = material;
                    }
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ApplySkyboxToScene(string scenePath, Material skyboxMaterial)
        {
            if (skyboxMaterial == null)
            {
                Debug.LogWarning($"Could not apply skybox to '{scenePath}' because material was not found.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1.05f;
            RenderSettings.reflectionIntensity = 0.72f;
            DynamicGI.UpdateEnvironment();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void ApplyDressingToScene(string scenePath, string environmentRootName, string dressingPrefabPath)
        {
            ApplyDressingToScene(scenePath, environmentRootName, dressingPrefabPath, "ToonFarmDressing");
        }

        private static void ApplyDressingToScene(string scenePath, string environmentRootName, string dressingPrefabPath, string instanceName)
        {
            var dressingPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(dressingPrefabPath);
            if (dressingPrefab == null)
            {
                Debug.LogWarning($"Could not load dressing prefab at '{dressingPrefabPath}'.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            var environmentRoot = GameObject.Find(environmentRootName);
            if (environmentRoot == null)
            {
                Debug.LogWarning($"Could not find environment root '{environmentRootName}' in scene '{scenePath}'.");
                return;
            }

            var existing = environmentRoot.transform.Find(instanceName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            var instance = PrefabUtility.InstantiatePrefab(dressingPrefab) as GameObject;
            if (instance == null)
            {
                return;
            }

            instance.name = instanceName;
            instance.transform.SetParent(environmentRoot.transform, false);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        private static void BuildGreetingToonFarmDressing()
        {
            BuildSceneDressingPrefab(GreetingDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "GreetingDirtPath", $"{ToonFarmRoot}/Ground/TFP_Dirt_Road_Straight_02A.prefab", new Vector3(0f, 0.02f, 4.2f), new Vector3(0f, 90f, 0f), new Vector3(1.35f, 1f, 1.35f));
                AddVendorPrefab(root, "GreetingCutoutA", $"{ToonFarmRoot}/Background/TFP_Cutout_01A.prefab", new Vector3(-16f, 0f, 24f), Vector3.zero, new Vector3(2.6f, 2.4f, 2.6f));
                AddVendorPrefab(root, "GreetingCutoutC", $"{ToonFarmRoot}/Background/TFP_Cutout_01C.prefab", new Vector3(16f, 0f, 24f), Vector3.zero, new Vector3(2.6f, 2.4f, 2.6f));
                AddVendorPrefab(root, "GreetingTreeLeft", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_02A.prefab", new Vector3(-8f, 0f, 13f), Vector3.zero, Vector3.one * 1.2f);
                AddVendorPrefab(root, "GreetingTreeRight", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_03A.prefab", new Vector3(8f, 0f, 13f), Vector3.zero, Vector3.one * 1.15f);
                AddVendorPrefab(root, "GreetingBushLeft", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Bush_01A.prefab", new Vector3(-5.2f, 0f, 7.5f), Vector3.zero, Vector3.one * 1.2f);
                AddVendorPrefab(root, "GreetingBushRight", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Bush_01A.prefab", new Vector3(5.4f, 0f, 7.2f), new Vector3(0f, 140f, 0f), Vector3.one * 1.15f);
                AddVendorPrefab(root, "GreetingFenceL", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(-6.8f, 0f, 8.8f), new Vector3(0f, 25f, 0f), Vector3.one);
                AddVendorPrefab(root, "GreetingFenceR", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(6.8f, 0f, 8.8f), new Vector3(0f, -25f, 0f), Vector3.one);
                AddVendorPrefab(root, "GreetingGrassA", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_03A.prefab", new Vector3(-2.8f, 0f, 4.6f), Vector3.zero, Vector3.one * 1.25f);
                AddVendorPrefab(root, "GreetingGrassB", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_05A.prefab", new Vector3(2.9f, 0f, 4.4f), new Vector3(0f, 35f, 0f), Vector3.one * 1.2f);
            });
        }

        private static void BuildHerdToonFarmDressing()
        {
            BuildSceneDressingPrefab(HerdDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "PaddockDirtPathA", $"{ToonFarmRoot}/Ground/TFP_Dirt_Road_Straight_02A.prefab", new Vector3(-2.8f, 0.02f, 4.2f), new Vector3(0f, 82f, 0f), new Vector3(1.45f, 1f, 1.25f));
                AddVendorPrefab(root, "PaddockDirtPathB", $"{ToonFarmRoot}/Ground/TFP_Dirt_Road_Curve_01A.prefab", new Vector3(3.6f, 0.02f, 4.8f), new Vector3(0f, -22f, 0f), new Vector3(1.25f, 1f, 1.25f));
                AddVendorPrefab(root, "PaddockCutoutA", $"{ToonFarmRoot}/Background/TFP_Cutout_01A.prefab", new Vector3(-18f, 0f, 26f), Vector3.zero, new Vector3(3f, 2.8f, 3f));
                AddVendorPrefab(root, "PaddockCutoutB", $"{ToonFarmRoot}/Background/TFP_Cutout_01B.prefab", new Vector3(18f, 0f, 26f), Vector3.zero, new Vector3(3f, 2.8f, 3f));
                AddVendorPrefab(root, "FenceBackL", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_4.prefab", new Vector3(-5.5f, 0f, 8.5f), Vector3.zero, Vector3.one);
                AddVendorPrefab(root, "FenceBackC", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_4.prefab", new Vector3(0f, 0f, 8.5f), Vector3.zero, Vector3.one);
                AddVendorPrefab(root, "FenceBackR", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_4.prefab", new Vector3(5.5f, 0f, 8.5f), Vector3.zero, Vector3.one);
                AddVendorPrefab(root, "FenceLeftA", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(-7.2f, 0f, 5.6f), new Vector3(0f, 90f, 0f), Vector3.one);
                AddVendorPrefab(root, "FenceRightA", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(7.2f, 0f, 5.6f), new Vector3(0f, 90f, 0f), Vector3.one);
                AddVendorPrefab(root, "TroughLeft", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Trough_06A.prefab", new Vector3(-4.5f, 0f, 6.8f), new Vector3(0f, 25f, 0f), Vector3.one);
                AddVendorPrefab(root, "TroughRight", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Tire_Trough_03A.prefab", new Vector3(4.4f, 0f, 6.9f), new Vector3(0f, -20f, 0f), Vector3.one);
                AddVendorPrefab(root, "HayLeft", $"{ToonFarmRoot}/Vegetation/Agricultural Plants/TFP_Hay_Patch_01B.prefab", new Vector3(-5.4f, 0f, 3.8f), Vector3.zero, Vector3.one * 1.2f);
                AddVendorPrefab(root, "HayRight", $"{ToonFarmRoot}/Vegetation/Agricultural Plants/TFP_Hay_Patch_02B.prefab", new Vector3(5.2f, 0f, 3.6f), new Vector3(0f, 45f, 0f), Vector3.one * 1.2f);
                AddVendorPrefab(root, "TreeLeft", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_02A.prefab", new Vector3(-10f, 0f, 12.5f), Vector3.zero, Vector3.one * 1.4f);
                AddVendorPrefab(root, "TreeRight", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_03A.prefab", new Vector3(10.2f, 0f, 12.8f), Vector3.zero, Vector3.one * 1.35f);
                AddVendorPrefab(root, "GrassClusterL", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_04A.prefab", new Vector3(-3.2f, 0f, 2.5f), Vector3.zero, Vector3.one * 1.5f);
                AddVendorPrefab(root, "GrassClusterR", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_06A.prefab", new Vector3(2.8f, 0f, 2.2f), new Vector3(0f, 55f, 0f), Vector3.one * 1.45f);
                AddVendorPrefab(root, "RocksLeft", $"{ToonFarmRoot}/Vegetation/Rocks/TFP_Rocks_01A.prefab", new Vector3(-6.4f, 0f, 2.4f), Vector3.zero, Vector3.one * 1.05f);
                AddVendorPrefab(root, "RocksRight", $"{ToonFarmRoot}/Vegetation/Rocks/TFP_Rocks_02A.prefab", new Vector3(6.2f, 0f, 2.3f), new Vector3(0f, 25f, 0f), Vector3.one * 1.05f);
            });
        }

        private static void BuildScanToonFarmDressing()
        {
            BuildSceneDressingPrefab(ScanDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "ScanDirtPath", $"{ToonFarmRoot}/Ground/TFP_Dirt_Road_Straight_02A.prefab", new Vector3(0f, 0.02f, 4.9f), new Vector3(0f, 90f, 0f), new Vector3(1.2f, 1f, 1.05f));
                AddVendorPrefab(root, "ScanCutoutA", $"{ToonFarmRoot}/Background/TFP_Cutout_01C.prefab", new Vector3(-12f, 0f, 22f), Vector3.zero, new Vector3(2.4f, 2.4f, 2.4f));
                AddVendorPrefab(root, "ScanCutoutB", $"{ToonFarmRoot}/Background/TFP_Cutout_01B.prefab", new Vector3(12f, 0f, 22f), Vector3.zero, new Vector3(2.4f, 2.4f, 2.4f));
                AddVendorPrefab(root, "ScanFenceL", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(-4.8f, 0f, 6.7f), new Vector3(0f, 65f, 0f), Vector3.one);
                AddVendorPrefab(root, "ScanFenceR", $"{ToonFarmRoot}/Props/Fence Kits/TFP_Fence_04A_Module_2.prefab", new Vector3(4.8f, 0f, 6.7f), new Vector3(0f, -65f, 0f), Vector3.one);
                AddVendorPrefab(root, "ScanTrough", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Trough_08A.prefab", new Vector3(3.8f, 0f, 5.6f), new Vector3(0f, -18f, 0f), Vector3.one);
                AddVendorPrefab(root, "ScanHay", $"{ToonFarmRoot}/Vegetation/Agricultural Plants/TFP_Hay_Patch_01B.prefab", new Vector3(-3.9f, 0f, 4.5f), new Vector3(0f, 30f, 0f), Vector3.one * 1.1f);
                AddVendorPrefab(root, "ScanRocks", $"{ToonFarmRoot}/Vegetation/Rocks/TFP_Rocks_03A.prefab", new Vector3(5.5f, 0f, 3.1f), Vector3.zero, Vector3.one * 0.9f);
            });
        }

        private static void BuildProcedureToonFarmDressing()
        {
            BuildSceneDressingPrefab(ProcedureDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "ProcedureDirtScuff", $"{ToonFarmRoot}/Ground/TFP_Decal_Debris_01A.prefab", new Vector3(0f, 0.02f, 4.8f), new Vector3(0f, 25f, 0f), new Vector3(1.4f, 1f, 1.4f));
                AddVendorPrefab(root, "ProcedureBucketA", $"{ToonFarmRoot}/Props/Farming Props/TFP_Bucket_Metal_01A.prefab", new Vector3(-2.6f, 0f, 4.4f), Vector3.zero, Vector3.one);
                AddVendorPrefab(root, "ProcedureBucketB", $"{ToonFarmRoot}/Props/Farming Props/TFP_Plastic_Bucket_02A.prefab", new Vector3(2.9f, 0f, 4.15f), new Vector3(0f, 35f, 0f), Vector3.one);
                AddVendorPrefab(root, "ProcedureHose", $"{ToonFarmRoot}/Props/Farming Props/TFP_Hose_02A.prefab", new Vector3(-2.2f, 0f, 5.3f), new Vector3(0f, 25f, 0f), Vector3.one * 0.95f);
                AddVendorPrefab(root, "ProcedureBarrel", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Wood_Barrel_Open_04A.prefab", new Vector3(2.5f, 0f, 5.75f), new Vector3(0f, -18f, 0f), Vector3.one);
                AddVendorPrefab(root, "ProcedureMetalBarrel", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Metal_Barrel_01B.prefab", new Vector3(-2.8f, 0f, 6.1f), Vector3.zero, Vector3.one);
                AddVendorPrefab(root, "ProcedureWoodStack", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Wood_Stack_03A.prefab", new Vector3(2.9f, 0f, 6.5f), new Vector3(0f, 22f, 0f), Vector3.one * 1.1f);
                AddVendorPrefab(root, "ProcedurePlank", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Wooden_Plank_02A.prefab", new Vector3(1.7f, 0f, 3.8f), new Vector3(0f, 90f, 0f), Vector3.one);
            });
        }

        private static void BuildValidationToonFarmDressing()
        {
            BuildSceneDressingPrefab(ValidationDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "ValidationFloorDebris", $"{ToonFarmRoot}/Ground/TFP_Decal_Debris_02A.prefab", new Vector3(0.8f, 0.02f, 4.6f), new Vector3(0f, -35f, 0f), new Vector3(1.15f, 1f, 1.15f));
                AddVendorPrefab(root, "ValidationSign", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Signpost_Small_01A.prefab", new Vector3(-2.8f, 0f, 4.9f), new Vector3(0f, 30f, 0f), Vector3.one);
                AddVendorPrefab(root, "ValidationPlanks", $"{ToonFarmRoot}/Props/Exterior Props/TFP_Wooden_Plank_03A.prefab", new Vector3(2.5f, 0f, 4.6f), new Vector3(0f, -20f, 0f), Vector3.one);
                AddVendorPrefab(root, "ValidationBucket", $"{ToonFarmRoot}/Props/Farming Props/TFP_Bucket_Metal_01B.prefab", new Vector3(2.15f, 0f, 4.15f), Vector3.zero, Vector3.one);
            });
        }

        private static void BuildResultsToonFarmDressing()
        {
            BuildSceneDressingPrefab(ResultsDressingPrefabPath, root =>
            {
                AddVendorPrefab(root, "ResultsDirtPath", $"{ToonFarmRoot}/Ground/TFP_Dirt_Road_Curve_01A.prefab", new Vector3(0f, 0.02f, 4.4f), new Vector3(0f, 10f, 0f), new Vector3(1.35f, 1f, 1.35f));
                AddVendorPrefab(root, "ResultsCutoutA", $"{ToonFarmRoot}/Background/TFP_Cutout_01A.prefab", new Vector3(-15f, 0f, 22f), Vector3.zero, new Vector3(2.6f, 2.5f, 2.6f));
                AddVendorPrefab(root, "ResultsCutoutC", $"{ToonFarmRoot}/Background/TFP_Cutout_01C.prefab", new Vector3(15f, 0f, 22f), Vector3.zero, new Vector3(2.6f, 2.5f, 2.6f));
                AddVendorPrefab(root, "ResultsTreeLeft", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_01A.prefab", new Vector3(-8.6f, 0f, 10.6f), Vector3.zero, Vector3.one * 1.2f);
                AddVendorPrefab(root, "ResultsTreeRight", $"{ToonFarmRoot}/Vegetation/Trees/TFP_Beech_Tree_03D.prefab", new Vector3(8.8f, 0f, 10.9f), Vector3.zero, Vector3.one * 1.2f);
                AddVendorPrefab(root, "ResultsGrassL", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_03A.prefab", new Vector3(-3.8f, 0f, 3.6f), Vector3.zero, Vector3.one * 1.35f);
                AddVendorPrefab(root, "ResultsGrassR", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Grass_Patch_05A.prefab", new Vector3(3.9f, 0f, 3.6f), new Vector3(0f, 45f, 0f), Vector3.one * 1.35f);
                AddVendorPrefab(root, "ResultsFlowersL", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Flowers_Patch_01B.prefab", new Vector3(-5.6f, 0f, 4.4f), Vector3.zero, Vector3.one * 1.15f);
                AddVendorPrefab(root, "ResultsFlowersR", $"{ToonFarmRoot}/Vegetation/Grass/TFP_Flowers_Patch_02B.prefab", new Vector3(5.4f, 0f, 4.3f), new Vector3(0f, 20f, 0f), Vector3.one * 1.1f);
                AddVendorPrefab(root, "ResultsRocks", $"{ToonFarmRoot}/Vegetation/Rocks/TFP_Rocks_02B.prefab", new Vector3(0f, 0f, 3.1f), Vector3.zero, Vector3.one);
            });
        }

        private static void BuildSceneDressingPrefab(string prefabPath, System.Action<Transform> populate)
        {
            var root = new GameObject(Path.GetFileNameWithoutExtension(prefabPath));
            populate(root.transform);
            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            Object.DestroyImmediate(root);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }
        }

        private static void BuildAnimalWrapper(string sourcePrefabPath, string outputPrefabPath, string wrapperName, Vector3 visualScale)
        {
            var sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePrefabPath);
            if (sourcePrefab == null)
            {
                Debug.LogWarning($"Could not load licensed animal prefab '{sourcePrefabPath}'.");
                return;
            }

            var root = new GameObject(wrapperName);
            var visual = PrefabUtility.InstantiatePrefab(sourcePrefab) as GameObject;
            if (visual == null)
            {
                Object.DestroyImmediate(root);
                return;
            }

            visual.name = "Visual";
            visual.transform.SetParent(root.transform, false);
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.identity;
            visual.transform.localScale = visualScale;

            DisableColliders(root);

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(root, outputPrefabPath);
            Object.DestroyImmediate(root);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }
        }

        private static GameObject AddVendorPrefab(Transform parent, string name, string assetPath, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab == null)
            {
                Debug.LogWarning($"Could not load Toon Farm prefab '{assetPath}'.");
                return null;
            }

            var instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            if (instance == null)
            {
                return null;
            }

            instance.name = name;
            instance.transform.SetParent(parent, false);
            instance.transform.localPosition = localPosition;
            instance.transform.localRotation = Quaternion.Euler(localEulerAngles);
            instance.transform.localScale = localScale;
            return instance;
        }

        private static GameObject AddProjectPrefab(Transform parent, string name, string assetPath, Vector3 localPosition, Vector3 localEulerAngles, Vector3 localScale)
        {
            return AddVendorPrefab(parent, name, assetPath, localPosition, localEulerAngles, localScale);
        }

        private static void DisableColliders(GameObject root)
        {
            var colliders = root.GetComponentsInChildren<Collider>(true);
            for (var index = 0; index < colliders.Length; index++)
            {
                colliders[index].enabled = false;
                EditorUtility.SetDirty(colliders[index]);
            }
        }

        private static EditorBuildSettingsScene BuildScene(string sceneId)
        {
            return new EditorBuildSettingsScene($"{ScenesFolder}/{sceneId}.unity", true);
        }

        private static void CreateLighting()
        {
            var lightObject = new GameObject("Directional Light");
            var lightTransform = lightObject.transform;
            lightTransform.position = new Vector3(0f, 3f, 0f);
            lightTransform.rotation = Quaternion.Euler(50f, -30f, 0f);
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.color = new Color(1f, 0.96f, 0.9f, 1f);
            light.shadows = LightShadows.Soft;
        }

        private static void ApplySharedSceneRenderingSettings()
        {
            var skyboxMaterial = AssetDatabase.LoadAssetAtPath<Material>(DaySkyboxPath);
            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
            }

            RenderSettings.ambientMode = AmbientMode.Skybox;
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.reflectionIntensity = 0.7f;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogColor = new Color(0.76f, 0.82f, 0.88f, 1f);
            RenderSettings.fogStartDistance = 18f;
            RenderSettings.fogEndDistance = 42f;

            var globalVolumeProfile = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GlobalVolumeProfilePath);
            if (globalVolumeProfile != null)
            {
                var volumeObject = new GameObject("GlobalVolume");
                var volume = volumeObject.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.priority = 1f;
                volume.sharedProfile = globalVolumeProfile;
            }
        }

        private static void CreateAppRoot()
        {
            var appRoot = new GameObject("AppRoot");
            appRoot.AddComponent<AppBootstrap>();
            appRoot.AddComponent<SceneLoader>();
        }

        private static void CreateXrOrigin()
        {
            var xrOriginPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(XrOriginPrefabPath);
            if (xrOriginPrefab == null)
            {
                Debug.LogWarning($"Could not load XR Origin prefab at '{XrOriginPrefabPath}'.");
                return;
            }

            var instance = PrefabUtility.InstantiatePrefab(xrOriginPrefab) as GameObject;
            if (instance != null)
            {
                instance.name = "XR Origin (VR)";
                instance.transform.position = Vector3.zero;
                instance.AddComponent<VeterinarVR.Gameplay.HandTrackingSetup>();
            }
        }

        private static void CreateGreetingEnvironment()
        {
            var environmentRoot = new GameObject("GreetingEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3f, 1f, 3f);

            var sign = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sign.name = "WelcomeSign";
            sign.transform.SetParent(environmentRoot.transform);
            sign.transform.position = new Vector3(0f, 2f, 6f);
            sign.transform.localScale = new Vector3(3.5f, 1.6f, 0.2f);
        }

        private static void CreateHerdEnvironment()
        {
            var environmentRoot = new GameObject("HerdEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(4f, 1f, 4f);

            var cowA = CreateCowPlaceholder(environmentRoot.transform, "Cow_A", new Vector3(-2f, 0f, 5f), new Color(0.55f, 0.45f, 0.35f));
            var cowB = CreateCowPlaceholder(environmentRoot.transform, "Cow_B", new Vector3(0f, 0f, 5.5f), new Color(0.65f, 0.55f, 0.42f));
            var cowC = CreateCowPlaceholder(environmentRoot.transform, "Cow_C", new Vector3(2f, 0f, 5f), new Color(0.45f, 0.38f, 0.3f));

            ConfigureCowWorldTarget(cowA, "Cow_A");
            ConfigureCowWorldTarget(cowB, "Cow_B");
            ConfigureCowWorldTarget(cowC, "Cow_C");
            CreateWorldLocalizedLabel(cowA.transform, "CowALabel", "Cow A", "Lembu A", GetLabelAnchor(cowA, 0.25f), new Vector2(280f, 70f), 28);
            CreateWorldLocalizedLabel(cowB.transform, "CowBLabel", "Cow B", "Lembu B", GetLabelAnchor(cowB, 0.25f), new Vector2(280f, 70f), 28);
            CreateWorldLocalizedLabel(cowC.transform, "CowCLabel", "Cow C", "Lembu C", GetLabelAnchor(cowC, 0.25f), new Vector2(280f, 70f), 28);
        }

        private static void CreateCowScanEnvironment()
        {
            var environmentRoot = new GameObject("CowScanEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3.5f, 1f, 3.5f);

            var scanCow = CreateCowPlaceholder(environmentRoot.transform, "ScanCow", new Vector3(0f, 0f, 5f), new Color(0.65f, 0.55f, 0.42f));

            var vulvaTarget = new GameObject("VulvaTarget");
            vulvaTarget.transform.SetParent(scanCow.transform, false);
            vulvaTarget.transform.localPosition = new Vector3(0f, -0.35f, 0.9f);

            var vulvaRuler = vulvaTarget.AddComponent<VeterinarVR.Gameplay.VulvaRuler>();
            var vulvaCollider = vulvaTarget.AddComponent<SphereCollider>();
            vulvaCollider.radius = 0.15f;
            vulvaCollider.isTrigger = true;

            var vulvaRenderer = vulvaTarget.AddComponent<MeshRenderer>();
            var vulvaFilter = vulvaTarget.AddComponent<MeshFilter>();
            vulvaFilter.sharedMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            if (vulvaFilter.sharedMesh == null)
            {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                vulvaFilter.sharedMesh = sphere.GetComponent<MeshFilter>().sharedMesh;
                Object.DestroyImmediate(sphere);
            }

            var vulvaMaterial = new Material(Shader.Find("Standard"));
            vulvaMaterial.color = new Color(0.9f, 0.35f, 0.55f, 0.6f);
            vulvaRenderer.sharedMaterial = vulvaMaterial;
            vulvaTarget.transform.localScale = Vector3.one * 0.22f;

            CreateWorldLocalizedLabel(vulvaTarget.transform, "VulvaLabel", "Vulva", "Vulva", new Vector3(0f, 0.2f, 0f), new Vector2(200f, 50f), 20);

            CreateWorldLocalizedLabel(scanCow.transform, "ScanCowLabel", "Scan Target", "Sasaran Imbasan", GetLabelAnchor(scanCow, 0.35f), new Vector2(340f, 72f), 28);
        }

        private static void CreateAIProcedureEnvironment()
        {
            var environmentRoot = new GameObject("AIProcedureEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3.5f, 1f, 3.5f);

            CreateProcedureChute(environmentRoot.transform);

            var tray = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tray.name = "InstrumentTray";
            tray.transform.SetParent(environmentRoot.transform);
            tray.transform.position = new Vector3(-1.7f, 0.9f, 4f);
            tray.transform.localScale = new Vector3(1.2f, 0.15f, 0.8f);

            CreateProcedureTool(environmentRoot.transform);
            CreatePlacementZone(environmentRoot.transform);
            CreateFinalDeliveryTrigger(environmentRoot.transform);
        }

        private static void CreateValidationEnvironment()
        {
            var environmentRoot = new GameObject("ValidationEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3.2f, 1f, 3.2f);

            var dashboard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            dashboard.name = "DashboardStand";
            dashboard.transform.SetParent(environmentRoot.transform);
            dashboard.transform.position = new Vector3(0f, 1.2f, 5.4f);
            dashboard.transform.localScale = new Vector3(3.4f, 2f, 0.2f);
        }

        private static void CreateResultsEnvironment()
        {
            var environmentRoot = new GameObject("ResultsEnvironment");

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(environmentRoot.transform);
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(3.2f, 1f, 3.2f);

            var backdrop = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backdrop.name = "ResultsBackdrop";
            backdrop.transform.SetParent(environmentRoot.transform);
            backdrop.transform.position = new Vector3(0f, 1.4f, 5.6f);
            backdrop.transform.localScale = new Vector3(3.6f, 2.4f, 0.2f);
        }

        private static void CreateProcedureTool(Transform parent)
        {
            var tool = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tool.name = "AIProcedureTool";
            tool.transform.SetParent(parent);
            tool.transform.position = new Vector3(-1.7f, 1.05f, 4f);
            tool.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            tool.transform.localScale = new Vector3(0.08f, 0.45f, 0.08f);

            var rigidBody = tool.AddComponent<Rigidbody>();
            rigidBody.mass = 0.35f;
            rigidBody.angularDamping = 0.8f;

            var grabInteractable = tool.AddComponent<XRGrabInteractable>();
            grabInteractable.trackPosition = true;
            grabInteractable.trackRotation = true;

            tool.AddComponent<AIProcedureToolTracker>();

            var renderer = tool.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = new Color(0.78f, 0.82f, 0.88f, 1f);
            }

            var tip = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tip.name = "ToolTip";
            tip.transform.SetParent(tool.transform, false);
            tip.transform.localPosition = new Vector3(0f, 0f, 0.55f);
            tip.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);

            var tipRenderer = tip.GetComponent<Renderer>();
            if (tipRenderer != null)
            {
                tipRenderer.sharedMaterial.color = new Color(0.92f, 0.62f, 0.26f, 1f);
            }

            var tipCollider = tip.GetComponent<Collider>();
            if (tipCollider != null)
            {
                tipCollider.isTrigger = false;
            }
        }

        private static void CreateProcedureChute(Transform parent)
        {
            var chutePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(ChutePrefabPath);
            if (chutePrefab != null)
            {
                var chuteInstance = PrefabUtility.InstantiatePrefab(chutePrefab) as GameObject;
                if (chuteInstance != null)
                {
                    chuteInstance.name = "ProcedureChute";
                    chuteInstance.transform.SetParent(parent);
                    chuteInstance.transform.position = new Vector3(0f, 0f, 5.2f);
                    chuteInstance.transform.rotation = Quaternion.identity;
                    chuteInstance.transform.localScale = Vector3.one;
                    return;
                }
            }

            var fallbackChute = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fallbackChute.name = "ProcedureChute";
            fallbackChute.transform.SetParent(parent);
            fallbackChute.transform.position = new Vector3(0f, 1.2f, 5.2f);
            fallbackChute.transform.localScale = new Vector3(2.2f, 2.2f, 1.2f);
        }

        private static void CreatePlacementZone(Transform parent)
        {
            var zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zone.name = "PlacementZone";
            zone.transform.SetParent(parent);
            zone.transform.position = new Vector3(0.75f, 1.05f, 4.85f);
            zone.transform.rotation = Quaternion.Euler(0f, 0f, 25f);
            zone.transform.localScale = new Vector3(0.3f, 0.3f, 0.8f);

            var collider = zone.GetComponent<BoxCollider>();
            if (collider != null)
            {
                collider.isTrigger = true;
            }

            var zoneScript = zone.AddComponent<AIProcedurePlacementZone>();
            var snapPoint = new GameObject("SnapPoint");
            snapPoint.transform.SetParent(zone.transform, false);
            snapPoint.transform.localPosition = Vector3.zero;
            snapPoint.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            snapPoint.transform.localScale = Vector3.one;

            SetPrivateObject(zoneScript, "snapPoint", snapPoint.transform);
            SetPrivateObject(zoneScript, "zoneRenderer", zone.GetComponent<Renderer>());

            var renderer = zone.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = new Color(0.22f, 0.33f, 0.42f, 1f);
            }
        }

        private static void CreateFinalDeliveryTrigger(Transform parent)
        {
            var trigger = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trigger.name = "FinalDeliveryTrigger";
            trigger.transform.SetParent(parent);
            trigger.transform.position = new Vector3(1.85f, 0.2f, 4.2f);
            trigger.transform.localScale = new Vector3(0.3f, 0.12f, 0.3f);

            var renderer = trigger.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.sharedMaterial.color = new Color(0.2f, 0.24f, 0.3f, 1f);
            }

            var finalTrigger = trigger.AddComponent<AIProcedureFinalStepTrigger>();
            SetPrivateObject(finalTrigger, "targetRenderer", renderer);

            CreateWorldLocalizedLabel(trigger.transform, "FinalDeliveryLabel", "Deliver Dose", "Hantar Dos", new Vector3(0f, 0.45f, 0f), new Vector2(260f, 60f), 22);
        }

        private static void CreateGreetingCanvas()
        {
            var uiRoot = new GameObject("GreetingUI");
            var controller = uiRoot.AddComponent<GreetingSceneController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.55f, 2.2f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);

            var canvasRect = canvasObject.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.12f, 0.18f, 0.25f, 0.92f);

            CreateLocalizedLabel("Title", panel.transform, "SMART RUMINANT BREEDING LAB", "MAKMAL PEMBIAKAN RUMINAN PINTAR", 72, new Vector2(0f, 250f), new Vector2(900f, 110f));
            CreateLocalizedLabel("Subtitle", panel.transform, "Choose your language to begin the VR training flow.", "Pilih bahasa anda untuk memulakan aliran latihan VR.", 34, new Vector2(0f, 155f), new Vector2(920f, 80f));

            var englishButton = CreateLocalizedButton("EnglishButton", panel.transform, "English", "Bahasa Inggeris", new Vector2(-180f, 10f), new Vector2(280f, 100f));
            var bahasaButton = CreateLocalizedButton("BahasaButton", panel.transform, "Bahasa Melayu", "Bahasa Melayu", new Vector2(180f, 10f), new Vector2(280f, 100f));
            var proceedButton = CreateLocalizedButton("ProceedButton", panel.transform, "Proceed", "Teruskan", new Vector2(0f, -180f), new Vector2(320f, 110f));

            var englishSelected = CreateLocalizedLabel("EnglishSelected", panel.transform, "English selected", "Bahasa Inggeris dipilih", 28, new Vector2(-180f, -80f), new Vector2(260f, 60f));
            var bahasaSelected = CreateLocalizedLabel("BahasaSelected", panel.transform, "Bahasa Melayu selected", "Bahasa Melayu dipilih", 28, new Vector2(180f, -80f), new Vector2(320f, 60f));

            proceedButton.gameObject.SetActive(false);
            englishSelected.gameObject.SetActive(false);
            bahasaSelected.gameObject.SetActive(false);

            UnityEventTools.AddPersistentListener(englishButton.onClick, controller.SelectEnglish);
            UnityEventTools.AddPersistentListener(bahasaButton.onClick, controller.SelectBahasaMelayu);
            UnityEventTools.AddPersistentListener(proceedButton.onClick, controller.ProceedToScene02);

            SetPrivateObject(controller, "proceedButtonRoot", proceedButton.gameObject);
            SetPrivateObject(controller, "englishSelectedState", englishSelected.gameObject);
            SetPrivateObject(controller, "bahasaSelectedState", bahasaSelected.gameObject);
        }

        private static void CreateEventSystem()
        {
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(XRUIInputModule));
            eventSystemObject.GetComponent<EventSystem>().sendNavigationEvents = true;
        }

        private static void CreateHerdCanvas()
        {
            var uiRoot = new GameObject("HerdUI");
            var controller = uiRoot.AddComponent<HerdObservationController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.5f, 2.1f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.15f, 0.2f, 0.16f, 0.92f);

            CreateProgressStrip(panel.transform, SceneIds.HerdObservation);

            CreateLocalizedLabel("Title", panel.transform, "Which cow shows the clearest heat signs?", "Lembu manakah menunjukkan tanda birahi paling jelas?", 58, new Vector2(0f, 250f), new Vector2(980f, 110f));
            CreateLocalizedLabel("Subtitle", panel.transform, "Select the best candidate to continue.", "Pilih calon terbaik untuk meneruskan.", 30, new Vector2(0f, 175f), new Vector2(900f, 70f));

            var feedback = CreateLabel("Feedback", panel.transform, string.Empty, 30, new Vector2(0f, -70f), new Vector2(900f, 80f));
            CreateLocalizedLabel("WorldHint", panel.transform, "Point at a cow in the paddock to make your selection.", "Halakan pada lembu di kawasan ragut untuk membuat pilihan anda.", 24, new Vector2(0f, 55f), new Vector2(920f, 60f));

            var proceedButton = CreateLocalizedButton("ProceedButton", panel.transform, "Continue to Scan", "Teruskan ke Imbasan", new Vector2(0f, -200f), new Vector2(340f, 110f));
            proceedButton.gameObject.SetActive(false);
            UnityEventTools.AddPersistentListener(proceedButton.onClick, controller.ProceedToResults);

            SetPrivateObject(controller, "correctCowId", "Cow_B");
            SetPrivateObject(controller, "proceedButtonRoot", proceedButton.gameObject);
            SetPrivateObject(controller, "feedbackLabel", feedback);
        }

        private static void CreateResultsCanvas()
        {
            var appRoot = GameObject.Find("AppRoot");
            if (appRoot != null && appRoot.GetComponent<SceneLoader>() == null)
            {
                appRoot.AddComponent<SceneLoader>();
            }

            var uiRoot = new GameObject("ResultsUI");
            var controller = uiRoot.AddComponent<ResultsPanelController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.55f, 2.2f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.12f, 0.16f, 0.22f, 0.92f);

            CreateProgressStrip(panel.transform, SceneIds.ResultsScoreboard);

            CreateLocalizedLabel("Title", panel.transform, "Session Results", "Keputusan Sesi", 64, new Vector2(0f, 250f), new Vector2(900f, 100f));
            var selectedCow = CreateLabel("SelectedCow", panel.transform, "Selected Cow: None", 34, new Vector2(0f, 165f), new Vector2(700f, 60f));
            var finding = CreateLabel("Finding", panel.transform, "Finding: Pending", 30, new Vector2(0f, 100f), new Vector2(700f, 56f));
            var procedure = CreateLabel("Procedure", panel.transform, "Procedure: Not started", 28, new Vector2(0f, 40f), new Vector2(780f, 56f));
            var score = CreateLabel("Score", panel.transform, "Score: 0", 38, new Vector2(0f, -25f), new Vector2(500f, 64f));
            var correct = CreateLabel("Correct", panel.transform, "Correct: 0", 28, new Vector2(-160f, -90f), new Vector2(280f, 56f));
            var wrong = CreateLabel("Wrong", panel.transform, "Wrong: 0", 28, new Vector2(160f, -90f), new Vector2(280f, 56f));
            var decision = CreateLabel("Decision", panel.transform, "Decision: Pending", 30, new Vector2(0f, -150f), new Vector2(560f, 56f));
            var outcome = CreateLabel("Outcome", panel.transform, "Outcome: Not Assessed", 28, new Vector2(0f, -205f), new Vector2(620f, 52f));
            var badge = CreateLabel("Badge", panel.transform, "Badge: -", 28, new Vector2(0f, -252f), new Vector2(480f, 52f));
            var recommendation = CreateLabel("Recommendation", panel.transform, "Complete all AI procedure steps before final validation.", 24, new Vector2(0f, -315f), new Vector2(920f, 88f));
            var restart = CreateLocalizedButton("RestartButton", panel.transform, "Restart", "Mula Semula", new Vector2(0f, -410f), new Vector2(300f, 92f));

            UnityEventTools.AddPersistentListener(restart.onClick, controller.RestartExperience);

            SetPrivateObject(controller, "resultsRoot", panel);
            SetPrivateObject(controller, "selectedCowLabel", selectedCow);
            SetPrivateObject(controller, "findingLabel", finding);
            SetPrivateObject(controller, "procedureLabel", procedure);
            SetPrivateObject(controller, "scoreLabel", score);
            SetPrivateObject(controller, "correctLabel", correct);
            SetPrivateObject(controller, "wrongLabel", wrong);
            SetPrivateObject(controller, "decisionLabel", decision);
            SetPrivateObject(controller, "outcomeLabel", outcome);
            SetPrivateObject(controller, "badgeLabel", badge);
            SetPrivateObject(controller, "recommendationLabel", recommendation);
        }

        private static void CreateCowScanCanvas()
        {
            var uiRoot = new GameObject("CowScanUI");
            var controller = uiRoot.AddComponent<CowScanController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.55f, 2.15f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.17f, 0.14f, 0.2f, 0.92f);

            CreateProgressStrip(panel.transform, SceneIds.CowScanDecision);

            CreateLocalizedLabel("Title", panel.transform, "Cow Reproductive Evaluation", "Penilaian Reproduktif Lembu", 58, new Vector2(0f, 260f), new Vector2(1000f, 100f));
            var selectedCow = CreateLabel("SelectedCow", panel.transform, "Selected Cow: None", 30, new Vector2(0f, 170f), new Vector2(700f, 60f));
            var status = CreateLabel("Status", panel.transform, "Pinch thumb & index finger near the vulva to measure", 26, new Vector2(0f, 100f), new Vector2(950f, 70f));
            CreateLocalizedLabel("Hint", panel.transform, "Bring both hands near the cow's vulva, pinch and spread fingers to measure.", "Dekatkan kedua-dua tangan ke vulva lembu, cubit dan renggang jari untuk mengukur.", 22, new Vector2(0f, 30f), new Vector2(960f, 60f));

            var measurement = CreateLabel("Measurement", panel.transform, string.Empty, 32, new Vector2(0f, -30f), new Vector2(800f, 60f));
            var feedback = CreateLabel("Feedback", panel.transform, string.Empty, 26, new Vector2(0f, -100f), new Vector2(940f, 60f));
            var proceedButton = CreateLocalizedButton("ProceedButton", panel.transform, "Continue Procedure", "Teruskan Prosedur", new Vector2(0f, -220f), new Vector2(360f, 100f));

            proceedButton.gameObject.SetActive(false);

            UnityEventTools.AddPersistentListener(proceedButton.onClick, controller.ProceedToResults);

            SetPrivateObject(controller, "selectedCowLabel", selectedCow);
            SetPrivateObject(controller, "statusLabel", status);
            SetPrivateObject(controller, "feedbackLabel", feedback);
            SetPrivateObject(controller, "measurementLabel", measurement);
            SetPrivateObject(controller, "proceedButtonRoot", proceedButton.gameObject);

            var vulvaTarget = GameObject.Find("VulvaTarget");
            if (vulvaTarget != null)
            {
                SetPrivateObject(controller, "vulvaRulerObject", vulvaTarget);
            }
        }

        private static void CreateAIProcedureCanvas()
        {
            var uiRoot = new GameObject("AIProcedureUI");
            var controller = uiRoot.AddComponent<AIProcedureController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.55f, 2.15f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.18f, 0.13f, 0.12f, 0.92f);

            CreateProgressStrip(panel.transform, SceneIds.AIProcedure);

            CreateLocalizedLabel("Title", panel.transform, "AI Procedure", "Prosedur AI", 62, new Vector2(0f, 260f), new Vector2(900f, 100f));
            var step = CreateLabel("Step", panel.transform, "Press Start Procedure", 34, new Vector2(0f, 160f), new Vector2(920f, 70f));
            var status = CreateLabel("Status", panel.transform, "Prepare the AI procedure station", 28, new Vector2(0f, 95f), new Vector2(920f, 60f));
            var beginButton = CreateLocalizedButton("BeginProcedureButton", panel.transform, "Start Procedure", "Mula Prosedur", new Vector2(0f, 10f), new Vector2(340f, 100f));
            CreateLocalizedLabel("WorldHint", panel.transform, "Move the tool from the tray, place it in the target zone, then activate Deliver Dose in the scene.", "Gerakkan alat dari dulang, letakkan di zon sasaran, kemudian aktifkan Hantar Dos dalam babak.", 24, new Vector2(0f, -55f), new Vector2(980f, 60f));
            var feedback = CreateLabel("Feedback", panel.transform, string.Empty, 28, new Vector2(0f, -235f), new Vector2(940f, 80f));
            var proceedButton = CreateLocalizedButton("ProceedButton", panel.transform, "Open Dashboard", "Buka Papan Pemuka", new Vector2(0f, -285f), new Vector2(340f, 100f));

            proceedButton.gameObject.SetActive(false);

            UnityEventTools.AddPersistentListener(beginButton.onClick, controller.StartProcedure);
            UnityEventTools.AddPersistentListener(proceedButton.onClick, controller.ProceedToDashboard);

            SetPrivateObject(controller, "stepLabel", step);
            SetPrivateObject(controller, "statusLabel", status);
            SetPrivateObject(controller, "feedbackLabel", feedback);
            SetPrivateObject(controller, "beginButtonRoot", beginButton.gameObject);
            SetPrivateObject(controller, "proceedButtonRoot", proceedButton.gameObject);
            SetPrivateStringArray(controller, "procedureSteps", new[]
            {
                "Prepare the catheter and straw",
                "Position the insemination gun in the target zone",
                "Deliver the semen dose correctly"
            });
            SetPrivateStringArray(controller, "bahasaProcedureSteps", new[]
            {
                "Sediakan kateter dan straw",
                "Posisikan gun inseminasi di zon sasaran",
                "Hantar dos semen dengan betul"
            });
        }

        private static void CreateValidationCanvas()
        {
            var uiRoot = new GameObject("ValidationUI");
            var controller = uiRoot.AddComponent<ValidationDashboardController>();

            var canvasObject = new GameObject("WorldSpaceCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(TrackedDeviceGraphicRaycaster));
            canvasObject.transform.SetParent(uiRoot.transform);
            canvasObject.transform.position = new Vector3(0f, 1.55f, 2.15f);
            canvasObject.transform.localScale = Vector3.one * 0.0018f;

            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            var scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1200f, 900f);
            canvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2(1200f, 900f);

            var panel = CreateUiObject("Panel", canvasObject.transform, typeof(Image));
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.08f, 0.08f);
            panelRect.anchorMax = new Vector2(0.92f, 0.92f);
            panelRect.offsetMin = Vector2.zero;
            panelRect.offsetMax = Vector2.zero;
            panel.GetComponent<Image>().color = new Color(0.1f, 0.17f, 0.2f, 0.92f);

            CreateProgressStrip(panel.transform, SceneIds.ValidationDashboard);

            CreateLocalizedLabel("Title", panel.transform, "Validation Dashboard", "Papan Pemuka Pengesahan", 62, new Vector2(0f, 260f), new Vector2(920f, 100f));
            var selectedCow = CreateLabel("SelectedCow", panel.transform, "Selected Cow: None", 30, new Vector2(0f, 190f), new Vector2(740f, 60f));
            var finding = CreateLabel("Finding", panel.transform, "Finding: Pending", 28, new Vector2(0f, 130f), new Vector2(720f, 56f));
            var procedure = CreateLabel("Procedure", panel.transform, "Procedure: 0/0 steps", 28, new Vector2(0f, 70f), new Vector2(780f, 56f));
            var score = CreateLabel("Score", panel.transform, "Score: 0", 34, new Vector2(0f, 10f), new Vector2(500f, 60f));
            var decision = CreateLabel("Decision", panel.transform, "Decision: Pending", 30, new Vector2(0f, -50f), new Vector2(620f, 60f));
            var status = CreateLabel("Status", panel.transform, "Review the session record and choose the breeding status.", 26, new Vector2(0f, -115f), new Vector2(940f, 70f));
            var bredButton = CreateLocalizedButton("BredButton", panel.transform, "Mark Bred", "Tanda Bunting", new Vector2(-170f, -205f), new Vector2(280f, 100f));
            var notBredButton = CreateLocalizedButton("NotBredButton", panel.transform, "Mark Not Bred", "Tanda Tidak Bunting", new Vector2(170f, -205f), new Vector2(320f, 100f));
            var proceedButton = CreateLocalizedButton("ProceedButton", panel.transform, "View Final Results", "Lihat Keputusan Akhir", new Vector2(0f, -325f), new Vector2(360f, 96f));

            proceedButton.gameObject.SetActive(false);

            UnityEventTools.AddPersistentListener(bredButton.onClick, controller.MarkBred);
            UnityEventTools.AddPersistentListener(notBredButton.onClick, controller.MarkNotBred);
            UnityEventTools.AddPersistentListener(proceedButton.onClick, controller.ProceedToResults);

            SetPrivateObject(controller, "selectedCowLabel", selectedCow);
            SetPrivateObject(controller, "selectedFindingLabel", finding);
            SetPrivateObject(controller, "procedureStatusLabel", procedure);
            SetPrivateObject(controller, "scoreLabel", score);
            SetPrivateObject(controller, "decisionLabel", decision);
            SetPrivateObject(controller, "statusLabel", status);
            SetPrivateObject(controller, "proceedButtonRoot", proceedButton.gameObject);
        }

        private static GameObject CreateUiObject(string name, Transform parent, params System.Type[] components)
        {
            var gameObject = new GameObject(name, components);
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static Text CreateLabel(string name, Transform parent, string textValue, int fontSize, Vector2 anchoredPosition, Vector2 size)
        {
            var label = CreateUiObject(name, parent, typeof(Text));
            var rect = label.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var text = label.GetComponent<Text>();
            text.text = textValue;
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.color = Color.white;
            return text;
        }

        private static Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPosition, Vector2 size)
        {
            var buttonObject = CreateUiObject(name, parent, typeof(Image), typeof(Button));
            var rect = buttonObject.GetComponent<RectTransform>();
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPosition;

            var image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.85f, 0.91f, 0.97f, 1f);

            var text = CreateLabel("Label", buttonObject.transform, label, 32, Vector2.zero, size);
            text.color = new Color(0.08f, 0.12f, 0.18f, 1f);

            return buttonObject.GetComponent<Button>();
        }

        private static Button CreateCowChoice(Transform parent, string name, string label, Vector2 anchoredPosition)
        {
            return CreateButton(name, parent, label, anchoredPosition, new Vector2(220f, 90f));
        }

        private static Text CreateLocalizedLabel(string name, Transform parent, string englishText, string bahasaText, int fontSize, Vector2 anchoredPosition, Vector2 size)
        {
            var text = CreateLabel(name, parent, englishText, fontSize, anchoredPosition, size);
            AttachLocalizedText(text, englishText, bahasaText);
            return text;
        }

        private static Button CreateLocalizedButton(string name, Transform parent, string englishText, string bahasaText, Vector2 anchoredPosition, Vector2 size)
        {
            var button = CreateButton(name, parent, englishText, anchoredPosition, size);
            var label = button.GetComponentInChildren<Text>();
            if (label != null)
            {
                AttachLocalizedText(label, englishText, bahasaText);
            }

            return button;
        }

        private static void CreateProgressStrip(Transform parent, string currentSceneId)
        {
            var root = CreateUiObject("ProgressStrip", parent, typeof(Image));
            var rootRect = root.GetComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.08f, 0.83f);
            rootRect.anchorMax = new Vector2(0.92f, 0.93f);
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;

            var rootImage = root.GetComponent<Image>();
            rootImage.color = new Color(1f, 1f, 1f, 0.06f);

            var currentIndex = GetProgressIndex(currentSceneId);
            var stepWidth = 180f;
            var spacing = 14f;
            var totalWidth = (ProgressLabels.Length * stepWidth) + ((ProgressLabels.Length - 1) * spacing);
            var startX = -(totalWidth * 0.5f) + (stepWidth * 0.5f);

            for (var index = 0; index < ProgressLabels.Length; index++)
            {
                var isCompleted = index < currentIndex;
                var isCurrent = index == currentIndex;
                var step = CreateUiObject($"Step{index + 2:00}", root.transform, typeof(Image));
                var stepRect = step.GetComponent<RectTransform>();
                stepRect.sizeDelta = new Vector2(stepWidth, 58f);
                stepRect.anchorMin = new Vector2(0.5f, 0.5f);
                stepRect.anchorMax = new Vector2(0.5f, 0.5f);
                stepRect.anchoredPosition = new Vector2(startX + (index * (stepWidth + spacing)), 0f);

                var stepImage = step.GetComponent<Image>();
                stepImage.color = isCurrent
                    ? new Color(0.86f, 0.67f, 0.22f, 0.95f)
                    : isCompleted
                        ? new Color(0.22f, 0.56f, 0.38f, 0.92f)
                        : new Color(0.22f, 0.26f, 0.32f, 0.92f);

                var number = CreateLabel("Number", step.transform, (index + 2).ToString(), 18, new Vector2(-58f, 0f), new Vector2(34f, 34f));
                number.color = isCurrent || isCompleted ? Color.white : new Color(0.82f, 0.87f, 0.92f, 0.9f);

                var stepLabel = CreateLocalizedLabel("Label", step.transform, ProgressLabels[index].English, ProgressLabels[index].Bahasa, 20, new Vector2(18f, 0f), new Vector2(120f, 42f));
                stepLabel.alignment = TextAnchor.MiddleLeft;
                stepLabel.color = isCurrent || isCompleted ? Color.white : new Color(0.82f, 0.87f, 0.92f, 0.9f);
            }
        }

        private static int GetProgressIndex(string sceneId)
        {
            for (var index = 0; index < ProgressSceneIds.Length; index++)
            {
                if (ProgressSceneIds[index] == sceneId)
                {
                    return index;
                }
            }

            return 0;
        }

        private static GameObject CreateCowPlaceholder(Transform parent, string name, Vector3 position, Color color)
        {
            var cowPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(CowPrefabPath);
            if (cowPrefab != null)
            {
                var cowInstance = PrefabUtility.InstantiatePrefab(cowPrefab) as GameObject;
                if (cowInstance != null)
                {
                    cowInstance.name = name;
                    cowInstance.transform.SetParent(parent);
                    cowInstance.transform.position = position;
                    cowInstance.transform.rotation = Quaternion.identity;
                    cowInstance.transform.localScale = Vector3.one;
                    return cowInstance;
                }
            }

            var cow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cow.name = name;
            cow.transform.SetParent(parent);
            cow.transform.position = position + new Vector3(0f, 0.75f, 0f);
            cow.transform.localScale = new Vector3(1.2f, 1.2f, 2f);

            var renderer = cow.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = color;
                renderer.sharedMaterial = material;
            }

            return cow;
        }

        private static void ConfigureCowWorldTarget(GameObject cow, string cowId)
        {
            var target = cow.AddComponent<CowSelectionTarget>();
            SetPrivateObject(target, "cowId", cowId);
            SetPrivateObject(target, "requireExplicitConfirmation", false);
            SetPrivateObject(target, "targetRenderer", cow.GetComponentInChildren<Renderer>());
        }

        private static Vector3 GetLabelAnchor(GameObject target, float yPadding)
        {
            var bounds = CalculateCombinedBounds(target);
            var worldAnchor = new Vector3(bounds.center.x, bounds.max.y + yPadding, bounds.center.z);
            return target.transform.InverseTransformPoint(worldAnchor);
        }

        private static Bounds CalculateCombinedBounds(GameObject target)
        {
            var renderers = target.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0)
            {
                return new Bounds(target.transform.position, Vector3.one);
            }

            var bounds = renderers[0].bounds;
            for (var index = 1; index < renderers.Length; index++)
            {
                bounds.Encapsulate(renderers[index].bounds);
            }

            return bounds;
        }

        private static UniversalRendererData EnsureUrpRendererData()
        {
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRendererData>(UrpRendererPath);
            if (existing != null)
            {
                return existing;
            }

            var createRendererAsset = typeof(UniversalRenderPipelineAsset).GetMethod(
                "CreateRendererAsset",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);

            var created = createRendererAsset?.Invoke(null, new object[] { UrpRendererPath, RendererType.UniversalRenderer, false, "Renderer" }) as UniversalRendererData;
            if (created == null)
            {
                throw new System.InvalidOperationException("Failed to create URP renderer data asset.");
            }

            return created;
        }

        private static UniversalRenderPipelineAsset EnsureUrpAsset(UniversalRendererData rendererData)
        {
            var existing = AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>(UrpAssetPath);
            if (existing != null)
            {
                return existing;
            }

            var asset = UniversalRenderPipelineAsset.Create(rendererData);
            AssetDatabase.CreateAsset(asset, UrpAssetPath);
            return asset;
        }

        private static VolumeProfile EnsureGlobalVolumeProfile()
        {
            var existing = AssetDatabase.LoadAssetAtPath<VolumeProfile>(GlobalVolumeProfilePath);
            if (existing == null)
            {
                existing = ScriptableObject.CreateInstance<VolumeProfile>();
                AssetDatabase.CreateAsset(existing, GlobalVolumeProfilePath);
            }

            existing.components.RemoveAll(component => component == null);

            var bloom = EnsureVolumeComponent<Bloom>(existing);
            bloom.active = true;
            bloom.threshold.overrideState = true;
            bloom.threshold.value = 0.95f;
            bloom.intensity.overrideState = true;
            bloom.intensity.value = 0.2f;
            bloom.scatter.overrideState = true;
            bloom.scatter.value = 0.62f;
            bloom.highQualityFiltering.overrideState = true;
            bloom.highQualityFiltering.value = false;

            var colorAdjustments = EnsureVolumeComponent<ColorAdjustments>(existing);
            colorAdjustments.active = true;
            colorAdjustments.postExposure.overrideState = true;
            colorAdjustments.postExposure.value = 0.08f;
            colorAdjustments.contrast.overrideState = true;
            colorAdjustments.contrast.value = 12f;
            colorAdjustments.saturation.overrideState = true;
            colorAdjustments.saturation.value = 6f;

            var tonemapping = EnsureVolumeComponent<Tonemapping>(existing);
            tonemapping.active = true;
            tonemapping.mode.overrideState = true;
            tonemapping.mode.value = TonemappingMode.Neutral;

            var vignette = EnsureVolumeComponent<Vignette>(existing);
            vignette.active = true;
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.1f;
            vignette.smoothness.overrideState = true;
            vignette.smoothness.value = 0.3f;

            EditorUtility.SetDirty(existing);
            return existing;
        }

        private static T EnsureVolumeComponent<T>(VolumeProfile profile)
            where T : VolumeComponent
        {
            if (profile.TryGet<T>(out var existingComponent) && existingComponent != null)
            {
                return existingComponent;
            }

            var component = ScriptableObject.CreateInstance<T>();
            component.name = typeof(T).Name;
            component.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(component, profile);
            profile.components.Add(component);
            EditorUtility.SetDirty(component);
            return component;
        }

        private static void UpgradeAnimalPackMaterialsForUrp()
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                Debug.LogWarning("Could not find 'Universal Render Pipeline/Lit'. Animal materials were not upgraded.");
                return;
            }

            if (!AssetDatabase.IsValidFolder(AnimalPackRoot))
            {
                Debug.LogWarning($"Animal pack folder was not found at '{AnimalPackRoot}'.");
                return;
            }

            var materialGuids = AssetDatabase.FindAssets("t:Material", new[] { AnimalPackRoot });
            foreach (var guid in materialGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (material == null)
                {
                    continue;
                }

                var baseMap = GetFirstTexture(material, "_BaseMap", "_MainTex", "_BaseColorMap");
                var normalMap = GetFirstTexture(material, "_BumpMap", "_NormalMap");
                var metallicMap = GetFirstTexture(material, "_MetallicGlossMap", "_MetallicSmoothnessMap");
                var occlusionMap = GetFirstTexture(material, "_OcclusionMap");
                var baseColor = GetFirstColor(material, Color.white, "_BaseColor", "_Color");

                material.shader = shader;
                SetTextureIfPresent(material, "_BaseMap", baseMap);
                SetTextureIfPresent(material, "_BumpMap", normalMap);
                SetTextureIfPresent(material, "_MetallicGlossMap", metallicMap);
                SetTextureIfPresent(material, "_OcclusionMap", occlusionMap);
                SetColorIfPresent(material, "_BaseColor", baseColor);
                SetFloatIfPresent(material, "_Smoothness", 0.38f);
                SetFloatIfPresent(material, "_Metallic", 0f);

                if (normalMap != null)
                {
                    SetFloatIfPresent(material, "_BumpScale", 1f);
                }

                EditorUtility.SetDirty(material);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void EnsureGroundMaterials()
        {
            EnsureFolder(RenderingFolder);

            EnsureUrpGroundMaterial(
                GroundGrassMaterialPath,
                "VeterinarVR_GroundGrass",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Grass_1A_D.tif",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Grass_1A_N.png",
                new Color(0.5f, 0.72f, 0.4f, 1f),
                5.5f);

            EnsureUrpGroundMaterial(
                GroundDirtMaterialPath,
                "VeterinarVR_GroundDirt",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Dirt_1A_D.png",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Dirt_1A_N.png",
                new Color(0.58f, 0.45f, 0.32f, 1f),
                4.5f);

            EnsureUrpGroundMaterial(
                GroundIndoorMaterialPath,
                "VeterinarVR_GroundIndoor",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Pavement_1A_D.png",
                "Assets/Toon Farm Pack/Terrain/TFP_Terrain_Pavement_1A_N.png",
                new Color(0.52f, 0.54f, 0.52f, 1f),
                4f);
        }

        private static Material EnsureUrpGroundMaterial(string materialPath, string materialName, string baseMapPath, string normalMapPath, Color fallbackColor, float tiling)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                throw new System.InvalidOperationException("Could not find 'Universal Render Pipeline/Lit' shader.");
            }

            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(shader)
                {
                    name = materialName
                };
                AssetDatabase.CreateAsset(material, materialPath);
            }
            else
            {
                material.shader = shader;
            }

            var baseMap = AssetDatabase.LoadAssetAtPath<Texture2D>(baseMapPath);
            var normalMap = AssetDatabase.LoadAssetAtPath<Texture2D>(normalMapPath);

            SetColorIfPresent(material, "_BaseColor", fallbackColor);
            SetTextureIfPresent(material, "_BaseMap", baseMap);
            SetTextureIfPresent(material, "_BumpMap", normalMap);
            SetFloatIfPresent(material, "_BumpScale", 0.55f);
            SetFloatIfPresent(material, "_Smoothness", 0.22f);
            SetFloatIfPresent(material, "_Metallic", 0f);

            if (material.HasProperty("_BaseMap"))
            {
                material.SetTextureScale("_BaseMap", Vector2.one * tiling);
            }

            if (material.HasProperty("_BumpMap"))
            {
                material.SetTextureScale("_BumpMap", Vector2.one * tiling);
            }

            EditorUtility.SetDirty(material);
            return material;
        }

        private static Texture GetFirstTexture(Material material, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (material.HasProperty(propertyName))
                {
                    var texture = material.GetTexture(propertyName);
                    if (texture != null)
                    {
                        return texture;
                    }
                }
            }

            return null;
        }

        private static Color GetFirstColor(Material material, Color fallback, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                if (material.HasProperty(propertyName))
                {
                    return material.GetColor(propertyName);
                }
            }

            return fallback;
        }

        private static void SetTextureIfPresent(Material material, string propertyName, Texture texture)
        {
            if (texture != null && material.HasProperty(propertyName))
            {
                material.SetTexture(propertyName, texture);
            }
        }

        private static void SetColorIfPresent(Material material, string propertyName, Color color)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetColor(propertyName, color);
            }
        }

        private static void SetFloatIfPresent(Material material, string propertyName, float value)
        {
            if (material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        private static Material EnsureDaySkyboxMaterial()
        {
            var existing = AssetDatabase.LoadAssetAtPath<Material>(DaySkyboxPath);
            if (existing != null)
            {
                return existing;
            }

            var shader = Shader.Find("Skybox/Procedural");
            if (shader == null)
            {
                throw new System.InvalidOperationException("Could not find 'Skybox/Procedural' shader.");
            }

            var material = new Material(shader)
            {
                name = "VeterinarVR_DaySkybox"
            };

            material.SetColor("_SkyTint", new Color(0.54f, 0.68f, 0.86f, 1f));
            material.SetColor("_GroundColor", new Color(0.42f, 0.45f, 0.43f, 1f));
            material.SetFloat("_AtmosphereThickness", 0.85f);
            material.SetFloat("_Exposure", 1.1f);
            material.SetFloat("_SunSize", 0.04f);
            material.SetFloat("_SunSizeConvergence", 5f);

            AssetDatabase.CreateAsset(material, DaySkyboxPath);
            return material;
        }

        private static void ApplyPipelineToAllQualityLevels(RenderPipelineAsset pipelineAsset)
        {
            var originalQuality = QualitySettings.GetQualityLevel();
            for (var index = 0; index < QualitySettings.names.Length; index++)
            {
                QualitySettings.SetQualityLevel(index, false);
                QualitySettings.renderPipeline = pipelineAsset;
            }

            QualitySettings.SetQualityLevel(originalQuality, false);
            QualitySettings.renderPipeline = pipelineAsset;
        }

        private static void ConfigureUrpLightingSettings(UniversalRenderPipelineAsset pipelineAsset)
        {
            var serializedObject = new SerializedObject(pipelineAsset);
            serializedObject.FindProperty("m_MainLightShadowsSupported").boolValue = true;
            serializedObject.FindProperty("m_AdditionalLightsRenderingMode").intValue = (int)LightRenderingMode.PerPixel;
            serializedObject.FindProperty("m_AdditionalLightsPerObjectLimit").intValue = 4;
            serializedObject.FindProperty("m_AdditionalLightShadowsSupported").boolValue = false;
            serializedObject.FindProperty("m_SoftShadowsSupported").boolValue = false;
            serializedObject.FindProperty("m_MainLightShadowmapResolution").intValue = 1024;
            serializedObject.FindProperty("m_AdditionalLightsShadowmapResolution").intValue = 512;
            serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void CreateScanFindingHotspot(Transform parent, string name, string findingId, Vector3 localPosition, Color color)
        {
            var hotspot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            hotspot.name = name;
            hotspot.transform.SetParent(parent, false);
            hotspot.transform.localPosition = localPosition;
            hotspot.transform.localScale = Vector3.one * 0.28f;

            var renderer = hotspot.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = color;
                renderer.sharedMaterial = material;
            }

            var target = hotspot.AddComponent<ScanFindingTarget>();
            SetPrivateObject(target, "findingId", findingId);
            SetPrivateObject(target, "targetRenderer", renderer);

            var (englishLabel, bahasaLabel) = findingId switch
            {
                "TailRaise" => ("Tail Raising", "Ekor Diangkat"),
                "MucusDischarge" => ("Mucus Discharge", "Lelehan Mukus"),
                "Restlessness" => ("Restlessness", "Gelisah"),
                _ => (findingId, findingId)
            };

            CreateWorldLocalizedLabel(hotspot.transform, $"{name}_Label", englishLabel, bahasaLabel, new Vector3(0f, 0.34f, 0f), new Vector2(300f, 68f), 22);
        }

        private static void CreateWorldLocalizedLabel(Transform parent, string name, string englishText, string bahasaText, Vector3 localPosition, Vector2 size, int fontSize)
        {
            var root = new GameObject(name, typeof(TextMeshPro));
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPosition;
            root.transform.localRotation = Quaternion.identity;
            root.transform.localScale = Vector3.one * 0.12f;

            var tmp = root.GetComponent<TextMeshPro>();
            tmp.text = englishText;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;
            tmp.fontStyle = FontStyles.Bold;
            tmp.enableAutoSizing = false;
            tmp.rectTransform.sizeDelta = size;

            var localizedText = root.AddComponent<LocalizedTextMeshPro>();
            SetPrivateObject(localizedText, "targetText", tmp);
            SetPrivateObject(localizedText, "englishText", englishText);
            SetPrivateObject(localizedText, "bahasaText", bahasaText);
        }

        private static void AttachLocalizedText(Text targetText, string englishText, string bahasaText)
        {
            var localizedText = targetText.gameObject.AddComponent<LocalizedText>();
            SetPrivateObject(localizedText, "targetText", targetText);
            SetPrivateObject(localizedText, "englishText", englishText);
            SetPrivateObject(localizedText, "bahasaText", bahasaText);
        }

        private static void SetPrivateObject(Object target, string fieldName, Object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetPrivateObject(Object target, string fieldName, string value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetPrivateObject(Object target, string fieldName, bool value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetPrivateStringArray(Object target, string fieldName, string[] value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }

        private static void SetPrivateAudioClipArray(Object target, string fieldName, AudioClip[] value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }

        private static System.Type FindType(string fullName)
        {
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            return null;
        }

        private static void SetPublicField(Component target, string fieldName, object value)
        {
            if (target == null)
            {
                return;
            }

            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
            {
                return;
            }

            field.SetValue(target, value);
            EditorUtility.SetDirty(target);
        }

        private static void EnsureFolder(string assetPath)
        {
            var parts = assetPath.Split('/');
            var current = parts[0];

            for (var index = 1; index < parts.Length; index++)
            {
                var next = $"{current}/{parts[index]}";
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, parts[index]);
                }

                current = next;
            }
        }

        private static void EnsureGuideAvatarImport()
        {
            // Masculine.fbx ships as a fully-configured Humanoid avatar (animationType=3,
            // avatarSetup=1, complete humanDescription). We do NOT reconfigure it here -
            // touching the importer would destroy the Streamoji team's tuned muscle
            // mapping, which is exactly what causes the limb deformation. We only verify
            // the Avatar sub-asset resolves so downstream steps can rely on it.
            var avatar = AssetDatabase.LoadAssetAtPath<Avatar>(GuideAvatarSourceFbxPath);
            if (avatar == null)
            {
                Debug.LogWarning($"Masculine.fbx at '{GuideAvatarSourceFbxPath}' has no Humanoid Avatar sub-asset. Check the Streamoji import is intact.");
                return;
            }

            Debug.Log($"Guide avatar source OK: {avatar.name} (valid={avatar.isValid}, human={avatar.isHuman}, bodyBones={avatar.humanDescription.human.Length}).");

            // The talking clip comes from a separate rig-native FBX (F_Talking_Variations_001),
            // already converted from GLB via Blender. Verify it loads too.
            var talkClips = AssetDatabase.LoadAllAssetsAtPath(GuideTalkClipFbxPath);
            var talkClip = FindNonPreviewClip(talkClips);
            if (talkClip == null)
            {
                Debug.LogWarning($"No usable AnimationClip found at '{GuideTalkClipFbxPath}'. The Talk state will be skipped.");
            }
            else
            {
                Debug.Log($"Guide talk clip OK: {talkClip.name} ({talkClip.length:F2}s).");
            }
        }

        private static AnimationClip FindNonPreviewClip(UnityEngine.Object[] assets)
        {
            if (assets == null)
            {
                return null;
            }

            foreach (var asset in assets)
            {
                if (asset is AnimationClip clip && !clip.name.StartsWith("__preview__", System.StringComparison.Ordinal))
                {
                    return clip;
                }
            }

            return null;
        }

        private static void EnsureGuideAnimatorController()
        {
            var folder = Path.GetDirectoryName(GuideAnimatorControllerPath);
            EnsureFolder(folder);

            // Load the talk clip (and Streamoji's idle as a calm default if present).
            var talkClip = FindNonPreviewClip(AssetDatabase.LoadAllAssetsAtPath(GuideTalkClipFbxPath));
            var idleClip = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Streamoji/Animations/IdleAnimation.anim");

            var controller = AnimatorController.CreateAnimatorControllerAtPath(GuideAnimatorControllerPath);
            controller.AddParameter("Talk", AnimatorControllerParameterType.Trigger);

            var layer = controller.layers[0];
            var stateMachine = layer.stateMachine;
            stateMachine.entryPosition = new Vector3(50f, 0f, 0f);
            stateMachine.anyStatePosition = new Vector3(50f, 100f, 0f);
            stateMachine.exitPosition = new Vector3(800f, 0f, 0f);

            // Idle = the calm Streamoji idle if present, otherwise fall back to the talk
            // clip so the guide is never motionless. The talk clip plays as a default
            // loop when no dedicated idle is available.
            var idleMotion = idleClip != null ? (Motion)idleClip : talkClip;
            var idleState = stateMachine.AddState("Idle", new Vector3(300f, 0f, 0f));
            idleState.motion = idleMotion;

            if (talkClip != null && idleMotion != talkClip)
            {
                var talkState = stateMachine.AddState(GuideTalkStateName, new Vector3(300f, 120f, 0f));
                talkState.motion = talkClip;

                var toTalk = idleState.AddTransition(talkState);
                toTalk.AddCondition(AnimatorConditionMode.If, 0, "Talk");
                toTalk.hasExitTime = false;
                toTalk.duration = 0.15f;

                var toIdle = talkState.AddTransition(idleState);
                toIdle.hasExitTime = true;
                toIdle.exitTime = 0.95f;
                toIdle.duration = 0.2f;
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Guide AnimatorController created at '{GuideAnimatorControllerPath}' (idle={(idleMotion != null ? idleMotion.name : "none")}, talk={(talkClip != null ? talkClip.name : "none")}).");
        }

        private static void BuildGuideAvatarPrefab()
        {
            EnsureFolder("Assets/_Project/VeterinarVR/Prefabs/Characters");

            var sourceModel = AssetDatabase.LoadAssetAtPath<GameObject>(GuideAvatarSourceFbxPath);
            if (sourceModel == null)
            {
                Debug.LogWarning($"Could not load guide avatar model at '{GuideAvatarSourceFbxPath}'.");
                return;
            }

            // Masculine.fbx already carries a valid Humanoid Avatar; load it to wire the Animator.
            var avatar = AssetDatabase.LoadAssetAtPath<Avatar>(GuideAvatarSourceFbxPath);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(GuideAnimatorControllerPath);

            var instance = PrefabUtility.InstantiatePrefab(sourceModel) as GameObject;
            instance.name = "pref_guide_avatar";
            instance.transform.position = Vector3.zero;
            instance.transform.rotation = Quaternion.identity;
            instance.transform.localScale = Vector3.one;

            var animator = instance.GetComponent<Animator>();
            if (animator == null)
            {
                animator = instance.AddComponent<Animator>();
            }

            animator.runtimeAnimatorController = controller;
            if (avatar != null)
            {
                animator.avatar = avatar;
            }
            animator.applyRootMotion = false;
            animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            EditorUtility.SetDirty(animator);

            DisableColliders(instance);

            var savedPrefab = PrefabUtility.SaveAsPrefabAsset(instance, GuideAvatarPrefabPath);
            Object.DestroyImmediate(instance);

            if (savedPrefab != null)
            {
                EditorUtility.SetDirty(savedPrefab);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void PlaceGuideAvatarInGreetingScene()
        {
            var guidePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GuideAvatarPrefabPath);
            if (guidePrefab == null)
            {
                Debug.LogWarning($"Could not load guide avatar prefab at '{GuideAvatarPrefabPath}'. Run BuildGuideAvatarPrefab first.");
                return;
            }

            var scene = EditorSceneManager.OpenScene(GreetingScenePath, OpenSceneMode.Single);

            var environmentRoot = GameObject.Find("GreetingEnvironment");
            if (environmentRoot == null)
            {
                Debug.LogWarning($"Could not find GreetingEnvironment root in '{GreetingScenePath}'.");
                return;
            }

            // Remove both our previous placement and the user's idle reference
            // (avatar_guide_a_Unity) so only the animated guide remains at the spot.
            var existingGuide = FindChildByName(environmentRoot.transform, "GuideAvatar");
            if (existingGuide != null)
            {
                Object.DestroyImmediate(existingGuide.gameObject);
            }

            var idleReference = FindChildByName(environmentRoot.transform, "avatar_guide_a_Unity");
            if (idleReference != null)
            {
                Object.DestroyImmediate(idleReference.gameObject);
            }

            var instance = PrefabUtility.InstantiatePrefab(guidePrefab) as GameObject;
            instance.name = "GuideAvatar";
            instance.transform.SetParent(environmentRoot.transform, false);
            // Coordinates match the user's idle reference (avatar_guide_a_Unity) placement.
            instance.transform.localPosition = new Vector3(GuideAvatarPosX, GuideAvatarPosY, GuideAvatarPosZ);
            instance.transform.localRotation = Quaternion.Euler(0f, GuideAvatarRotY, 0f);
            instance.transform.localScale = Vector3.one;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Guide avatar placed in '{GreetingScenePath}' at ({GuideAvatarPosX}, {GuideAvatarPosY}, {GuideAvatarPosZ}) rotY={GuideAvatarRotY}.");
        }

        public static void EnsureCowAndQuestionDataAssets()
        {
            EnsureFolder(QuestionsFolder);
            EnsureFolder(CowsFolder);

            // 1. Create QuestionData Assets
            var qS03Ready = CreateQuestionAsset(
                "Q_S03_Ready",
                "Is Cow B ready for insemination based on the scan parameters (38.5°C and mature follicle)?",
                new[] { "Yes (Mature follicle and normal temperature)", "No (Follicle is too small)", "No (Temperature indicates fever)" },
                0,
                10
            );

            var qS05Traits = CreateQuestionAsset(
                "Q_S05_Traits",
                "Which cow shows optimal genetic traits and health indicators for breeding?",
                new[] { "Cow A (Low milk yield, no heat)", "Cow B (High milk yield, active heat signs)", "Cow C (Average milk yield, silent heat)" },
                1,
                10
            );

            var qS05Records = CreateQuestionAsset(
                "Q_S05_Records",
                "What is the primary benefit of digital record-keeping in herd management?",
                new[] { "Tracks lineage, heat cycles, and genetic progress accurately", "Reduces daily feed consumption", "Guarantees twins in every pregnancy" },
                0,
                10
            );

            // 2. Create CowData Assets
            var cowA = CreateCowAsset(
                "Cow_A",
                "Cow A",
                "Herd Group Alpha",
                "Low milk yield, no signs of heat.",
                new QuestionData[0]
            );

            var cowB = CreateCowAsset(
                "Cow_B",
                "Cow B (Holstein)",
                "Herd Group Beta",
                "High milk yield, clear mucus discharge, smart collar alert active.",
                new[] { qS03Ready }
            );

            var cowC = CreateCowAsset(
                "Cow_C",
                "Cow C",
                "Herd Group Alpha",
                "Average milk yield, restless behaviour but no physical signs of heat.",
                new QuestionData[0]
            );

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // 3. Automatically locate ValidationDashboardController in S05 and link availableCows array
            LinkCowsToValidationDashboard(new[] { cowA, cowB, cowC });
            LinkCowsToCowScanDecision(new[] { cowA, cowB, cowC });
        }

        private static QuestionData CreateQuestionAsset(string id, string prompt, string[] options, int correctIndex, int score)
        {
            var path = $"{QuestionsFolder}/{id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<QuestionData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<QuestionData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            SetPrivateObject(asset, "questionId", id);
            SetPrivateObject(asset, "prompt", prompt);
            SetPrivateObject(asset, "answerOptions", options);
            SetPrivateObject(asset, "correctAnswerIndex", correctIndex);
            SetPrivateObject(asset, "scoreValue", score);

            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static CowData CreateCowAsset(string id, string name, string group, string notes, QuestionData[] questionSet)
        {
            var path = $"{CowsFolder}/{id}.asset";
            var asset = AssetDatabase.LoadAssetAtPath<CowData>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CowData>();
                AssetDatabase.CreateAsset(asset, path);
            }

            SetPrivateObject(asset, "cowId", id);
            SetPrivateObject(asset, "displayName", name);
            SetPrivateObject(asset, "herdGroup", group);
            SetPrivateObject(asset, "notes", notes);
            SetPrivateObject(asset, "questionSet", questionSet);

            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void LinkCowsToValidationDashboard(CowData[] cows)
        {
            var scene = EditorSceneManager.OpenScene(ValidationScenePath, OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<ValidationDashboardController>();
            if (controller != null)
            {
                SetPrivateObject(controller, "availableCows", cows);

                // Wire the two S05 validation MCQ questions
                var qTraits = AssetDatabase.LoadAssetAtPath<QuestionData>($"{QuestionsFolder}/Q_S05_Traits.asset");
                var qRecords = AssetDatabase.LoadAssetAtPath<QuestionData>($"{QuestionsFolder}/Q_S05_Records.asset");
                var s05Questions = new List<QuestionData>();
                if (qTraits != null) s05Questions.Add(qTraits);
                if (qRecords != null) s05Questions.Add(qRecords);
                if (s05Questions.Count > 0)
                {
                    SetPrivateObject(controller, "validationQuestions", s05Questions.ToArray());
                }

                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Successfully linked {cows.Length} cows and {s05Questions.Count} validation questions to ValidationDashboardController.");
            }
            else
            {
                Debug.LogWarning($"Could not find ValidationDashboardController in '{ValidationScenePath}' to link cows.");
            }
        }

        private static void EnableHandTracking()
        {
            var configType = System.Type.GetType("OVRProjectConfig, com.meta.xr.sdk.core.Editor");
            if (configType == null)
            {
                Debug.LogWarning("Could not find OVRProjectConfig type to enable hand tracking.");
                return;
            }

            var configs = AssetDatabase.FindAssets("t:OVRProjectConfig");
            if (configs.Length == 0)
            {
                Debug.LogWarning("No OVRProjectConfig asset found. Hand tracking must be enabled manually in Meta Quest Features.");
                return;
            }

            var configPath = AssetDatabase.GUIDToAssetPath(configs[0]);
            var config = AssetDatabase.LoadAssetAtPath<Object>(configPath);
            if (config == null)
            {
                return;
            }

            var serializedConfig = new SerializedObject(config);
            var handTrackingProp = serializedConfig.FindProperty("handTrackingSupport");
            if (handTrackingProp != null && handTrackingProp.intValue == 0)
            {
                handTrackingProp.intValue = 1;
            }

            var handFrequencyProp = serializedConfig.FindProperty("handTrackingFrequency");
            if (handFrequencyProp != null && handFrequencyProp.intValue == 0)
            {
                handFrequencyProp.intValue = 1;
            }

            serializedConfig.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
            Debug.Log("Hand tracking enabled: ControllersAndHands at HIGH frequency");
        }

        private static void LinkCowsToCowScanDecision(CowData[] cows)
        {
            var scene = EditorSceneManager.OpenScene(CowScanScenePath, OpenSceneMode.Single);
            var controller = Object.FindFirstObjectByType<CowScanController>();
            if (controller != null)
            {
                SetPrivateObject(controller, "availableCows", cows);
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
                Debug.Log($"Successfully linked {cows.Length} cows to CowScanController in '{CowScanScenePath}'.");
            }
            else
            {
                Debug.LogWarning($"Could not find CowScanController in '{CowScanScenePath}' to link cows.");
            }
        }

        private static void SetPrivateObject(Object target, string fieldName, object value)
        {
            var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (field != null)
            {
                field.SetValue(target, value);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
