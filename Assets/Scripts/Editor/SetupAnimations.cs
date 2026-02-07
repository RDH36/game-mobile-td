using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;

public static class SetupAnimations
{
    [MenuItem("Monster Cannon/Setup Animations")]
    public static string Execute()
    {
        // Ensure folders exist
        EnsureFolder("Assets/Animations");
        EnsureFolder("Assets/Animations/Monsters");
        EnsureFolder("Assets/Animations/Cannon");
        EnsureFolder("Assets/Animations/FX");

        // 1. Load sprite frames
        Sprite[] monster01 = LoadFrames("Assets/Art/Sprites/Png/Monster/Monster01", "Monster01-animation_");
        Sprite[] monster03 = LoadFrames("Assets/Art/Sprites/Png/Monster/Monster03", "Monster03-animation_");
        Sprite[] monster05 = LoadFrames("Assets/Art/Sprites/Png/Monster/Monster05", "Monster05-animation_");
        Sprite[] cannonShoot = LoadFrames("Assets/Art/Sprites/Png/Guns/Gun01/Shoot", "Gun01-Shoot_");
        Sprite[] cannonIdle = LoadFrames("Assets/Art/Sprites/Png/Guns/Gun01/Idle", "Gun01-Idle_");
        Sprite[] shootFX = LoadFrames("Assets/Art/Sprites/Png/Shoot fx", "Shoot fx-animation_");
        Sprite[] deathFX = LoadFrames("Assets/Art/Sprites/Png/Dead fx", "EnemyDieFx-EnemyDieFx_");

        Debug.Log($"Loaded — M01:{monster01.Length} M03:{monster03.Length} M05:{monster05.Length} Shoot:{cannonShoot.Length} ShootFX:{shootFX.Length} DeathFX:{deathFX.Length}");

        // 2. Create Monster AnimationClips + Controllers
        CreateMonsterAnimations("Monster01", monster01, "Assets/ScriptableObjects/EnemyData/EnemyData_Weak.asset");
        CreateMonsterAnimations("Monster03", monster03, "Assets/ScriptableObjects/EnemyData/EnemyData_Medium.asset");
        CreateMonsterAnimations("Monster05", monster05, "Assets/ScriptableObjects/EnemyData/EnemyData_Strong.asset");

        // 3. Create Cannon AnimatorController (Idle → Shoot → Idle)
        CreateCannonAnimator(cannonIdle, cannonShoot);

        // 4. Create FX prefabs with Animator
        CreateFXPrefab("ShootFX", shootFX, 24f, 5, 1f);
        CreateFXPrefab("DeathFX", deathFX, 20f, 5, 6f);

        // 5. Assign FX prefabs to ArrowManager
        AssignArrowManagerPrefabs();

        // 6. Assign Cannon Animator to Bow in scene
        AssignCannonAnimator();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return "SetupAnimations COMPLETE — check Assets/Animations/";
    }

    // ─── Animation Clip Creation ───

