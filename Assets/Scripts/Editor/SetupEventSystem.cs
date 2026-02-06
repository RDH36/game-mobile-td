using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public static class SetupEventSystem
{
    [MenuItem("Tools/Setup Event System")]
    public static void Execute()
    {
        // Remove old EventSystem if exists
        var old = Object.FindFirstObjectByType<EventSystem>();
        if (old != null)
            Object.DestroyImmediate(old.gameObject);

        // Create new EventSystem with Input System UI module
        GameObject go = new GameObject("EventSystem");
        go.AddComponent<EventSystem>();
        go.AddComponent<InputSystemUIInputModule>();

        Debug.Log("EventSystem created with InputSystemUIInputModule!");
    }
}
