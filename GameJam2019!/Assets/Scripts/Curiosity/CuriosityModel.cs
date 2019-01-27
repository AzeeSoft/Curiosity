using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    const float MaxBattery = 100f;

    public float Battery = 100f;

    [ReadOnly] public float SolarChargeRate = 5f;
    public float BatteryDepletionRate = 5f;

    public bool solarChargeMode = false;

    public Transform CamTarget;
    [ReadOnly] public ThirdPersonPlayerCamera thirdPersonPlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        AvatarColliderGenerator avatarColliderGenerator = GetComponentInChildren<AvatarColliderGenerator>();
//        avatarColliderGenerator.GenerateMeshColliders();
    }

    // Update is called once per frame
    void Update()
    {
        if (solarChargeMode)
        {
            RechargeBattery(SolarChargeRate * Time.deltaTime);
        }
        else
        {
            DepleteBattery(BatteryDepletionRate * Time.deltaTime);
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
}