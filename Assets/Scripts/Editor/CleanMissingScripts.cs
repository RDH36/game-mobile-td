using UnityEditor;
using UnityEngine;

public static class CleanMissingScripts
{
    [MenuItem("Tools/Clean Missing Scripts")]
    public static void Execute()
    {
        int totalRemoved = 0;

        // Clean scene objects
        foreach (var go in Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0)
            {
                Debug.Log($"Removed {removed} missing scripts from scene object: {go.name}");
                totalRemoved += removed;
            }
        }

        // Clean all prefabs
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null) continue;

            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
            if (removed > 0)
            {
                Debug.Log($"Removed {removed} missing scripts from prefab: {path}");
                EditorUtility.SetDirty(prefab);
                totalRemoved += removed;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Total missing scripts removed: {totalRemoved}");
    }
}
