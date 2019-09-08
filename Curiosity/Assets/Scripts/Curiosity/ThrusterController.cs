using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour
{
    public float MaxThrusterForce;
    public float ThrusterDepletionRate = 100f;
    public float ThrusterCooldownTime = 3f;
    public bool IsThrusterActive { get; private set; } = false;

    private float _thrusterCharge = 100f;
    private float _lastUsedTime = 0;

    private Rigidbody _rigidbody;
    private CuriosityInputController _inputController;
    private CuriosityMovementController _curiosityMovementController;
    private Thruster[] _thrusters;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputController = GetComponent<CuriosityInputController>();
        _curiosityMovementController = GetComponent<CuriosityMovementController>();
        _thrusters = GetComponentsInChildren<Thruster>(true);
    }

    void FixedUpdate()
    {
        CuriosityInputController.CuriosityInput input = _inputController.GetPlayerInput();

        if (input.Jump && _thrusterCharge > 0)
        {
            UseThrusters();
            IsThrusterActive = true;
        }
        else
        {
            if (_thrusterCharge < 100 && (_curiosityMovementController.AreWheelsOnGround() ||
                                          Time.time - _lastUsedTime > ThrusterCooldownTime))
            {
                RechargeThruster();
            }

            IsThrusterActive = false;
        }
        
        UpdateThrusterGraphics();
        _thrusterCharge = Mathf.Clamp(_thrusterCharge, 0, 100);
    }

    void UseThrusters()
    {
//        Debug.Log("Using Thrusters");

        _rigidbody.AddForce(Vector3.up * GetThrusterForceToUse());

        _thrusterCharge -= ThrusterDepletionRate * Time.deltaTime;
        _lastUsedTime = Time.time;

        StartCoroutine(StopTrailsTemp());
    }

    IEnumerator StopTrailsTemp()
    {
        _curiosityMovementController.DrawTrails = false;
        yield return new WaitForSeconds(0.3f);
        _curiosityMovementController.DrawTrails = true;
    }

    float GetThrusterForceToUse()
    {
        //NOTE: Maybe use a quadratic function or something to return the force.
        
        /*RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
                
        }*/
        
        return MaxThrusterForce;
    }

    void RechargeThruster()
    {
        if (_thrusterCharge < 100f)
        {
            _thrusterCharge = 100f;
        }
    }

    void UpdateThrusterGraphics()
    {
        foreach (Thruster thruster in _thrusters)
        {
            thruster.gameObject.SetActive(IsThrusterActive);
        }
    }
}