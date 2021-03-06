using Cinemachine;
using UnityEngine;

public class OverTheShoulderCamera : MonoBehaviour
{
    public class StateData
    {
        public bool enablePlayerInput = false;
    }

    public float FollowSmoothness = 1f;
    public float LookSmoothness = 1f;

    private CinemachineVirtualCamera _virtualCamera;
    private StatefulCinemachineCamera _statefulCinemachineCamera;
    private Vector3 _offsetInCuriosity;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _statefulCinemachineCamera = GetComponent<StatefulCinemachineCamera>();
    }

    private void Start()
    {
        Transform curiosityBody = LevelManager.Instance.CuriosityModel.Body;
        _virtualCamera.Follow = curiosityBody;

        Vector3 posInCuriosity = curiosityBody.transform.InverseTransformPoint(transform.position);
        _offsetInCuriosity = curiosityBody.transform.position - posInCuriosity;

        _statefulCinemachineCamera.OnActivated.AddListener((statefulCamera) =>
        {
            StateData stateData = (StateData) statefulCamera.stateData;

            if (stateData != null)
            {
                LevelManager.Instance.CuriosityModel.UpdatePlayerInputState(stateData.enablePlayerInput);
            }
        });
        _statefulCinemachineCamera.OnDeactivated.AddListener((statefulCamera) => { _virtualCamera.LookAt = null; });
    }

    private void FixedUpdate()
    {
//        MoveWithCuriosity();
//        LookTowardsCuriosityForward();
    }

    void MoveWithCuriosity()
    {
        Transform curiosityBody = LevelManager.Instance.CuriosityModel.Body;

        Vector3 targetPosInCuriosity = curiosityBody.transform.position - _offsetInCuriosity;
        Vector3 targetPos = curiosityBody.transform.TransformPoint(targetPosInCuriosity);
        transform.position = Vector3.Lerp(transform.position, targetPos, FollowSmoothness);
    }

    void LookTowardsCuriosityForward()
    {
        Vector3 targetForward = LevelManager.Instance.CuriosityModel.transform.forward;
        transform.forward = Vector3.Lerp(transform.forward, targetForward, Time.deltaTime * LookSmoothness);
    }
}