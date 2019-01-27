using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject gameObject;
    private bool isPaused;

    void Start()
    {
        gameObject = GameObject.Find("UI");
    }

    public void PlayGame()
    {
        gameObject = GameObject.Find("MMenu");
        gameObject.SetActive(false);

        gameObject = GameObject.Find("PMenu");
        gameObject.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowOptions()
    {
        gameObject.SetActive(false);

        gameObject = GameObject.Find("OptionsMenu");
        gameObject.SetActive(true);
    }

}
