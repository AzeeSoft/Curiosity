using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityMovementController : MonoBehaviour
{
    [Serializable]
    public class WheelSetup
    {
        public GameObject WheelObject;
        public float radius;

        [HideInInspector] public Wheel wheel;

        public void SetupWheel()
        {
            wheel = WheelObject.AddComponent<Wheel>();
            wheel.radius = radius;
        }
    }

    [Header("Required Objects")] public WheelSetup FrontLeftWheelSetup;
    public WheelSetup MiddleLeftWheelSetup;
    public WheelSetup BackLeftWheelSetup;
    public WheelSetup FrontRightWheelSetup;
    public WheelSetup MiddleRightWheelSetup;
    public WheelSetup BackRightWheelSetup;
    public GameObject Avatar;
    public GameObject Body;

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
    public Transform FloorAlignTarget;
    public float GroundHugMaxDistance = 3f;
    public float GroundHugMinDistance = 2f;
    public float FloorAlignSpeed = 2f;
    public float GroundHugSpeed = 2f;

    public Vector3 bodyOffset;

    private Rigidbody _rigidbody;
    private CuriosityInputController _curiosityInputController;
    private List<Wheel> wheels = new List<Wheel>();

    private float _currentRotationAngle = 0;
    private bool _invertTurn = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _curiosityInputController = GetComponent<CuriosityInputController>();

        FrontLeftWheelSetup.SetupWheel();
        MiddleLeftWheelSetup.SetupWheel();
        BackLeftWheelSetup.SetupWheel();
        FrontRightWheelSetup.SetupWheel();
        MiddleRightWheelSetup.SetupWheel();
        BackRightWheelSetup.SetupWheel();

        wheels.Add(FrontLeftWheelSetup.wheel);
        wheels.Add(MiddleLeftWheelSetup.wheel);
        wheels.Add(BackLeftWheelSetup.wheel);
        wheels.Add(FrontRightWheelSetup.wheel);
        wheels.Add(MiddleRightWheelSetup.wheel);
        wheels.Add(BackRightWheelSetup.wheel);
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
//        AlignWithFloor();
//        HugWithFloor();
        StayWithWheels();
    }

    void OnDrawGizmos()
    {
    }

    /*public void Move(CuriosityInputController.CuriosityInput curiosityInput)
    {
        // Forward/Backward Movement
        // Clamping negative value to slow down backward movement
        float forward = Mathf.Clamp(curiosityInput.Forward, -MaxReverseThreshold, 1f);

        Vector3 yLessForward = transform.forward;
        Vector3 targetVelocity = yLessForward * forward * MaxSpeed;
        
        _rigidbody.AddForce(targetVelocity);
    }*/

    public void Move(CuriosityInputController.CuriosityInput curiosityInput)
    {
        // Forward/Backward Movement
        // Clamping negative value to slow down backward movement
        float forward = Mathf.Clamp(curiosityInput.Forward, -MaxReverseThreshold, 1f);

        Vector3 yLessForward = Avatar.transform.forward;
//        yLessForward.y = 0;

        Vector3 targetVelocity = yLessForward * forward * MaxSpeed;

        bool slowingDown = targetVelocity.magnitude < _rigidbody.velocity.magnitude;
        _rigidbody.velocity =
            Vector3.Lerp(_rigidbody.velocity, targetVelocity,
                (slowingDown ? Deceleration : Acceleration) * Time.fixedDeltaTime);

//        transform.Translate(targetVelocity);

        // Turning
        Vector3 glideDir = _rigidbody.velocity;
        glideDir.y = 0;

        float glideAngle = Vector3.Angle(Avatar.transform.forward, glideDir);
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

        FrontLeftWheelSetup.wheel.RotateWheel(wheelAngle);
        FrontRightWheelSetup.wheel.RotateWheel(wheelAngle);

        float targetAngle = curiosityInput.Turn * TurnSpeed * targetVelocity.magnitude;

        if (_invertTurn)
        {
            targetAngle *= -1;
        }

        bool slowingDownTurn = (Math.Abs(Mathf.Sign(targetAngle) - Mathf.Sign(_currentRotationAngle)) < 0.0001) &&
                               Mathf.Abs(targetAngle) < Mathf.Abs(_currentRotationAngle);

        _currentRotationAngle = Mathf.LerpAngle(_currentRotationAngle, targetAngle,
            (slowingDownTurn ? TurnDeceleration : TurnAcceleration) * Time.fixedDeltaTime);

        Avatar.transform.Rotate(transform.up, _currentRotationAngle);


        /*// Clamping the x and z rotations
        Vector3 rotationAngles = transform.rotation.eulerAngles;

        rotationAngles.x = HelperUtilities.ClampAngle(rotationAngles.x, -MaxRotation, MaxRotation);
        rotationAngles.z = HelperUtilities.ClampAngle(rotationAngles.z, -MaxRotation, MaxRotation);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(rotationAngles),
            Time.fixedDeltaTime * AntiRotationSpeed);*/
    }

    public float GetSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }

    void AlignWithFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(FloorAlignTarget.position, Vector3.down, out hit))
        {
            Vector3 targetUp = hit.normal;

//            transform.up = Vector3.Lerp(transform.up, targetUp, Time.fixedDeltaTime * FloorAlignSpeed);
//            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
        }
    }

    void HugWithFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 newPos = transform.position;
            if (hit.distance > GroundHugMaxDistance)
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y - (hit.distance - GroundHugMaxDistance),
                    Time.fixedDeltaTime * GroundHugSpeed);
            }
            else if (hit.distance < GroundHugMinDistance)
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y + (GroundHugMinDistance - hit.distance),
                    Time.fixedDeltaTime * GroundHugSpeed);
            }

            transform.position = newPos;
        }
    }

    void StayWithWheels()
    {
        Vector3 wheelsSuperPosition = Vector3.zero;
        foreach (Wheel wheel in wheels)
        {
            wheelsSuperPosition += wheel.transform.position;
        }

        wheelsSuperPosition /= wheels.Count;

        Body.transform.position = wheelsSuperPosition + bodyOffset;
    }
}