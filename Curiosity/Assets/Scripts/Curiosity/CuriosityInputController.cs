using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityInputController : MonoBehaviour
{
    public class CuriosityInputSettings
    {
        public float lookXSensitivity = 3.0f;
        public float lookYSensitivity = 3.0f;
        public bool invertLookY = false;
    }

    [Serializable]
    public struct CuriosityInput
    {
        public float Forward;
        public float Turn;
        public bool Boost;
        public bool Jump;
        public bool Scan;
        public bool Respawn;
    }

    [SerializeField] private CuriosityInputSettings _curiosityInputSettings = new CuriosityInputSettings();
    [ReadOnly] [SerializeField] private CuriosityInput _curiosityInput = new CuriosityInput();

    void Update()
    {
        UpdateMoveInput();
    }

    public CuriosityInput GetPlayerInput()
    {
        return _curiosityInput;
    }

    private void OnDisable()
    {
        _curiosityInput = new CuriosityInput();
    }

    void UpdateMoveInput()
    {
        _curiosityInput.Turn = Input.GetAxis("Horizontal");
        _curiosityInput.Forward = Input.GetAxis("Vertical");
        _curiosityInput.Boost = Input.GetButton("Boost");
        _curiosityInput.Jump = Input.GetButton("Jump");
        _curiosityInput.Scan = Input.GetButtonDown("Scan");
        _curiosityInput.Respawn = Input.GetButtonDown("Respawn");
    }
}