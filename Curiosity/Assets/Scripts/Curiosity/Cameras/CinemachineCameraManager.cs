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
        EventLock,
        FirstPerson,
        Research,
    }

    public static CinemachineCameraManager Instance;

    public CinemachineCameraState CurrentState => _currentState;

    public StatefulCinemachineCamera CurrentStatefulCinemachineCamera
    {
        get
        {
            foreach (StatefulCinemachineCamera statefulCinemachineCamera in _statefulCinemachineCameras)
            {
                if (statefulCinemachineCamera.cinemachineCameraState == CurrentState)
                {
                    return statefulCinemachineCamera;
                }
            }

            return null;
        }
    }

    [SerializeField] private CinemachineCameraState _currentState = CinemachineCameraState.ThirdPerson;
    [SerializeField] private CinemachineCameraState _prevReturnableState = CinemachineCameraState.None;

    private Dictionary<CinemachineCameraState, bool> _returnableStates = new Dictionary<CinemachineCameraState, bool>
    {
        {CinemachineCameraState.ThirdPerson, true},
        {CinemachineCameraState.FirstPerson, true},
        {CinemachineCameraState.OverTheShoulder, true},
    };

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
        if (_returnableStates.ContainsKey(_currentState) && _returnableStates[_currentState])
        {
            _prevReturnableState = _currentState;
        }

        _currentState = cinemachineCameraState;
        CheckAndSwitchCamera(stateData);
    }

    public void SwitchToPreviousCameraState()
    {
        SwitchCameraState(_prevReturnableState);
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