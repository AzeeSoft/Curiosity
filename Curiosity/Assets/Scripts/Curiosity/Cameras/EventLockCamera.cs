using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class EventLockCamera : MonoBehaviour
{
    public class StateData
    {
        public Transform LookAtTarget = null;
        public bool ZoomInOut = false;
        public float EventLockDuration;
    }

    public float ZoomTransitionDuration = 0.3f;
    public float leastZoomMinFOV = 15;
    public float farthestZoomDistance = 50f;
    public float EventDurationCorrection = 0.5f;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFollowZoom _cinemachineFollowZoom;
    private StatefulCinemachineCamera _statefulCinemachineCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineFollowZoom = GetComponent<CinemachineFollowZoom>();
        _statefulCinemachineCamera = GetComponent<StatefulCinemachineCamera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform curiosityBody = LevelManager.Instance.CuriosityModel.Body;
        _virtualCamera.Follow = curiosityBody;

        _statefulCinemachineCamera.OnActivated.AddListener((statefulCamera) =>
        {
            StateData stateData = (StateData) statefulCamera.stateData;

            if (stateData != null)
            {
                if (stateData.LookAtTarget)
                {
                    _virtualCamera.LookAt = stateData.LookAtTarget;

                    if (stateData.ZoomInOut)
                    {
                        float zoomMinFOV = HelperUtilities.Remap(
                            Vector3.Distance(transform.position, stateData.LookAtTarget.position), 0,
                            farthestZoomDistance,
                            _cinemachineFollowZoom.m_MinFOV, leastZoomMinFOV);
                        zoomMinFOV = Mathf.Clamp(zoomMinFOV, leastZoomMinFOV, _cinemachineFollowZoom.m_MinFOV);
                        StartCoroutine(ZoomInOut(stateData.EventLockDuration, zoomMinFOV));
                    }
                }
            }
        });
        _statefulCinemachineCamera.OnDeactivated.AddListener((statefulCamera) => { _virtualCamera.LookAt = null; });
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator ZoomInOut(float totalDuration, float zoomMinFOV)
    {
        float elapsedTime = 0;
        float originalMinFOV = _cinemachineFollowZoom.m_MinFOV;

        while (_cinemachineFollowZoom.m_MinFOV > zoomMinFOV)
        {
            _cinemachineFollowZoom.m_MinFOV =
                Mathf.Lerp(originalMinFOV, zoomMinFOV, elapsedTime * (1 / ZoomTransitionDuration));

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(totalDuration - (2 * ZoomTransitionDuration) - EventDurationCorrection);

        elapsedTime = 0;
        while (_cinemachineFollowZoom.m_MinFOV < originalMinFOV)
        {
            _cinemachineFollowZoom.m_MinFOV =
                Mathf.Lerp(zoomMinFOV, originalMinFOV, elapsedTime * (1 / ZoomTransitionDuration));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
}