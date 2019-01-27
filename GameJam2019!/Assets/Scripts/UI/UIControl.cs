using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    private GameObject master;
    private GameObject mainMenu;
    private GameObject credsMenu;
    private GameObject optMenu;
    private string sceneName;


    void Start()
    {
        Time.timeScale = 1;
        
        master = GameObject.Find("UI");
        mainMenu = GameObject.Find("MMenu");
        credsMenu = GameObject.Find("CMenu");
        optMenu = GameObject.Find("OMenu");
        sceneName = "Into Cinematic";

        mainMenu.SetActive(true);
        credsMenu.SetActive(false);
        optMenu.SetActive(false);
    }

    public void ShowMain()
    {
        mainMenu.SetActive(true);
        credsMenu.SetActive(false);
        optMenu.SetActive(false);
    }

    public void ShowCredits()
    {
        mainMenu.SetActive(false);
        credsMenu.SetActive(true);
        optMenu.SetActive(false);
    }

    public void ShowOptions()
    {
        mainMenu.SetActive(false);
        credsMenu.SetActive(false);
        optMenu.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
