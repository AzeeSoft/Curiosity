using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityInputController : MonoBehaviour
{
    public class CuriosityInputSettings
    {
        
    }

    [Serializable]
    public struct CuriosityInput
    {
        public float Forward;
        public float Turn;
        public bool Boost;
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

    void UpdateMoveInput()
    {
        _curiosityInput.Turn = Input.GetAxis("Horizontal");
        _curiosityInput.Forward = Input.GetAxis("Vertical");
        _curiosityInput.Boost = Input.GetButton("Boost");
        _curiosityInput.Scan = Input.GetButtonDown("Scan");
        _curiosityInput.Respawn = Input.GetButtonDown("Respawn");
    }
}
