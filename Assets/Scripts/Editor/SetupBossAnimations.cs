using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

public static class SetupBossAnimations
{
    [MenuItem("Monster Cannon/Setup Boss Animations")]
    public static void Execute()
    {
        EnsureFolder("Assets/Animations");
        EnsureFolder("Assets/Animations/Bosses");

        string[] bossNames = { "Bos01", "Bos02", "Bos03", "Bos04", "Bos05" };

        foreach (string bossName in bossNames)
        {
            string controllerPath = $"Assets/Animations/Bosses/{bossName}.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
            {
                Debug.Log($"{bossName}: controller already exists, skipping.");
                continue;
            }

            string folder = $"Assets/Art/Sprites/Png/{bossName}";
            string prefix = $"{bossName}-animation_";
            Sprite[] frames = LoadFrames(folder, prefix);

            if (frames.Length == 0)
            {
                Debug.LogWarning($"{bossName}: no sprite frames found in {folder}");
                continue;
            }

            CreateBossAnimation(bossName, frames);
            Debug.Log($"{bossName}: created controller + idle clip ({frames.Length} frames @ 12fps)");
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("SetupBossAnimations COMPLETE");
    }

    static void CreateBossAnimation(string bossName, Sprite[] idleFrames)
    {
        string clipPath = $"Assets/Animations/Bosses/{bossName}_Idle.anim";
        string controllerPath = $"Assets/Animations/Bosses/{bossName}.controller";

        AnimationClip clip = new AnimationClip();
        clip.name = $"{bossName}_Idle";
        clip.frameRate = 12f;

        EditorCurveBinding binding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[idleFrames.Length];
        for (int i = 0; i < idleFrames.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i / 12f,
                value = idleFrames[i]
            };
        }

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        AssetDatabase.CreateAsset(clip, clipPath);

        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var rootSM = controller.layers[0].stateMachine;
        var idleState = rootSM.AddState("Idle");
        idleState.motion = clip;
        rootSM.defaultState = idleState;
    }

    static Sprite[] LoadFrames(string folder, string prefix)
    {
        if (!AssetDatabase.IsValidFolder(folder)) return new Sprite[0];

        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folder });
        return guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Where(p => Path.GetFileName(p).StartsWith(prefix))
            .OrderBy(p => p)
            .Select(p => AssetDatabase.LoadAssetAtPath<Sprite>(p))
            .Where(s => s != null)
            .ToArray();
    }

    static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace("\\", "/");
        string name = Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, name);
    }
}
