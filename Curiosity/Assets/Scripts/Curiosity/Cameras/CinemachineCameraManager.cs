using System;
using System.Collections;
using System.Collections.Generic;
using BasicTools.ButtonInspector;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class CinemachineCameraManager : MonoBehaviour
{
    [Serializable]
    public class OnCinemachineCameraStateUpdated : UnityEvent<CinemachineCameraState>
    {
    }

    public enum CinemachineCameraState
    {
        None,
        ThirdPerson,
        OverTheShoulder,
        EventLock
    }

    public static CinemachineCameraManager Instance;

    [SerializeField] private CinemachineCameraState _currentState = CinemachineCameraState.ThirdPerson;
    [SerializeField] private CinemachineCameraState _prevState = CinemachineCameraState.None;

    [SerializeField]
    private List<StatefulCinemachineCamera> _statefulCinemachineCameras = new List<StatefulCinemachineCamera>();

    public OnCinemachineCameraStateUpdated onCinemachineCameraStateUpdated;

    [SerializeField] [Button("Refresh Stateful Cameras", "RefreshStatefulCameras")]
    private bool _refreshStatefulCameras;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        RefreshStatefulCameras();
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndSwitchCamera();
    }

    void CheckAndSwitchCamera(object stateData = null)
    {
        List<StatefulCinemachineCamera> camerasToDeactivate = new List<StatefulCinemachineCamera>();
        List<StatefulCinemachineCamera> camerasToActivate = new List<StatefulCinemachineCamera>();

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in _statefulCinemachineCameras)
        {
            if (statefulCinemachineCamera.cinemachineCameraState == _currentState)
            {
                if (!statefulCinemachineCamera.IsActive)
                {
                    camerasToActivate.Add(statefulCinemachineCamera);
                }
            }
            else
            {
                if (statefulCinemachineCamera.IsActive)
                {
                    camerasToDeactivate.Add(statefulCinemachineCamera);
                }
            }
        }

        if (camerasToActivate.Count > 0 || camerasToDeactivate.Count > 0)
        {
            onCinemachineCameraStateUpdated.Invoke(_currentState);
        }

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in camerasToDeactivate)
        {
            statefulCinemachineCamera.Deactivate();
        }

        foreach (StatefulCinemachineCamera statefulCinemachineCamera in camerasToActivate)
        {
            statefulCinemachineCamera.Activate(stateData);
        }
    }

    void RefreshStatefulCameras()
    {
        _statefulCinemachineCameras.Clear();
        _statefulCinemachineCameras.AddRange(GetComponentsInChildren<StatefulCinemachineCamera>());
    }

    public void SwitchCameraState(CinemachineCameraState cinemachineCameraState, object stateData = null)
    {
        _prevState = _currentState;
        _currentState = cinemachineCameraState;
        CheckAndSwitchCamera(stateData);
    }

    public void SwitchToPreviousCameraState()
    {
        SwitchCameraState(_prevState);
    }
    
    public void OnCameraCut(CinemachineBrain cinemachineBrain)
    {
        Debug.Log("Camera Cut");
    }

    public void OnCameraActivated(ICinemachineCamera to, ICinemachineCamera from)
    {
        if (from != null)
        {
            Debug.Log("From: " + from.Name);
        }

        if (to != null)
        {
            Debug.Log("To: " + to.Name);
        }
    }
}