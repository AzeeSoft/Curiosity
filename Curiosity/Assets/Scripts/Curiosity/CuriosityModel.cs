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

    private GameObject _spotLightObject;

    private CuriosityInputController _curiosityInputController;
    private Sun _sun;

    void Awake()
    {
        _curiosityInputController = GetComponent<CuriosityInputController>();
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


        _sun = LevelManager.Instance.GetSun();

        _sun.OnSunStateChanged += newState => { UpdateSpotLight(); };
        UpdateSpotLight();
    }

    // Update is called once per frame
    void Update()
    {
        CuriosityInputController.CuriosityInput input = _curiosityInputController.GetPlayerInput();
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

    void UpdateSpotLight()
    {
        Sun.SunState curSunState = _sun.GetSunState();
        _spotLightObject.SetActive(curSunState != Sun.SunState.Day);
    }

    public bool IsAlive()
    {
        // TODO (Azee): Implement a life system.
        return true;
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