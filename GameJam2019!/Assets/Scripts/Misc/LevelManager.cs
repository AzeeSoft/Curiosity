using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public event Action<bool> OnGameOver;
    public bool GameOver { get; private set; }

    private CuriosityModel _curiosityModel;
    private Sun _sun;
    
    void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        _curiosityModel = FindObjectOfType<CuriosityModel>();
        _sun = GetComponent<Sun>();
        _sun.OnSunStateChanged += newSunState =>
        {
            _curiosityModel.solarChargeMode = (newSunState == Sun.SunState.Day);
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameOver)
        {
            if (_curiosityModel.Battery <= 0)
            {
                GameLost();
            }
        }
    }

    public Sun GetSun()
    {
        return _sun;
    }

    void GameLost()
    {
        GameOver = true;
        
        Debug.Log("You Lost");
        OnGameOver?.Invoke(false);
    }
}