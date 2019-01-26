using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonPlayerCamera : MonoBehaviour
{
    public CuriosityModel curiosityModel;
    public float followSpeed = 3f;
    public float rotationSpeed = 3f;

    public float minAngle = -60f;
    public float maxAngle = 60f;

    #region Accessors

    public new Camera camera
    {
        get { return _camera; }
    }

    #endregion

    private Camera _camera;
    private CameraInputController _cameraInputController;

    void Awake()
    {
        _camera = GetComponentInChildren<Camera>();
        _cameraInputController = GetComponentInChildren<CameraInputController>();
    }

    // Use this for initialization
    void Start()
    {
        curiosityModel.ThirdPersonPlayerCamera = this;
        HelperUtilities.UpdateCursorLock(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePosition();
        UpdateRotation();
    }

    void UpdatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, curiosityModel.CamTarget.position,
            followSpeed * Time.fixedDeltaTime);
    }

    void UpdateRotation()
    {
        CameraInputController.CameraInput playerInput = _cameraInputController.GetCameraInput();

        Vector3 targetRotationAngle = transform.localRotation.eulerAngles;
        targetRotationAngle.x += playerInput.camVertical;
        targetRotationAngle.y += playerInput.camHorizontal;

        targetRotationAngle.x = HelperUtilities.ClampAngle(targetRotationAngle.x, minAngle, maxAngle);
        targetRotationAngle.y = Mathf.Repeat(targetRotationAngle.y, 360f);

        transform.rotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(targetRotationAngle),
            Time.fixedDeltaTime * rotationSpeed);

//        transform.Rotate(playerInput.camVertical, playerInput.camHorizontal, 0);

        _camera.transform.LookAt(curiosityModel.CamTarget.position);
    }
}