using UnityEngine;

namespace PrototypeSubMod.Monobehaviors;

internal class EngineCameraShake : MonoBehaviour
{
    [Header("Lever Lock")]
    [SerializeField] private float intensity_LL = 1;
    [SerializeField] private float duration_LL = -1;
    [SerializeField] private MainCameraControl.ShakeMode shakeMode_LL = MainCameraControl.ShakeMode.Linear;
    [SerializeField] private float shakeFrequency_LL = 1;

    [Header("Lever Down")]
    [SerializeField] private float intensity_LD = 1;
    [SerializeField] private float duration_LD = -1;
    [SerializeField] private MainCameraControl.ShakeMode shakeMode_LD = MainCameraControl.ShakeMode.Linear;
    [SerializeField] private float shakeFrequency_LD = 1;

    [Header("Lever Up")]
    [SerializeField] private float intensity_LU = 1;
    [SerializeField] private float duration_LU = -1;
    [SerializeField] private MainCameraControl.ShakeMode shakeMode_LU = MainCameraControl.ShakeMode.Linear;
    [SerializeField] private float shakeFrequency_LU = 1;

    public void PushUpShake()
    {
        MainCameraControl.main.ShakeCamera(intensity_LU, duration_LU, shakeMode_LU, shakeFrequency_LU);
    }

    public void PullDownShake()
    {
        MainCameraControl.main.ShakeCamera(intensity_LD, duration_LD, shakeMode_LD, shakeFrequency_LD);
    }

    public void LockShake()
    {
        MainCameraControl.main.ShakeCamera(intensity_LL, duration_LL, shakeMode_LL, shakeFrequency_LL);
    }
}
