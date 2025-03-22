using PrototypeSubMod.Patches;
using System.Collections;
using PrototypeSubMod.Credits;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PrototypeSubMod.PrototypeStory;

internal class EndCinematicCameraController : MonoBehaviour
{
    [SerializeField] private SubRoot root;
    [SerializeField] private ProtoStoryLocker storyLocker;
    [SerializeField] private CyclopsExternalCams externalCams;
    [SerializeField] private FreezeRigidbodyWhenFar freezeWhenFar;
    [SerializeField] private Transform cameraPos;
    [SerializeField] private float cameraDelay;
    [SerializeField] private float creditsDelay;

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

        ProtoScreenFadeManager.instance.FadeIn(2);
        Player.main.cinematicModeActive = true;
        Inventory.main.quickSlots.SetIgnoreHotkeyInput(true);
        Player.main.GetPDA().Close();
        Player.main.GetPDA().SetIgnorePDAInput(true);
        yield return new WaitForSeconds(2);
        
        LockCameraPosition();
        
        ProtoScreenFadeManager.instance.FadeOut(2);

        yield return StartCreditsDelayed();
    }

    private IEnumerator StartCreditsDelayed()
    {
        yield return new WaitForSeconds(creditsDelay);

        var asyncOp = SceneManager.LoadSceneAsync("ProtoCredits");
        while (!asyncOp.isDone)
        {
            yield return null;
        }
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
