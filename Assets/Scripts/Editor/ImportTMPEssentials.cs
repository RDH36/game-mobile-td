using UnityEditor;
using UnityEngine;
using System.IO;

public static class ImportTMPEssentials
{
    [MenuItem("Tools/Import TMP Essentials")]
    public static void Execute()
    {
        // Find the TMP package path
        string packagePath = GetTMPPackagePath();
        if (string.IsNullOrEmpty(packagePath))
        {
            Debug.LogError("Could not find TMP package path!");
            return;
        }

        string essentialsPackage = Path.Combine(packagePath, "Package Resources", "TMP Essential Resources.unitypackage");
        if (!File.Exists(essentialsPackage))
        {
            // Try alternative paths for Unity 6
            essentialsPackage = Path.Combine(packagePath, "Package Resources~", "TMP Essential Resources.unitypackage");
        }

        if (File.Exists(essentialsPackage))
        {
            AssetDatabase.ImportPackage(essentialsPackage, false);
            Debug.Log($"TMP Essential Resources imported from: {essentialsPackage}");
        }
        else
        {
            Debug.LogError($"TMP Essential Resources not found at: {essentialsPackage}");
            // List what's available
            string resourceDir = Path.Combine(packagePath, "Package Resources");
            if (Directory.Exists(resourceDir))
            {
                foreach (var f in Directory.GetFiles(resourceDir, "*", SearchOption.AllDirectories))
                    Debug.Log($"  Found: {f}");
            }
            resourceDir = Path.Combine(packagePath, "Package Resources~");
            if (Directory.Exists(resourceDir))
            {
                foreach (var f in Directory.GetFiles(resourceDir, "*", SearchOption.AllDirectories))
                    Debug.Log($"  Found: {f}");
            }
        }
    }

    static string GetTMPPackagePath()
    {
        // Unity 6: TMP is part of com.unity.ugui
        string uguiPath = Path.GetFullPath("Packages/com.unity.ugui");
        if (Directory.Exists(uguiPath))
        {
            // Check if TMP resources exist inside ugui
            string tmpInUgui = Path.Combine(uguiPath, "Package Resources");
            if (Directory.Exists(tmpInUgui))
                return uguiPath;

            string tmpInUguiHidden = Path.Combine(uguiPath, "Package Resources~");
            if (Directory.Exists(tmpInUguiHidden))
                return uguiPath;
        }

        // Fallback: standalone TMP package
        string tmpPath = Path.GetFullPath("Packages/com.unity.textmeshpro");
        if (Directory.Exists(tmpPath))
            return tmpPath;

        return null;
    }
}
