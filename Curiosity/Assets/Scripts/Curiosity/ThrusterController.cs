using System.Collections;
using UnityEngine;

public class ThrusterController : MonoBehaviour
{
    public float ThrusterForce;
    public float ThrusterDepletionRate = 100f;
    public float ThrusterCooldownTime = 3f;
    public bool IsThrusterActive { get; private set; } = false;

    private float _thrusterCharge = 100f;
    private float _lastUsedTime = 0;

    private Rigidbody _rigidbody;
    private CuriosityInputController _inputController;
    private CuriosityMovementController _curiosityMovementController;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _inputController = GetComponent<CuriosityInputController>();
        _curiosityMovementController = GetComponent<CuriosityMovementController>();
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

        _thrusterCharge = Mathf.Clamp(_thrusterCharge, 0, 100);
    }

    void UseThrusters()
    {
        Debug.Log("Using Thrusters");

        _rigidbody.AddForce(Vector3.up * ThrusterForce);

        _thrusterCharge -= ThrusterDepletionRate * Time.deltaTime;
        _lastUsedTime = Time.time;
    }

    void RechargeThruster()
    {
        if (_thrusterCharge < 100f)
        {
            _thrusterCharge = 100f;
        }
    }
}