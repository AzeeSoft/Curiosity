using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ResearchScannableEffect))]
public class ResearchScannable : MonoBehaviour
{
    public ResearchItemData researchItemData;

    private ResearchScannableEffect _researchScannableEffect;

    private void Awake()
    {
        _researchScannableEffect = GetComponent<ResearchScannableEffect>();
    }

    public void StartResearch()
    {
        StartCoroutine(TriggerResearch());
    }

    IEnumerator TriggerResearch()
    {
        float transitionDuration = 2f;
        float researchDuration = 5f;

        CinemachineCameraManager.Instance.SwitchCameraState(
            CinemachineCameraManager.CinemachineCameraState.Research, new ResearchCamera.StateData()
            {
                LookAtTarget = transform,
                ZoomInOut = false,
                EventLockDuration = researchDuration,
                researchItemData = researchItemData,
                showInfoDelay = transitionDuration + researchDuration,
            });

        yield return new WaitForSeconds(transitionDuration);

        _researchScannableEffect.PlayScanEffect();

//        yield return new WaitForSeconds(researchDuration);

//        CinemachineCameraManager.Instance.SwitchToPreviousCameraState();
    }
}