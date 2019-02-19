using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    const float MaxBattery = 100f;

    public float Battery = 100f;

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
        if (Battery > 0)
        {
            CuriosityInputController.CuriosityInput input = _curiosityInputController.GetPlayerInput();
            if (input.Respawn)
            {
                Respawn();
            }

            if (_sun.GetSunState() == Sun.SunState.Day)
            {
                RechargeBattery(SolarChargeRate * Time.deltaTime);
            }
            else
            {
                DepleteBattery(BatteryDepletionRate * Time.deltaTime);
            }
        }
    }

    void DepleteBattery(float value)
    {
        Battery -= value;
        if (Battery < 0)
        {
            Battery = 0;
        }
    }

    void RechargeBattery(float value)
    {
        Battery += value;
        if (Battery > MaxBattery)
        {
            Battery = MaxBattery;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChargePad"))
        {
            RechargeBattery(ChargePadRechargeAmount);
            other.GetComponentInParent<AudioSource>().Play();
        }
    }
}