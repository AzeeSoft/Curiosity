using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Audio;

public class MainMenuManager : MonoBehaviour
{
    public string LevelToLoad;

    public GameObject main, settings;

   

    public enum State
    {
        Main,
        Settings
    }

    public State currentState;


   
    public void Update()
    {
        switch (currentState)
        {
            case State.Main:
                settings.SetActive(false);
                main.SetActive(true);
                break;
            case State.Settings:
                settings.SetActive(true);
                main.SetActive(false);
                break;
        }
    }

    public void SwitchState(int state)
    {
        if(state == 0)
        {
            currentState = State.Main;
        }
        else
        {
            currentState = State.Settings;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(LevelToLoad);
    }

    public void ExitGame()
    {
        Application.Quit();
    }


   
}
