using System.Collections;
using System.Collections.Generic;
using BasicTools.ButtonInspector;
using Cinemachine;
using UnityEngine;

[ExecuteInEditMode]
public class CinemachineCameraSwitcher : MonoBehaviour
{
    public enum CinemachineCameraState
    {
        ThirdPlayer,
        OverTheShoulder,
    }

    [System.Serializable]
    public struct StateMapping
    {
        public CinemachineCameraState State;
        public CinemachineVirtualCameraBase VirtualCamera;
    }

    public CinemachineCameraState CurrentState = CinemachineCameraState.ThirdPlayer;

    [SerializeField] private List<StateMapping> _stateMappings = new List<StateMapping>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        CheckAndSwitchCamera();
    }

    void CheckAndSwitchCamera()
    {
        foreach (StateMapping stateMapping in _stateMappings)
        {
            stateMapping.VirtualCamera.enabled = CurrentState == stateMapping.State;
        }
    }
}