using Cinemachine;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    public float camRotationSpeed;
    public float minCamAngle;
    public float maxCamAngle;

    private CinemachineVirtualCamera _virtualCamera;
    private CameraInputController _cameraInputController;
    private StatefulCinemachineCamera _statefulCinemachineCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _cameraInputController = GetComponent<CameraInputController>();
        _statefulCinemachineCamera = GetComponent<StatefulCinemachineCamera>();
    }

    private void Start()
    {
        _virtualCamera.Follow = LevelManager.Instance.CuriosityModel.curiosityMovementController.HeadRotX.transform;
    }

    private void FixedUpdate()
    {
        RotateCamera();
    }

    void RotateCamera()
    {
        CameraInputController.CameraInput playerInput = _cameraInputController.GetCameraInput();

        Vector3 targetRotationAngle = transform.localRotation.eulerAngles;
        targetRotationAngle.x += playerInput.camVertical;
        targetRotationAngle.y += playerInput.camHorizontal;

        targetRotationAngle.x = HelperUtilities.ClampAngle(targetRotationAngle.x, minCamAngle, maxCamAngle);
        targetRotationAngle.y = Mathf.Repeat(targetRotationAngle.y, 360f);
        targetRotationAngle.z = 0;

        transform.localRotation = Quaternion.Slerp(transform.localRotation,
            Quaternion.Euler(targetRotationAngle),
            Time.fixedDeltaTime * camRotationSpeed);
    }
}