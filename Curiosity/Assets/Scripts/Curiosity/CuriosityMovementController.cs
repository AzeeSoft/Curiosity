using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Audio;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class CuriosityMovementController : MonoBehaviour
{
    [Serializable]
    public class WheelSetup
    {
        public GameObject WheelRoot;
        public GameObject WheelHolder;
        public GameObject WheelParentJoint;
        public float radius;

        [HideInInspector] public Wheel wheel;

        public void SetupWheel(GameObject trailPrefab = null)
        {
            if (trailPrefab)
            {
                Instantiate(trailPrefab, WheelRoot.transform);
            }

            wheel = WheelRoot.AddComponent<Wheel>();
            wheel.radius = radius;
            wheel.WheelHolder = WheelHolder;
            wheel.WheelParentJoint = WheelParentJoint;
        }
    }

    [Serializable]
    private class GizmosData
    {
        public Vector3 wheelsSuperPosition = Vector3.zero;
        public Vector3 wheelsPlaneNormal = Vector3.up;

        public Vector3 frontWheelsNormal = Vector3.up;
        public Vector3 backWheelsNormal = Vector3.up;

        public Vector3 bodyForward = Vector3.forward;
    }

    [Header("Required Objects")] public WheelSetup FrontLeftWheelSetup;
    public WheelSetup MiddleLeftWheelSetup;
    public WheelSetup BackLeftWheelSetup;
    public WheelSetup FrontRightWheelSetup;
    public WheelSetup MiddleRightWheelSetup;
    public WheelSetup BackRightWheelSetup;
    public GameObject WheelTrailPrefab;
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

    [Header("Wheel Data")] public float GroundHugMaxDistance = 3f;
    public float GroundHugSpeed = 30f;
    public float Gravity = 10f;
    public float GravityModifier = 0.75f;
    public float MaxWheelSpinSpeed = 30f;

    [Header("Others")] public float MaxRotation = 2f;
    public float AntiRotationSpeed = 3f;

    public Vector3 bodyOffset;

    private Rigidbody _rigidbody;
    private CuriosityModel _curiosityModel;
    private CuriosityInputController _curiosityInputController;
    private ThrusterController _thrusterController;
    private List<Wheel> wheels = new List<Wheel>();

    private AudioSource roverAudioSource => _curiosityModel.curiosityAudio.roverAudioSource;
    private AudioSource gravelAudioSource => _curiosityModel.curiosityAudio.gravelAudioSource;

    private float _currentRotationAngle = 0;
    private bool _invertTurn = false;

    private GizmosData _gizmosData = new GizmosData();

    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _curiosityModel = GetComponent<CuriosityModel>();
        _curiosityInputController = GetComponent<CuriosityInputController>();
        _thrusterController = GetComponent<ThrusterController>();

        FrontLeftWheelSetup.SetupWheel();
        MiddleLeftWheelSetup.SetupWheel();
        BackLeftWheelSetup.SetupWheel(WheelTrailPrefab);
        FrontRightWheelSetup.SetupWheel();
        MiddleRightWheelSetup.SetupWheel();
        BackRightWheelSetup.SetupWheel(WheelTrailPrefab);

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
        if (_curiosityModel.IsAlive())
        {
            Move(_curiosityInputController.GetPlayerInput());
        }

        StayWithWheels();
        UpdateWheelSpinning();
    }

    private void LateUpdate()
    {
        ClampAngles();
    }

    void OnDrawGizmos()
    {
        try
        {
            Gizmos.color = Color.green;

            Vector3 frontStart = (FrontLeftWheelSetup.wheel.transform.position +
                                  FrontRightWheelSetup.wheel.transform.position) / 2;
            Vector3 middleStart = (MiddleLeftWheelSetup.wheel.transform.position +
                                   MiddleRightWheelSetup.wheel.transform.position) / 2;
            Vector3 backStart = (BackLeftWheelSetup.wheel.transform.position +
                                 BackRightWheelSetup.wheel.transform.position) / 2;

            Gizmos.DrawRay(frontStart, _gizmosData.frontWheelsNormal);
            Gizmos.DrawRay(backStart, _gizmosData.backWheelsNormal);


            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(_gizmosData.wheelsSuperPosition,
                _gizmosData.wheelsPlaneNormal);

            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(Body.transform.position,
                _gizmosData.bodyForward * 3);
        }
        catch (Exception e)
        {
        }
    }

    public void Move(CuriosityInputController.CuriosityInput curiosityInput)
    {
        // Forward/Backward Movement
        // Clamping negative value to slow down backward movement
        float forward = Mathf.Clamp(curiosityInput.Forward, -MaxReverseThreshold, 1f);

        Vector3 yLessAvatarForward = Avatar.transform.forward;
        yLessAvatarForward.y = 0;

        Vector3 targetVelocity = yLessAvatarForward * forward * MaxSpeed;
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

        FrontLeftWheelSetup.wheel.TurnWheel(wheelAngle);
        FrontRightWheelSetup.wheel.TurnWheel(wheelAngle);

        float targetAngle = curiosityInput.Turn * TurnSpeed * targetVelocity.magnitude;

        if (_invertTurn)
        {
            targetAngle *= -1;
        }

        // TODO (Azee): Decide whether this turn slowing down is even needed
        /*bool slowingDownTurn = (Math.Abs(Mathf.Sign(targetAngle - _currentRotationAngle)) > 0) &&
                               Mathf.Abs(targetAngle) < Mathf.Abs(_currentRotationAngle);*/

//        bool slowingDownTurn = (Math.Abs(Mathf.Sign(targetAngle - _currentRotationAngle)) < 15f);
        bool slowingDownTurn = false;

        _currentRotationAngle = Mathf.LerpAngle(_currentRotationAngle, targetAngle,
            (slowingDownTurn ? TurnDeceleration : TurnAcceleration) * Time.fixedDeltaTime);

        Avatar.transform.Rotate(transform.up, _currentRotationAngle);
    }

    private void ClampAngles()
    {
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

    public bool AreWheelsOnGround()
    {
        return wheels.TrueForAll((wheel) => wheel.onGround);
    }

    void UpdateAudioSources()
    {
        float audioVolume = HelperUtilities.Remap(GetSpeed(), 0, MaxSpeed, 0, 1);

        if (!AreWheelsOnGround())
        {
            audioVolume = 0;
        }

        float audioFadeSpeed = 7;

        roverAudioSource.volume = Mathf.Lerp(roverAudioSource.volume, audioVolume, Time.deltaTime * audioFadeSpeed);
        gravelAudioSource.volume = Mathf.Lerp(gravelAudioSource.volume, audioVolume, Time.deltaTime * audioFadeSpeed);
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
        if (AreWheelsOnGround())
        {
            foreach (ParticleSystem dustParticleSystem in DustParticleSystems)
            {
                ParticleSystem.MainModule mainModule = dustParticleSystem.main;
                int emitParticles = (int) HelperUtilities.Remap(GetSpeed(), 0, MaxSpeed, 0, 25);
                dustParticleSystem.Emit(emitParticles);
            }
        }
    }

    void UpdateHeadRotation()
    {
        if (!Camera.current)
        {
            return;
        }

        Transform thirdPersonCameraTransform = Camera.current.transform;

        Quaternion targetRotation = Quaternion.Euler(thirdPersonCameraTransform.rotation.eulerAngles.x - 30,
            thirdPersonCameraTransform.rotation.eulerAngles.y, 0);
        HeadRotX.transform.rotation = Quaternion.Lerp(HeadRotX.transform.rotation, targetRotation, Time.deltaTime);
    }

    void StayWithWheels()
    {
        /* Adjusting Body Offset */

        Vector3 wheelsSuperPosition = Vector3.zero;
        foreach (Wheel wheel in wheels)
        {
            wheelsSuperPosition += wheel.WheelHolder.transform.position;
        }

        wheelsSuperPosition /= wheels.Count;

        Body.transform.position = wheelsSuperPosition;
        Body.transform.localPosition += bodyOffset;


        /* Calculating forward for body */

        Vector3 frontSuperPos = (FrontLeftWheelSetup.wheel.transform.position +
                                 FrontRightWheelSetup.wheel.transform.position) / 2;

        Vector3 backSuperPos = (BackLeftWheelSetup.wheel.transform.position +
                                BackRightWheelSetup.wheel.transform.position) / 2;


        Vector3 bodyForward = (frontSuperPos - backSuperPos).normalized;


        /* Calculating Normal to Wheels Plane */

        Vector3 frontDir = (FrontLeftWheelSetup.wheel.transform.position -
                            FrontRightWheelSetup.wheel.transform.position).normalized;

        Vector3 backDir = (BackLeftWheelSetup.wheel.transform.position -
                           BackRightWheelSetup.wheel.transform.position).normalized;

        Vector3 frontNormal = Vector3.Cross(frontDir, bodyForward).normalized;
        Vector3 backNormal = Vector3.Cross(backDir, bodyForward).normalized;

        if (frontNormal.y < 0)
        {
            frontNormal *= -1;
        }

        if (backNormal.y < 0)
        {
            backNormal *= -1;
        }

        Vector3 normalToPlane = (frontNormal + backNormal).normalized;

        /*float turnAngle = Vector3.SignedAngle(Vector3.forward, Avatar.transform.forward, Vector3.up) % 360;
        if (turnAngle > 180)
        {
            turnAngle = turnAngle - 360;
        }

//        Debug.Log("Turn Angle: " + turnAngle);
        if (turnAngle < -37 || turnAngle > 143)
        {
            normalToPlane *= -1;
        }

        if (normalToPlane.y < 0)
        {
            normalToPlane *= -1;
        }*/


        /* Assigning Body Rotation */

        Quaternion newRotation = Quaternion.LookRotation(bodyForward, normalToPlane);
        Body.transform.rotation = newRotation;

        Vector3 eulerAngles = Body.transform.localRotation.eulerAngles;
        eulerAngles.y = 0;
        Quaternion newLocalRotation = Quaternion.Euler(eulerAngles);
        Body.transform.localRotation = newLocalRotation;


        /* Preparing data for Gizmos */

        _gizmosData.wheelsSuperPosition = wheelsSuperPosition;
        _gizmosData.wheelsPlaneNormal = normalToPlane;
        _gizmosData.frontWheelsNormal = frontNormal;
        _gizmosData.backWheelsNormal = backNormal;
        _gizmosData.bodyForward = bodyForward;
    }
}