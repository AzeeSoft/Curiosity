﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraInputController : MonoBehaviour
{
    public class CameraInputSettings
    {
        public float lookXSensitivity = 3.0f;
        public float lookYSensitivity = 3.0f;
        public bool invertLookY = false;
    }

    [Serializable]
    public struct CameraInput
    {
        public float camHorizontal;
        public float camVertical;
    }

    [SerializeField] private CameraInputSettings _cameraInputSettings = new CameraInputSettings();
    [ReadOnly] [SerializeField] private CameraInput _cameraInput = new CameraInput();

    void Update()
    {
        UpdateCameraInput();
    }

    public CameraInput GetCameraInput()
    {
        return _cameraInput;
    }

    void UpdateCameraInput()
    {
        _cameraInput.camHorizontal = _cameraInputSettings.lookXSensitivity * Input.GetAxis("Look X");
        _cameraInput.camVertical = _cameraInputSettings.lookYSensitivity * Input.GetAxis("Look Y") *
                                   (_cameraInputSettings.invertLookY ? 1 : -1);
    }
}
