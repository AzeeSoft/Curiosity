using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class StatefulCinemachineCamera : MonoBehaviour
{
    [Serializable]
    public class StatefulCinemachineCameraEvent : UnityEvent<StatefulCinemachineCamera>
    {
    }

    public CinemachineCameraManager.CinemachineCameraState cinemachineCameraState;
    public StatefulCinemachineCameraEvent OnActivated;
    public StatefulCinemachineCameraEvent OnDeactivated;

    public object stateData { get; private set; }

    private CinemachineVirtualCameraBase _virtualCamera;

    public bool IsActive { get; private set; }

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCameraBase>();
    }

    public void Activate(object stateData = null)
    {
        IsActive = true;
        this.stateData = stateData;
        _virtualCamera.enabled = true;
        OnActivated.Invoke(this);
    }

    public void Deactivate()
    {
        IsActive = false;
        stateData = null;
        _virtualCamera.enabled = false;
        OnDeactivated.Invoke(this);
    }
}