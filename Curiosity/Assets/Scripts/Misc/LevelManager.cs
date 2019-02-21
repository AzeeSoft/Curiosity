using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    public GameObject HUD;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    public GameObject FadeOut;

    public string MainMenuSceneName = "MainMenu";
    public string EndGameCreditsSceneName = "EndCredits";

    public event Action<bool> OnGameOver;
    public event Action OnAllTerminalsExplored;
    public bool GameOver { get; private set; }

    [HideInInspector] public CuriosityModel curiosityModel;
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
        Time.timeScale = 1;

        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }

        curiosityModel = FindObjectOfType<CuriosityModel>();
        _sun = GetComponent<Sun>();
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
            if (curiosityModel.Battery <= 0)
            {
                StartCoroutine(GameLost());
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

    IEnumerator GameLost()
    {
        GameOver = true;

        Debug.Log("You Lost");
        OnGameOver?.Invoke(false);

        Time.timeScale = 0;
        FadeOut.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        HelperUtilities.UpdateCursorLock(false);
        LoseScreen.SetActive(true);
        FadeOut.SetActive(false);
    }

    public void FinalBaseReached()
    {
        StartCoroutine(GameWon());
    }

    IEnumerator GameWon()
    {
        GameOver = true;

        Debug.Log("You Won");
        OnGameOver?.Invoke(true);

        Time.timeScale = 0;
        FadeOut.SetActive(true);

        yield return new WaitForSecondsRealtime(1);

        SceneManager.LoadScene(EndGameCreditsSceneName);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(MainMenuSceneName);
    }
}