    static AnimationClip CreateSpriteClip(string name, Sprite[] sprites, float fps, bool loop)
    {
        AnimationClip clip = new AnimationClip();
        clip.name = name;
        clip.frameRate = fps;

        EditorCurveBinding binding = new EditorCurveBinding();
        binding.type = typeof(SpriteRenderer);
        binding.path = "";
        binding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length + (loop ? 0 : 0)];
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe();
            keyframes[i].time = i / fps;
            keyframes[i].value = sprites[i];
        }

        AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

        // Set loop
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = loop;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        return clip;
    }

    // ─── Monster Animations ───

    static void CreateMonsterAnimations(string monsterName, Sprite[] idleFrames, string enemyDataPath)
    {
        string clipPath = $"Assets/Animations/Monsters/{monsterName}_Idle.anim";
        string controllerPath = $"Assets/Animations/Monsters/{monsterName}.controller";

        // Create idle clip (looping)
        AnimationClip idleClip = CreateSpriteClip($"{monsterName}_Idle", idleFrames, 12f, true);
        AssetDatabase.CreateAsset(idleClip, clipPath);

        // Create controller with single Idle state
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var rootSM = controller.layers[0].stateMachine;
        var idleState = rootSM.AddState("Idle");
        idleState.motion = idleClip;
        rootSM.defaultState = idleState;

        // Assign to EnemyData
        EnemyData data = AssetDatabase.LoadAssetAtPath<EnemyData>(enemyDataPath);
        if (data != null)
        {
            data.animController = controller;
            EditorUtility.SetDirty(data);
        }

        Debug.Log($"Created {monsterName}: {clipPath} + {controllerPath}");
    }

    // ─── Cannon Animator ───

    static void CreateCannonAnimator(Sprite[] idleFrames, Sprite[] shootFrames)
    {
        string idleClipPath = "Assets/Animations/Cannon/Cannon_Idle.anim";
        string shootClipPath = "Assets/Animations/Cannon/Cannon_Shoot.anim";
        string controllerPath = "Assets/Animations/Cannon/Cannon.controller";

        // Idle clip — single frame, looping (keeps the sprite stable)
        AnimationClip idleClip = CreateSpriteClip("Cannon_Idle", idleFrames, 1f, true);
        AssetDatabase.CreateAsset(idleClip, idleClipPath);

        // Shoot clip — 10 frames, one-shot
        AnimationClip shootClip = CreateSpriteClip("Cannon_Shoot", shootFrames, 20f, false);
        AssetDatabase.CreateAsset(shootClip, shootClipPath);

        // Controller: Idle (default) ←→ Shoot (trigger)
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        controller.AddParameter("Shoot", AnimatorControllerParameterType.Trigger);

        var rootSM = controller.layers[0].stateMachine;

        // Remove default empty state
        if (rootSM.states.Length > 0)
        {
            foreach (var s in rootSM.states)
                rootSM.RemoveState(s.state);
        }

        var idleState = rootSM.AddState("Idle");
        idleState.motion = idleClip;
        rootSM.defaultState = idleState;

        var shootState = rootSM.AddState("Shoot");
        shootState.motion = shootClip;

        // Idle → Shoot (on trigger)
        var toShoot = idleState.AddTransition(shootState);
        toShoot.AddCondition(AnimatorConditionMode.If, 0, "Shoot");
        toShoot.hasExitTime = false;
        toShoot.duration = 0f;

        // Shoot → Idle (when anim finishes)
        var toIdle = shootState.AddTransition(idleState);
        toIdle.hasExitTime = true;
        toIdle.exitTime = 1f;
        toIdle.duration = 0f;

        Debug.Log($"Created Cannon animator: {controllerPath}");
    }

    // ─── FX Prefabs ───

    static void CreateFXPrefab(string fxName, Sprite[] frames, float fps, int sortOrder, float scale)
    {
        string clipPath = $"Assets/Animations/FX/{fxName}.anim";
        string controllerPath = $"Assets/Animations/FX/{fxName}.controller";
        string prefabPath = $"Assets/Prefabs/Effects/{fxName}.prefab";

        // Delete existing
        if (AssetDatabase.LoadAssetAtPath<Object>(clipPath) != null) AssetDatabase.DeleteAsset(clipPath);
        if (AssetDatabase.LoadAssetAtPath<Object>(controllerPath) != null) AssetDatabase.DeleteAsset(controllerPath);
        if (AssetDatabase.LoadAssetAtPath<Object>(prefabPath) != null) AssetDatabase.DeleteAsset(prefabPath);

        // Create clip (one-shot)
        AnimationClip clip = CreateSpriteClip(fxName, frames, fps, false);
        AssetDatabase.CreateAsset(clip, clipPath);

        // Create controller
        AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var rootSM = controller.layers[0].stateMachine;
        var state = rootSM.AddState("Play");
        state.motion = clip;
        rootSM.defaultState = state;

        // Create prefab
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs/Effects"))
            AssetDatabase.CreateFolder("Assets/Prefabs", "Effects");

        GameObject go = new GameObject(fxName);
        go.transform.localScale = new Vector3(scale, scale, 1f);

        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = sortOrder;
        if (frames.Length > 0) sr.sprite = frames[0];

        Animator animator = go.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        go.AddComponent<DestroyAfterAnimation>();

        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);

        Debug.Log($"Created FX: {prefabPath} ({frames.Length} frames @ {fps}fps, scale {scale}x)");
    }

    // ─── Assignment ───

    static void AssignArrowManagerPrefabs()
    {
        ArrowManager mgr = Object.FindFirstObjectByType<ArrowManager>();
        if (mgr == null) { Debug.LogWarning("ArrowManager not found!"); return; }

        SerializedObject so = new SerializedObject(mgr);
        so.FindProperty("shootFXPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/ShootFX.prefab");
        so.FindProperty("deathFXPrefab").objectReferenceValue = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Effects/DeathFX.prefab");
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(mgr);
        Debug.Log("ArrowManager: prefabs assigned");
    }

    static void AssignCannonAnimator()
    {
        GameObject bow = GameObject.Find("Bow");
        if (bow == null) { Debug.LogWarning("Bow not found in scene!"); return; }

        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Cannon/Cannon.controller");
        if (controller == null) { Debug.LogWarning("Cannon.controller not found!"); return; }

        Animator animator = bow.GetComponent<Animator>();
        if (animator == null) animator = bow.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;

        EditorUtility.SetDirty(bow);
        Debug.Log("Bow: Cannon.controller assigned");
    }

    // ─── Helpers ───

    static Sprite[] LoadFrames(string folder, string prefix)
    {
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
