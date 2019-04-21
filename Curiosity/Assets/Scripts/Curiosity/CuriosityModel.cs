using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    public float SolarChargeRate = 5f;
    public float BatteryDepletionRate = 5f;
    public float ChargePadRechargeAmount = 100f;

    [ReadOnly] public ThirdPersonPlayerCamera thirdPersonPlayerCamera;
    [HideInInspector] public Transform CamTarget => Body;

    [Header("Required References")] public GameObject Lens;
    public Transform Body;

    [Header("Required Prefabs")] public GameObject SpotLightPrefab;
    public GameObject CuriosityColliderPrefab;
    public GameObject CuriosityAudioSourcePrefab;

    [HideInInspector] public CuriosityAudio curiosityAudio;

    public CuriosityInputController curiosityInputController { get; private set; }
    public CuriosityMovementController curiosityMovementController { get; private set; }
    public InteractionController interactionController { get; private set; }

    private GameObject _spotLightObject;

    private Animator _animator;
    private Sun _sun;

    void Awake()
    {
        curiosityInputController = GetComponent<CuriosityInputController>();
        curiosityMovementController = GetComponent<CuriosityMovementController>();
        interactionController = GetComponent<InteractionController>();
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

        _animator = GetComponentInChildren<Animator>();

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
    }

    void Respawn()
    {
        Vector3 targetPos = transform.position;
        targetPos.y += 10f;

        transform.position = targetPos;
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