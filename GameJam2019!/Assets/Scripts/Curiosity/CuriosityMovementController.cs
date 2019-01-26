using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityMovementController : MonoBehaviour
{
    [Header("Required Objects")] public GameObject FrontLeftWheelObject;
    public GameObject FrontRightWheelObject;

    [Header("Forward/Backward Movement")] public float MaxSpeed = 25;
    public float MaxReverseThreshold = 0.8f;
    public float Acceleration = 3;
    public float Deceleration = 1;

    [Header("Turning")] public float TurnSpeed = 0.1f;
    public float TurnAcceleration = 8;
    public float TurnDeceleration = 5;
    public float WheelTurnModifier = 60;

    [Header("Others")] public float MaxRotation = 2f;
    public float AntiRotationSpeed = 3f;

    private Rigidbody _rigidbody;
    private CuriosityInputController _curiosityInputController;
    private Wheel _frontLeftWheel;
    private Wheel _frontRightWheel;

    private float _currentRotationAngle = 0;
    private bool _invertTurn = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _curiosityInputController = GetComponent<CuriosityInputController>();

        _frontLeftWheel = FrontLeftWheelObject.AddComponent<Wheel>();
        _frontRightWheel = FrontRightWheelObject.AddComponent<Wheel>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void FixedUpdate()
    {
        Move(_curiosityInputController.GetPlayerInput());
    }

    void OnDrawGizmos()
    {
    }

    public void Move(CuriosityInputController.CuriosityInput curiosityInput)
    {
        // Forward/Backward Movement
        // Clamping negative value to slow down backward movement
        float forward = Mathf.Clamp(curiosityInput.Forward, -MaxReverseThreshold, 1f);

        Vector3 yLessForward = transform.forward;
        yLessForward.y = 0;

        Vector3 targetVelocity = yLessForward * forward * MaxSpeed;

        bool slowingDown = targetVelocity.magnitude < _rigidbody.velocity.magnitude;
        _rigidbody.velocity =
            Vector3.Lerp(_rigidbody.velocity, targetVelocity,
                (slowingDown ? Deceleration : Acceleration) * Time.fixedDeltaTime);

        // Turning
        Vector3 glideDir = _rigidbody.velocity;
        glideDir.y = 0;

        float glideAngle = Vector3.Angle(transform.forward, glideDir);
        if (glideAngle > 180)
        {
            glideAngle = glideAngle - 360;
        }

        if (targetVelocity.magnitude > 0)
        {
            _invertTurn = (_rigidbody.velocity.magnitude > 0) &&
                          Mathf.Abs(glideAngle) > 90f;
        }

        float wheelAngle = curiosityInput.Turn * WheelTurnModifier;
        
        _frontLeftWheel.RotateWheel(wheelAngle);
        _frontRightWheel.RotateWheel(wheelAngle);

        float targetAngle = curiosityInput.Turn * TurnSpeed * _rigidbody.velocity.magnitude;
        
        if (_invertTurn)
        {
            targetAngle *= -1;
        }
        

        bool slowingDownTurn = (Math.Abs(Mathf.Sign(targetAngle) - Mathf.Sign(_currentRotationAngle)) < 0.0001) &&
                               Mathf.Abs(targetAngle) < Mathf.Abs(_currentRotationAngle);

        _currentRotationAngle = Mathf.LerpAngle(_currentRotationAngle, targetAngle,
            (slowingDownTurn ? TurnDeceleration : TurnAcceleration) * Time.fixedDeltaTime);

        transform.Rotate(transform.up, _currentRotationAngle);


        // Clamping the x and z rotations
        Vector3 rotationAngles = transform.rotation.eulerAngles;

        rotationAngles.x = HelperUtilities.ClampAngle(rotationAngles.x, -MaxRotation, MaxRotation);
        rotationAngles.z = HelperUtilities.ClampAngle(rotationAngles.z, -MaxRotation, MaxRotation);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationAngles),
            Time.fixedDeltaTime * AntiRotationSpeed);
    }

    public float GetSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }
}