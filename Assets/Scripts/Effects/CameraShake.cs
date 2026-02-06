using UnityEngine;
using MoreMountains.Feedbacks;

/// <summary>
/// Thin wrapper around Feel's MMCameraShakeEvent for easy access from game code.
/// Requires MMCameraShaker + MMWiggle on the camera (setup via Feel's rig).
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public void Shake(float intensity = 0.15f, float duration = 0.2f)
    {
        float frequency = 40f;
        MMCameraShakeEvent.Trigger(duration, intensity, frequency, 0f, 0f, 0f);
    }
}
