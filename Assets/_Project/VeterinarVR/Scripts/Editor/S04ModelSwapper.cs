using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace VeterinarVR.Editor
{
    public static class S04ModelSwapper
    {
        private const string ScenePath = "Assets/_Project/VeterinarVR/Scenes/S04_AIProcedure.unity";
        private const string ModelRoot = "Assets/_Project/VeterinarVR/Models/Insemination";

        private static readonly Dictionary<string, string> PlaceholderToModel = new()
        {
            { "NitrogenTank",       "Meshy_AI_White_mini_keg_with_b_0705055117_texture_fbx/Meshy_AI_White_mini_keg_with_b_0705055117_texture.fbx" },
            { "Pistolette",         "Meshy_AI_Rainbow_Ringed_Needle_0705062107_texture_fbx/Meshy_AI_Rainbow_Ringed_Needle_0705062107_texture.fbx" },
            { "StrawCutter",        "Meshy_AI_Blue_retractable_badg_0705062919_texture_fbx/Meshy_AI_Blue_retractable_badg_0705062919_texture.fbx" },
            { "WarmWaterBeaker",    "Meshy_AI_White_mini_keg_with_b_0705055117_texture_fbx/Meshy_AI_White_mini_keg_with_b_0705055117_texture.fbx" },
            { "ThawingThermosFlask","Meshy_AI_Dairymac_Recharge_0705061113_texture_fbx/Meshy_AI_Dairymac_Recharge_0705061113_texture.fbx" },
        };

        private const string ForcepsModel = "uploads_files_3010095_simple+forceps.fbx";
        private const string InnerCanisterModel = "Meshy_AI_Minimalist_Stainless__0705060653_texture_fbx/Meshy_AI_Minimalist_Stainless__0705060653_texture.fbx";

        [MenuItem("Veterinar VR/Bootstrap/Swap S04 Models")]
        public static void SwapPlaceholders()
        {
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            if (scene == null)
            {
                Debug.LogError($"Could not open scene: {ScenePath}");
                return;
            }

            int swapped = 0;

            foreach (var kvp in PlaceholderToModel)
            {
                var placeholder = GameObject.Find(kvp.Key);
                if (placeholder == null)
                {
                    Debug.LogWarning($"Placeholder '{kvp.Key}' not found in scene. Skipping.");
                    continue;
                }

                string modelPath = $"{ModelRoot}/{kvp.Value}";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (prefab == null)
                {
                    Debug.LogWarning($"Model not found at '{modelPath}'. Did Unity finish importing?");
                    continue;
                }

                var parent = placeholder.transform.parent;
                var pos = placeholder.transform.localPosition;
                var rot = placeholder.transform.localRotation;
                var scale = placeholder.transform.localScale;

                Undo.DestroyObjectImmediate(placeholder);

                var replacement = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
                if (replacement == null)
                {
                    replacement = Object.Instantiate(prefab, parent);
                }
                Undo.RegisterCreatedObjectUndo(replacement, $"Swap {kvp.Key}");

                replacement.name = kvp.Key;
                replacement.transform.SetParent(parent);
                replacement.transform.localPosition = pos;
                replacement.transform.localRotation = rot;
                replacement.transform.localScale = scale;

                Debug.Log($"Swapped '{kvp.Key}' with '{kvp.Value}'");
                swapped++;
            }

            HandleForceps();
            HandleInnerCanister();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            Debug.Log($"S04 model swap complete. {swapped} placeholders replaced.");
        }

        private static void HandleForceps()
        {
            var placeholder = GameObject.Find("Forceps");
            if (placeholder != null)
            {
                var pos = placeholder.transform.localPosition;
                var rot = placeholder.transform.localRotation;
                var scale = placeholder.transform.localScale;
                var parent = placeholder.transform.parent;

                string modelPath = $"{ModelRoot}/{ForcepsModel}";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (prefab != null)
                {
                    Undo.DestroyObjectImmediate(placeholder);
                    var replacement = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
                    if (replacement == null)
                        replacement = Object.Instantiate(prefab, parent);
                    Undo.RegisterCreatedObjectUndo(replacement, "Swap Forceps");
                    replacement.name = "Forceps";
                    replacement.transform.SetParent(parent);
                    replacement.transform.localPosition = pos;
                    replacement.transform.localRotation = rot;
                    replacement.transform.localScale = scale;
                }
                else
                {
                    Debug.LogWarning("Forceps model not found. Placeholder left in place.");
                }
            }
            else
            {
                // No placeholder exists — create the forceps directly near the NitrogenTank
                var tank = GameObject.Find("NitrogenTank");
                var parent = tank != null ? tank.transform.parent : null;
                var tankPos = tank != null ? tank.transform.position : Vector3.zero;

                string modelPath = $"{ModelRoot}/{ForcepsModel}";
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
                if (prefab == null)
                {
                    Debug.LogWarning("Forceps model not found. Create placeholder or import manually.");
                    return;
                }

                var forceps = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
                if (forceps == null)
                    forceps = Object.Instantiate(prefab, parent);
                Undo.RegisterCreatedObjectUndo(forceps, "Create Forceps");
                forceps.name = "Forceps";
                forceps.transform.position = tankPos + new Vector3(0.5f, 0f, 0.5f);
                forceps.transform.localScale = Vector3.one * 0.5f;
                Debug.Log("Forceps created near NitrogenTank (no placeholder existed). Adjust position as needed.");
            }
        }

        private static void HandleInnerCanister()
        {
            var tank = GameObject.Find("NitrogenTank");
            if (tank == null) return;

            string modelPath = $"{ModelRoot}/{InnerCanisterModel}";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
            if (prefab == null)
            {
                Debug.LogWarning("Inner canister model not found.");
                return;
            }

            var existing = GameObject.Find("InnerCanister");
            if (existing != null)
                Undo.DestroyObjectImmediate(existing);

            var canister = PrefabUtility.InstantiatePrefab(prefab, tank.transform) as GameObject;
            if (canister == null)
                canister = Object.Instantiate(prefab, tank.transform);
            Undo.RegisterCreatedObjectUndo(canister, "Create InnerCanister");

            canister.name = "InnerCanister";
            canister.transform.localPosition = Vector3.zero;
            canister.transform.localRotation = Quaternion.identity;
            canister.transform.localScale = tank.transform.localScale * 0.75f;

            Debug.Log("Inner canister placed inside NitrogenTank at 75% scale.");
        }
    }
}
