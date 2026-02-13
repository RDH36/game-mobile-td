using UnityEditor;
using UnityEngine;

public static class SetBossPrefabScale
{
    public static string Execute()
    {
        string path = "Assets/Prefabs/Enemies/Boss.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) return $"ERROR: Prefab not found at {path}";

        // Open prefab for editing
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        using (var editScope = new PrefabUtility.EditPrefabContentsScope(assetPath))
        {
            GameObject root = editScope.prefabContentsRoot;
            root.transform.localScale = new Vector3(0.85f, 0.85f, 1f);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Verify
        GameObject verify = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        Vector3 scale = verify.transform.localScale;
        return $"Boss prefab scale set to ({scale.x}, {scale.y}, {scale.z})";
    }
}
