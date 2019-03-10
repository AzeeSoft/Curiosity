using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class ThirdPersonPlayerCamera : MonoBehaviour
{
    public Transform ThirdPersonVirtualCamera;
    public float followSpeed = 3f;
    public float rotationSpeed = 3f;

    public float minAngle = -60f;
    public float maxAngle = 60f;

    private CuriosityModel curiosityModel;
    
    #region Accessors

    #endregion

    private CameraInputController _cameraInputController;

    void Awake()
    {
        _cameraInputController = GetComponentInChildren<CameraInputController>();
    }

    // Use this for initialization
    void Start()
    {
        curiosityModel = LevelManager.Instance.CuriosityModel;
        curiosityModel.thirdPersonPlayerCamera = this;
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

        ThirdPersonVirtualCamera.transform.LookAt(curiosityModel.CamTarget.position);
    }
}