using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ResearchScannableEffect))]
public class ResearchScannable : MonoBehaviour
{
    public ResearchItemData researchItemData;
    public Transform researchCameraPosition;

    private ResearchScannableEffect _researchScannableEffect;
    private Interactable _interactable;

    private void Awake()
    {
        _researchScannableEffect = GetComponent<ResearchScannableEffect>();
        _interactable = GetComponentInParent<Interactable>();
        foreach (var interactableInteraction in _interactable.interactions)
        {
            if (interactableInteraction.type == Interactable.InteractionType.Primary)
            {
                interactableInteraction.interactableUi.icon.sprite = researchItemData.sprite;
            }
        }
    }

    public void StartResearch()
    {
        StartCoroutine(TriggerResearch());
    }

    IEnumerator TriggerResearch()
    {
        float transitionDuration = 1.5f;
        float researchDuration = _researchScannableEffect.scanAnimationDuration - 0.5f;

        CinemachineCameraManager.Instance.SwitchCameraState(
            CinemachineCameraManager.CinemachineCameraState.Research, new ResearchCamera.StateData()
            {
                LookFromTarget = researchCameraPosition,
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