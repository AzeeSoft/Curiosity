using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine;

public class TriggerRockFall : MonoBehaviour
{
    public GameObject target;
    public PlayableDirector timeline;
    private bool triggered = false;
    public float time;

    private void OnTriggerEnter(Collider other)
    {

        if(other.tag == "Player" && !triggered)
        {
            timeline.Play();
            StartCoroutine("CameraSwitch");
            
         }
        
    }

    IEnumerator CameraSwitch()
    {

        CinemachineCameraManager.Instance.SwitchCameraState(
        CinemachineCameraManager.CinemachineCameraState.EventLock, new EventLockCamera.StateData()
        {
            LookAtTarget = target.transform,
            ZoomInOut = true,
            EventLockDuration = time
        });

        yield return new WaitForSeconds(time);

        CinemachineCameraManager.Instance.SwitchToPreviousCameraState();
        triggered = true;
    }
}
