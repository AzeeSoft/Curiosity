using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    private bool isPaused;

    public GameObject menu;
    public GameObject subMenu;

    void Start()
    {
        menu = GameObject.Find("PauseMenu");
        subMenu = GameObject.Find("ConfirmMenu");

        menu.SetActive(false);
        subMenu.SetActive(false);

        isPaused = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Time.timeScale = 1;
                menu.SetActive(true);
                HelperUtilities.UpdateCursorLock(true);
            }
            else
            {
                Time.timeScale = 0;
                menu.SetActive(false);
                if (subMenu.activeSelf)
                    subMenu.SetActive(false);
                HelperUtilities.UpdateCursorLock(false);
            }

            isPaused = !isPaused;

        }
    }

    public void UserConfirm()
    {
        SceneManager.LoadScene("MainScene_Chris");
    }

    public void ShowConfirm()
    {
        menu.SetActive(false);
        subMenu.SetActive(true);
    }

    public void ShowMenu()
    {
        subMenu.SetActive(false);
        menu.SetActive(true);
    }
}
