using UnityEditor;

public static class RefreshAssets
{
    public static void Execute()
    {
        AssetDatabase.Refresh();
    }
}
