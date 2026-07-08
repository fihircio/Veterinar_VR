using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace VeterinarVR.EditorTools
{
    public static class FixMissingShaders
    {
        [MenuItem("VeterinarVR/Fix Missing Shaders in Scene")]
        private static void FixSceneShaders()
        {
            var renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            var fixedMaterials = new HashSet<Material>();
            int shaderCount = 0;

            foreach (var renderer in renderers)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat == null) continue;
                    if (fixedMaterials.Contains(mat)) continue;

                    if (mat.shader == null || mat.shader.name == "Standard")
                    {
                        var urpLit = Shader.Find("Universal Render Pipeline/Lit");
                        if (urpLit != null)
                        {
                            mat.shader = urpLit;
                            shaderCount++;
                            fixedMaterials.Add(mat);
                            Debug.Log($"Fixed: {mat.name} on {renderer.gameObject.name}", mat);
                        }
                    }
                }
            }

            Debug.Log($"Fixed {shaderCount} materials in the scene.");
            AssetDatabase.Refresh();
        }

        [MenuItem("VeterinarVR/Fix Missing Shaders in Project")]
        private static void FixProjectShaders()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material", null);
            int shaderCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (mat == null) continue;

                if (mat.shader == null || mat.shader.name == "Standard")
                {
                    var urpLit = Shader.Find("Universal Render Pipeline/Lit");
                    if (urpLit != null)
                    {
                        mat.shader = urpLit;
                        shaderCount++;
                    }
                }
            }

            Debug.Log($"Fixed {shaderCount} materials across the project.");
            AssetDatabase.Refresh();
        }
    }
}
