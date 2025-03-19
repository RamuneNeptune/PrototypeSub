using PrototypeSubMod.Patches;
using System.Collections;
using UnityEngine;

namespace PrototypeSubMod.PrototypeStory;

internal class EndCinematicCameraController : MonoBehaviour
{
    [SerializeField] private ProtoStoryLocker storyLocker;
    [SerializeField] private CyclopsExternalCams externalCams;
    [SerializeField] private FreezeRigidbodyWhenFar freezeWhenFar;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float cameraDelay;

    private void Start()
    {
        storyLocker.onEndingStart += OnEndingStarts;
    }

    private void OnEndingStarts()
    {
        StartCoroutine(StartCameraDelay());
    }

    private IEnumerator StartCameraDelay()
    {
        yield return new WaitForSeconds(cameraDelay);

        LockCameraPosition();
    }

    private void LockCameraPosition()
    {
        freezeWhenFar.enabled = false;

        externalCams.SetActive(false);
        Player.main.TryEject();
        MainCameraControl.main.enabled = false;
        SNCameraRoot.main.transform.position = cameraPos.position;
        SNCameraRoot.main.transform.rotation = cameraPos.rotation;
        SNCameraRoot.main.transform.SetParent(null);
        Player.main.groundMotor.enabled = false;

        GUIController_Patches.SetDenyHideCycling(true);
        GUIController.SetHidePhase(GUIController.HidePhase.All);

        VRUtil.Recenter();
    }

    private void OnDestroy()
    {
        GUIController_Patches.SetDenyHideCycling(false);
    }
}
