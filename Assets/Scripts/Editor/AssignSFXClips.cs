using UnityEditor;
using UnityEngine;

public static class AssignSFXClips
{
    public static void Execute()
    {
        var sfx = Object.FindFirstObjectByType<SFXManager>();
        if (sfx == null)
        {
            Debug.LogError("SFXManager not found in scene!");
            return;
        }

        var so = new SerializedObject(sfx);

        SetClip(so, "cannonFire", "Assets/Audio/SFX/cannon-fire.mp3");
        SetClip(so, "hitWall", "Assets/Audio/SFX/hit-wall.wav");
        SetClip(so, "monsterHit", "Assets/Audio/SFX/moster-hit.wav");
        SetClip(so, "cannonDamage", "Assets/Audio/SFX/cannon-damage-receive.wav");
        SetClip(so, "coinCollect", "Assets/Audio/SFX/coins.wav");
        SetClip(so, "uiClick", "Assets/Audio/SFX/click2.ogg");
        SetClip(so, "menuMusic", "Assets/Audio/Music/menu-bg-music.wav");
        SetClip(so, "actionMusic", "Assets/Audio/Music/action-bg-music.wav");

        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(sfx);
        Debug.Log("SFXManager: All audio clips assigned!");
    }

    static void SetClip(SerializedObject so, string propName, string assetPath)
    {
        var prop = so.FindProperty(propName);
        if (prop == null)
        {
            Debug.LogError($"Property '{propName}' not found!");
            return;
        }
        var clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);
        if (clip == null)
        {
            Debug.LogError($"AudioClip not found at '{assetPath}'!");
            return;
        }
        prop.objectReferenceValue = clip;
        Debug.Log($"Assigned {propName} = {assetPath}");
    }
}
