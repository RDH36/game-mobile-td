using UnityEditor;
using UnityEngine;

public static class TempActivatePanel
{
    public static string Execute()
    {
        foreach (var canvas in Resources.FindObjectsOfTypeAll<Canvas>())
        {
            if (canvas.gameObject.name == "GameOverCanvas")
            {
                var panel = canvas.transform.Find("GameOverPanel");
                if (panel != null)
                {
                    panel.gameObject.SetActive(false);
                    return "GameOverPanel deactivated";
                }
            }
        }
        return "GameOverPanel not found";
    }
}
