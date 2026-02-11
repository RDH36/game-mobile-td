using UnityEditor;
using UnityEngine;

public static class MoveWaveDataToResources
{
    public static void Execute()
    {
        // Create Resources/Waves folder if needed
        if (!AssetDatabase.IsValidFolder("Assets/Resources/Waves"))
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                AssetDatabase.CreateFolder("Assets", "Resources");
            AssetDatabase.CreateFolder("Assets/Resources", "Waves");
        }

        // Move all WaveData assets
        string[] guids = AssetDatabase.FindAssets("t:WaveData", new[] { "Assets/ScriptableObjects/WaveData" });
        foreach (string guid in guids)
        {
            string oldPath = AssetDatabase.GUIDToAssetPath(guid);
            string fileName = System.IO.Path.GetFileName(oldPath);
            string newPath = $"Assets/Resources/Waves/{fileName}";
            string result = AssetDatabase.MoveAsset(oldPath, newPath);
            if (string.IsNullOrEmpty(result))
                Debug.Log($"Moved: {oldPath} → {newPath}");
            else
                Debug.LogError($"Failed to move {oldPath}: {result}");
        }

        AssetDatabase.Refresh();
        Debug.Log("WaveData migration complete!");
    }
}
