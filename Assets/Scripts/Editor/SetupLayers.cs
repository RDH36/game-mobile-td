using UnityEditor;
using UnityEngine;

public static class SetupLayers
{
    [MenuItem("Tools/Setup Arrow Strike Layers")]
    public static void Execute()
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        string[] customLayers = { "Player", "Arrow", "Enemy", "Wall", "Obstacle" };

        foreach (string layerName in customLayers)
        {
            bool found = false;
            for (int i = 0; i < layers.arraySize; i++)
            {
                if (layers.GetArrayElementAtIndex(i).stringValue == layerName)
                {
                    found = true;
                    break;
                }
            }
            if (found) continue;

            for (int i = 6; i < layers.arraySize; i++)
            {
                if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue))
                {
                    layers.GetArrayElementAtIndex(i).stringValue = layerName;
                    Debug.Log($"Layer '{layerName}' added at index {i}");
                    break;
                }
            }
        }

        tagManager.ApplyModifiedProperties();
        Debug.Log("All layers configured!");
    }
}
