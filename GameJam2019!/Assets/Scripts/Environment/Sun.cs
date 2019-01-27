using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public enum SunState
    {
        None,
        Day,
        Night,
    }
    
    public float dayCycleSpeed;
    public GameObject sun;
    public GameObject moon;

    public Color sunSetColor;
    public float sunSetIntensity;
    public float sunSetLerpRate;

    public event SunStateChangeCallback OnSunStateChanged;
    public event SunStateDetectionCallback OnDayStateDetected;
    public event SunStateDetectionCallback OnNightStateDetected;

    [SerializeField] [ReadOnly] private int _hourOfDay;
    [SerializeField] [ReadOnly] private SunState _sunState = SunState.None;
    private Light _sunLight;
    private Light _moonLight;

    private Color originalSunColor;
    private float originalSunIntensity;

    void Start()
    {
        _sunLight = sun.GetComponent<Light>();
        _moonLight = moon.GetComponent<Light>();
        
        originalSunColor = _sunLight.color;
        originalSunIntensity = _sunLight.intensity;
        
        UpdateSunState();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sun.transform.RotateAround(Vector3.zero, Vector3.right, dayCycleSpeed * Time.deltaTime);
//        sun.transform.LookAt(Vector3.zero);

        moon.transform.RotateAround(Vector3.zero, Vector3.right, dayCycleSpeed * Time.deltaTime);
//        moon.transform.LookAt(Vector3.zero);

        UpdateSunState();
    }

    int GetHourOfTheDay()
    {
        float sunAngle = Vector3.SignedAngle(sun.transform.forward, Vector3.forward, Vector3.right);
//        Debug.Log("Angle: " + sunAngle);
        if (sunAngle < 0)
        {
            sunAngle = 360 + sunAngle;
        }

        sunAngle = 360 - sunAngle;

        return (int) HelperUtilities.Remap(sunAngle, 0, 360, 0, 24);
    }

    void UpdateSunState()
    {
        SunState prevState = _sunState;
        
        _hourOfDay = GetHourOfTheDay();

        if (_hourOfDay >= 2 && _hourOfDay < 12)
        {
            _sunState = SunState.Day;
        }
        else
        {
            _sunState = SunState.Night;
        }

        if (_sunState != prevState)
        {
            OnSunStateChanged?.Invoke(_sunState);
            OnDayStateDetected?.Invoke();
            OnNightStateDetected?.Invoke();
        }

        if (_hourOfDay >= 7 && _hourOfDay < 12)
        {
            // Sunset
            _sunLight.color = Color.Lerp(_sunLight.color, sunSetColor, Time.deltaTime * sunSetLerpRate);
            _sunLight.intensity = Mathf.Lerp(_sunLight.intensity, sunSetIntensity, Time.deltaTime * sunSetLerpRate);
        } else if (_hourOfDay >= 1 && _hourOfDay < 6)
        {
            // Sunrise
            _sunLight.color = Color.Lerp(_sunLight.color, originalSunColor, Time.deltaTime * sunSetLerpRate);
            _sunLight.intensity = Mathf.Lerp(_sunLight.intensity, originalSunIntensity, Time.deltaTime * sunSetLerpRate);
        }
    }
    
    public delegate void SunStateChangeCallback(SunState newSunState);
    public delegate void SunStateDetectionCallback();
}
