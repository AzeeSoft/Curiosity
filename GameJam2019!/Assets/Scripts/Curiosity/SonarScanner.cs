using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SonarScanner : MonoBehaviour
{
    public float maxScanRadius = 100f;
    public float scanRadiusGrowthRate = 10f;
    public float scanStayTime = 5f;
    public float scanRadiusShrinkRate = 10f;

    [SerializeField] private float _curScanRadius = 0;

    private CuriosityModel _curiosityModel;
    private CuriosityInputController _curiosityInputController;
    private SonarEffect _sonarEffect;
    
    private bool _isScanning = false;


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

        if (input.Scan && !_isScanning)
        {
            ScanEnvironment();
        }
    }

    async void ScanEnvironment()
    {
        Debug.Log("Scanning environment");

        _isScanning = true;

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

        _isScanning = false;
    }

    public float getCurrentScanRadius()
    {
        return _curScanRadius;
    }
}