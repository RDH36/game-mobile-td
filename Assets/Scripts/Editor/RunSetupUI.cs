using UnityEditor;
using UnityEngine;

public static class RunSetupUI
{
    public static string Execute()
    {
        SetupUICanvases.Execute();
        return "All UI Canvases rebuilt successfully.";
    }
}
