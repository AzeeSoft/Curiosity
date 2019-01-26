using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject main;
    public GameObject options;
    public Slider mySlider;

    void Start()
    {
        main = GameObject.Find("MainMenu");
        options = GameObject.Find("OptionsMenu");
        options.SetActive(false);
    }

    public void PlayGame()
    {
        //Add scene to build and place name here when available
        //SceneManager.LoadScene();
    }

    public void QuitGame()
    {
        Debug.Log("Quit button clicked.");
        Application.Quit();
    }

    public void ShowOptions()
    {
        //De-activate ManMenu
        main.SetActive(false);
        Debug.Log("Main menu de-activated");

        //Activate OptionsMenu
        options.SetActive(true);
    }

    public void ShowMain()
    {
        options.SetActive(false);
        main.SetActive(true);
    }

}
