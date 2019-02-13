using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

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
    public GameObject HeadRotY;
    public GameObject HeadRotX;
    public ParticleSystem[] DustParticleSystems;

    [Header("Forward/Backward Movement")] public float MaxSpeed = 25;
    public float MaxReverseThreshold = 0.8f;
    public float Acceleration = 3;
    public float Deceleration = 1;
    public float BoostFactor = 2f;

    [Header("Turning")] public float TurnSpeed = 0.1f;
    public float TurnAcceleration = 8;
    public float TurnDeceleration = 5;
    public float WheelTurnModifier = 60;

    [Header("TiltLimits")] public float MaxZTilt = 60f;

    [Header("Others")] public float MaxRotation = 2f;
    public float AntiRotationSpeed = 3f;
    public Transform FloorAlignTarget;
    public float GroundHugMaxDistance = 3f;
    public float GroundHugMinDistance = 2f;
    public float MaxWheelSpinSpeed = 30f;
    public AudioSource roverAudioSource;
    public AudioSource gravelAudioSource;

    public Vector3 bodyOffset;

    private Rigidbody _rigidbody;
    private CuriosityModel _curiosityModel;
    private CuriosityInputController _curiosityInputController;
    private List<Wheel> wheels = new List<Wheel>();

    private float _currentRotationAngle = 0;
    private bool _invertTurn = false;

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _curiosityModel = GetComponent<CuriosityModel>();
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
        UpdateAudioSources();
        UpdateDustParticles();
        UpdateHeadRotation();
    }

    void FixedUpdate()
    {
        if (_curiosityModel.Battery > 0)
        {
            Move(_curiosityInputController.GetPlayerInput());
        }

//        AlignWithFloor();
//        HugWithFloor();
        StayWithWheels();
        UpdateWheelSpinning();
    }

    void OnDrawGizmos()
    {
        /*Vector3 frontNormal = Vector3.Cross(FrontLeftWheelSetup.wheel.transform.position,
            FrontRightWheelSetup.wheel.transform.position);

        Vector3 middleNormal = Vector3.Cross(MiddleLeftWheelSetup.wheel.transform.position,
            MiddleRightWheelSetup.wheel.transform.position);

        Vector3 backNormal = Vector3.Cross(BackLeftWheelSetup.wheel.transform.position,
            BackRightWheelSetup.wheel.transform.position);

        Gizmos.color = Color.blue;

        Vector3 frontStart = (FrontLeftWheelSetup.wheel.transform.position +
                              FrontRightWheelSetup.wheel.transform.position) / 2;
        Vector3 middleStart = (MiddleLeftWheelSetup.wheel.transform.position +
                               MiddleRightWheelSetup.wheel.transform.position) / 2;
        Vector3 backStart = (BackLeftWheelSetup.wheel.transform.position +
                             BackRightWheelSetup.wheel.transform.position) / 2;

        Gizmos.DrawRay(frontStart, frontStart + frontNormal);
        Gizmos.DrawRay(middleStart, middleStart + middleNormal);
        Gizmos.DrawRay(backStart, backStart + backNormal);*/
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
        if (curiosityInput.Boost)
        {
            targetVelocity *= BoostFactor;
        }

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


        // Clamping the x and z rotations
        Vector3 rotationAngles = Avatar.transform.rotation.eulerAngles;

        rotationAngles.x = HelperUtilities.ClampAngle(rotationAngles.x, -MaxRotation, MaxRotation);
        rotationAngles.z = HelperUtilities.ClampAngle(rotationAngles.z, -MaxRotation, MaxRotation);

        Avatar.transform.rotation = Quaternion.Lerp(Avatar.transform.rotation, Quaternion.Euler(rotationAngles),
            Time.fixedDeltaTime * AntiRotationSpeed);
    }

    public float GetSpeed()
    {
        return _rigidbody.velocity.magnitude;
    }

    void UpdateAudioSources()
    {
        float audioVolume = HelperUtilities.Remap(GetSpeed(), 0, MaxSpeed, 0, 1);
        roverAudioSource.volume = audioVolume;
        gravelAudioSource.volume = audioVolume;
    }

    void UpdateWheelSpinning()
    {
        bool reverse = Vector3.SignedAngle(transform.forward, _rigidbody.velocity, transform.up) > 90;

        foreach (Wheel wheel in wheels)
        {
            wheel.UpdateSpinSpeed(HelperUtilities.Remap(GetSpeed(), 0, MaxSpeed, 0, MaxWheelSpinSpeed) *
                                  (reverse ? -1 : 1));
        }
    }

    void UpdateDustParticles()
    {
        foreach (ParticleSystem dustParticleSystem in DustParticleSystems)
        {
            ParticleSystem.MainModule mainModule = dustParticleSystem.main;
            int emitParticles = (int) HelperUtilities.Remap(GetSpeed(), 0, MaxSpeed, 0, 5);
            dustParticleSystem.Emit(emitParticles);
        }
    }

    void UpdateHeadRotation()
    {
        Transform thirdPersonCameraTransform = _curiosityModel.thirdPersonPlayerCamera.camera.transform;

//        HeadRotY.transform.rotation = Quaternion.Euler(0,thirdPersonCameraTransform.rotation.eulerAngles.y,0);
        Quaternion targetRotation = Quaternion.Euler(thirdPersonCameraTransform.rotation.eulerAngles.x - 30,
            thirdPersonCameraTransform.rotation.eulerAngles.y, 0);
        HeadRotX.transform.rotation = Quaternion.Lerp(HeadRotX.transform.rotation, targetRotation, Time.deltaTime);
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


        Vector3 frontNormal = Vector3.Cross(FrontLeftWheelSetup.wheel.transform.position,
            FrontRightWheelSetup.wheel.transform.position);

        Vector3 middleNormal = Vector3.Cross(MiddleLeftWheelSetup.wheel.transform.position,
            MiddleRightWheelSetup.wheel.transform.position);

        Vector3 backNormal = Vector3.Cross(BackLeftWheelSetup.wheel.transform.position,
            BackRightWheelSetup.wheel.transform.position);

        Vector3 normalToPlane = (frontNormal + middleNormal + backNormal) / 3;
//        normalToPlane.Normalize();

//        Debug.Log(Vector3.SignedAngle(Vector3.forward, Avatar.transform.forward, Vector3.up));
        float turnAngle = Vector3.SignedAngle(Vector3.forward, Avatar.transform.forward, Vector3.up) % 360;
        if (turnAngle > 180)
        {
            turnAngle = turnAngle - 360;
        }

//        Debug.Log("Turn Angle: " + turnAngle);
        if (turnAngle < -37 || turnAngle > 143)
        {
//            Debug.Log("Correcting from " + newLocalRotation.z);
            normalToPlane *= -1;
//            Debug.Log("Correcting to " + newLocalRotation.z);
        }

        if (normalToPlane.y < 0)
        {
            normalToPlane *= -1;
        }

        Quaternion newRotation = Quaternion.LookRotation(Body.transform.forward, normalToPlane);
        Body.transform.rotation = newRotation;

        Quaternion newLocalRotation = Body.transform.localRotation;
        newLocalRotation.y = 0;
        Body.transform.localRotation = newLocalRotation;
    }
}