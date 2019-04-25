using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResearchCamera : MonoBehaviour
{
    public class StateData
    {
        public Transform LookAtTarget = null;
        public bool ZoomInOut = false;
        public float EventLockDuration;
        public ResearchItemData researchItemData;
        public float showInfoDelay = 5f;
    }

    public float ZoomTransitionDuration = 0.3f;
    public float leastZoomMinFOV = 15;
    public float farthestZoomDistance = 50f;
    public float EventDurationCorrection = 0.5f;
    public Canvas ResearchCanvas;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public Image spriteImage;

    private CinemachineVirtualCamera _virtualCamera;
    private CinemachineFollowZoom _cinemachineFollowZoom;
    private StatefulCinemachineCamera _statefulCinemachineCamera;

    private bool canExitResearchMode = false;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cinemachineFollowZoom = GetComponent<CinemachineFollowZoom>();
        _statefulCinemachineCamera = GetComponent<StatefulCinemachineCamera>();

        ResearchCanvas.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        Transform curiosityBody = LevelManager.Instance.CuriosityModel.Body;
        _virtualCamera.Follow = curiosityBody;

        _statefulCinemachineCamera.OnActivated.AddListener((statefulCamera) =>
        {
            canExitResearchMode = false;
            
            StateData stateData = (StateData) statefulCamera.stateData;
            
            title.text = stateData.researchItemData.title;
            description.text = stateData.researchItemData.description;
            spriteImage.sprite = stateData.researchItemData.sprite;

            StartCoroutine(ShowDetailsAfterDelay(stateData.showInfoDelay));

            if (stateData != null)
            {
                if (stateData.LookAtTarget)
                {
                    _virtualCamera.transform.position =
                        stateData.LookAtTarget.position + (stateData.LookAtTarget.forward * 1);
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
        _statefulCinemachineCamera.OnDeactivated.AddListener((statefulCamera) =>
        {
            _virtualCamera.LookAt = null;
            ResearchCanvas.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        if (canExitResearchMode)
        {
            if (Input.GetButtonDown("Continue"))
            {
                CinemachineCameraManager.Instance.SwitchToPreviousCameraState();
                canExitResearchMode = false;
            }
        }
    }

    IEnumerator ShowDetailsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResearchCanvas.gameObject.SetActive(true);

        canExitResearchMode = true;
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