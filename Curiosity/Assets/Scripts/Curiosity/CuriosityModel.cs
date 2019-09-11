using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    public float SolarChargeRate = 5f;
    public float BatteryDepletionRate = 5f;
    public float ChargePadRechargeAmount = 100f;
    public float MaxResetDistanceFromGround = 10f;
    public List<CinemachineCameraManager.CinemachineCameraState> cyclableCameraStates = new List<CinemachineCameraManager.CinemachineCameraState>();

    [ReadOnly] public ThirdPersonPlayerCamera thirdPersonPlayerCamera;
    [HideInInspector] public Transform CamTarget => Body;

    [Header("Required References")] public GameObject Lens;
    public Transform Avatar;
    public Transform Body;

    [Header("Required Prefabs")] public GameObject SpotLightPrefab;
    public GameObject CuriosityColliderPrefab;
    public GameObject CuriosityAudioSourcePrefab;

    [Header("Debug")] public bool wheelsReposition = true;

    [HideInInspector] public CuriosityAudio curiosityAudio;

    public CuriosityInputController curiosityInputController { get; private set; }
    public CuriosityMovementController curiosityMovementController { get; private set; }
    public InteractionController interactionController { get; private set; }
    public InverseKinematicsController[] inverseKinematicsControllers { get; private set; }

    private GameObject _spotLightObject;

    public Animator _animator;
    private Sun _sun;

    void Awake()
    {
        curiosityInputController = GetComponent<CuriosityInputController>();
        curiosityMovementController = GetComponent<CuriosityMovementController>();
        interactionController = GetComponent<InteractionController>();
        inverseKinematicsControllers = GetComponents<InverseKinematicsController>();
        _spotLightObject = Instantiate(SpotLightPrefab, Lens.transform);

        Instantiate(CuriosityColliderPrefab, Body);

        curiosityAudio = Instantiate(CuriosityAudioSourcePrefab, Body).GetComponent<CuriosityAudio>();
        curiosityAudio.roverAudioSource.volume = 0;
        curiosityAudio.gravelAudioSource.volume = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        AvatarColliderGenerator avatarColliderGenerator = GetComponentInChildren<AvatarColliderGenerator>();
//        avatarColliderGenerator.GenerateMeshColliders();

        //_animator = GetComponentInChildren<Animator>();

        _sun = LevelManager.Instance.GetSun();
        _sun.OnSunStateChanged += newState => { RefreshSunState(newState); };
        RefreshSunState();

        CinemachineCameraManager.Instance.onCinemachineCameraStateUpdated.AddListener((state =>
        {
            switch (state)
            {
                case CinemachineCameraManager.CinemachineCameraState.FirstPerson:
                case CinemachineCameraManager.CinemachineCameraState.ThirdPerson:
                case CinemachineCameraManager.CinemachineCameraState.OverTheShoulder:
                    UpdatePlayerInputState(true);
                    break;
                default:
                    UpdatePlayerInputState(false);
                    break;
            }
        }));
    }

    // Update is called once per frame
    void Update()
    {
        CuriosityInputController.CuriosityInput input = curiosityInputController.GetPlayerInput();
        if (input.Respawn)
        {
            Respawn();
        }

        if (input.SwitchCamera)
        {
            SwitchCamera();
        }
    }

    public void Respawn(bool moveUp = true)
    {
        StartCoroutine(_Respawn(moveUp));
    }

    IEnumerator _Respawn(bool moveUp)
    {
        curiosityMovementController.DrawTrails = false;
        yield return new WaitForSeconds(0.3f);

        if (moveUp && Physics.Raycast(Body.position, Vector3.down, MaxResetDistanceFromGround))
        {
            Vector3 targetPos = transform.position;
            targetPos.y += 10f;
            transform.position = targetPos;
        }

        Vector3 targetEulerAngles = Body.rotation.eulerAngles;
        targetEulerAngles.x = 0;
        targetEulerAngles.z = 0;
        Body.rotation = Quaternion.Euler(targetEulerAngles);

        curiosityMovementController.ResetWheels();
        foreach (var inverseKinematicsController in inverseKinematicsControllers)
        {
            inverseKinematicsController.ResetIK();
        }

        yield return new WaitForSeconds(0.3f);

        curiosityMovementController.DrawTrails = true;
    }

    void SwitchCamera()
    {
        if (cyclableCameraStates.Count > 0)
        {
            int curCyclableState = -1;
            for (int i = 0; i < cyclableCameraStates.Count; i++)
            {
                if (cyclableCameraStates[i] == CinemachineCameraManager.Instance.CurrentState)
                {
                    curCyclableState = i;
                    break;
                }
            }

            if (curCyclableState >= 0)
            {
                curCyclableState = (curCyclableState + 1) % cyclableCameraStates.Count;
                CinemachineCameraManager.Instance.SwitchCameraState(cyclableCameraStates[curCyclableState]);
            }
        }
    }

    void RefreshSunState(Sun.SunState? curSunState = null)
    {
        if (!curSunState.HasValue)
        {
            curSunState = _sun.GetSunState();
        }

        UpdateSpotLight(curSunState.Value);
        UpdateSolarPanels(curSunState.Value);
    }

    void UpdateSpotLight(Sun.SunState curSunState)
    {
        _spotLightObject.SetActive(curSunState != Sun.SunState.Day);
    }

    void UpdateSolarPanels(Sun.SunState curSunState)
    {
        Debug.Log("Opening Panels");
        _animator.SetBool("openSolarPanels", curSunState == Sun.SunState.Day);
    }


    public bool IsAlive()
    {
        // TODO (Azee): Implement a life system.
        return true;
    }

    public void UpdatePlayerInputState(bool enable)
    {
        curiosityInputController.enabled = enable;
    }

    private void OnTriggerEnter(Collider other)
    {
        // No use of charge pad anymore
        /*if (other.CompareTag("ChargePad"))
        {
            other.GetComponentInParent<AudioSource>().Play();
        }*/
    }
}