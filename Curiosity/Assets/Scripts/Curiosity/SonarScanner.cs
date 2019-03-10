using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SonarScanner : MonoBehaviour
{
    public bool showGizmos = false;
    
    public float maxScanRadius = 100f;
    public float scanRadiusGrowthRate = 10f;
    public float scanStayTime = 5f;
    public float scanRadiusShrinkRate = 10f;
    public float cooldownInterval = 10f;

    public float CooldownLevel = 0;

    [SerializeField] private float _curScanRadius = 0;

    private CuriosityModel _curiosityModel;
    private CuriosityInputController _curiosityInputController;
    private SonarEffect _sonarEffect;

    private bool _canScan = true;


    private void Awake()
    {
        _curiosityModel = GetComponent<CuriosityModel>();
        _curiosityInputController = GetComponent<CuriosityInputController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _sonarEffect = _curiosityModel.thirdPersonPlayerCamera.GetComponentInChildren<SonarEffect>();
        _sonarEffect.sonarScanner = this;
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos)
        {
            return;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, maxScanRadius);

        if (_curScanRadius > 0)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _curScanRadius);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CuriosityInputController.CuriosityInput input = _curiosityInputController.GetPlayerInput();

        if (input.Scan && _canScan && CooldownLevel <= 0)
        {
            if (_curiosityModel.IsAlive())
            {
                ScanEnvironment();
            }
        }
        else if (CooldownLevel > 0)
        {
            CooldownLevel -= cooldownInterval * Time.deltaTime;
            if (CooldownLevel < 0)
            {
                CooldownLevel = 0;
            }
        }
    }

    async void ScanEnvironment()
    {
        Debug.Log("Scanning environment");

        _canScan = false;

        int updateFrequency = 60;

        while (_curScanRadius < maxScanRadius)
        {
            _curScanRadius += scanRadiusGrowthRate / updateFrequency;
            await Task.Delay(1000 / updateFrequency);
        }

        _curScanRadius = maxScanRadius;
        await Task.Delay(TimeSpan.FromSeconds(scanStayTime));

        while (_curScanRadius > 0)
        {
            _curScanRadius -= scanRadiusShrinkRate / updateFrequency;
            await Task.Delay(1000 / updateFrequency);
        }

        _curScanRadius = 0;

        CooldownLevel = 100;
        _canScan = true;
    }

    public float getCurrentScanRadius()
    {
        return _curScanRadius;
    }
}