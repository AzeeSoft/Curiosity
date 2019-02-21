using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    public float SolarChargeRate = 5f;
    public float BatteryDepletionRate = 5f;
    public float ChargePadRechargeAmount = 100f;

    public Transform CamTarget;
    [ReadOnly] public ThirdPersonPlayerCamera thirdPersonPlayerCamera;
    public GameObject spotLightObject;

    private CuriosityInputController _curiosityInputController;
    private Sun _sun;

    void Awake()
    {
        _curiosityInputController = GetComponent<CuriosityInputController>();
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
        spotLightObject.SetActive(curSunState != Sun.SunState.Day);
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