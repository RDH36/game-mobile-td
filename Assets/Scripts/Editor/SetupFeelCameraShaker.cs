using UnityEditor;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public static class SetupFeelCameraShaker
{
    [MenuItem("Tools/Setup Feel Camera Shaker")]
    public static void Execute()
    {
        // Find main camera
        Camera mainCam = Camera.main;
        if (mainCam == null)
        {
            Debug.LogError("No Main Camera found!");
            return;
        }

        // Check if already set up
        if (mainCam.GetComponentInParent<MMCameraShaker>() != null)
        {
            Debug.Log("Feel Camera Shaker already set up!");
            return;
        }

        // Create CameraRig
        GameObject cameraRig = new GameObject("CameraRig");
        cameraRig.transform.position = mainCam.transform.position;

        // Create CameraShaker child
        GameObject cameraShaker = new GameObject("CameraShaker");
        cameraShaker.transform.SetParent(cameraRig.transform);
        cameraShaker.transform.localPosition = Vector3.zero;

        // Add MMCameraShaker (which requires MMWiggle)
        MMCameraShaker shaker = cameraShaker.AddComponent<MMCameraShaker>();
        MMWiggle wiggle = cameraShaker.GetComponent<MMWiggle>();
        wiggle.PositionActive = true;
        wiggle.PositionWiggleProperties = new WiggleProperties();
        wiggle.PositionWiggleProperties.WigglePermitted = false;
        wiggle.PositionWiggleProperties.WiggleType = WiggleTypes.Noise;

        // Reparent camera under shaker
        mainCam.transform.SetParent(cameraShaker.transform);

        // Add our CameraShake wrapper to the shaker
        cameraShaker.AddComponent<CameraShake>();

        EditorUtility.SetDirty(cameraRig);
        Debug.Log("Feel Camera Shaker rig created: CameraRig > CameraShaker > Main Camera");
    }
}
