using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject HUD;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    
    public event Action<bool> OnGameOver;
    public event Action OnAllTerminalsExplored;
    public bool GameOver { get; private set; }

    private CuriosityModel _curiosityModel;
    private Sun _sun;

    private Dictionary<TerminalTrigger.State, bool> terminalExplorationStates =
        new Dictionary<TerminalTrigger.State, bool>
        {
            {TerminalTrigger.State.First, false},
            {TerminalTrigger.State.Second, false},
            {TerminalTrigger.State.Last, false},
        };

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

        Time.timeScale = 0;
        
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

    public bool HasExploredAllTerminals()
    {
        return terminalExplorationStates[TerminalTrigger.State.First] &&
               terminalExplorationStates[TerminalTrigger.State.Second] &&
               terminalExplorationStates[TerminalTrigger.State.Last];
    }

    public void OnTerminalExplored(TerminalTrigger.State terminalState)
    {
        terminalExplorationStates[terminalState] = true;

        if (HasExploredAllTerminals())
        {
            OnAllTerminalsExplored?.Invoke();
        }
    }

    void GameLost()
    {
        GameOver = true;

        Debug.Log("You Lost");
        OnGameOver?.Invoke(false);

        Time.timeScale = 0;
        LoseScreen.SetActive(true);
    }

    public void FinalBaseReached()
    {
        GameWon();
    }

    async void GameWon()
    {
        GameOver = true;
        
        Debug.Log("You Won");
        OnGameOver?.Invoke(true);
        
        Time.timeScale = 0;
        WinScreen.SetActive(true);
    }
}