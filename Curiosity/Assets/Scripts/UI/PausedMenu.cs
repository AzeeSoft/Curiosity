using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    private GameObject pauseMenu;
    private string sceneName = "MainScene_Chris";

    void Start()
    {
        pauseMenu = GameObject.Find("PauseMenu");
        pauseMenu.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;

        Time.timeScale = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ShowPause();
        }
    }

    public void ShowPause()
    {
        Cursor.lockState = CursorLockMode.None;

        pauseMenu.SetActive(true);

        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            ResumeGame();
        }
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;

        pauseMenu.SetActive(false);

        Time.timeScale = 1;
    }

    public void ExitGame()
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }
}
