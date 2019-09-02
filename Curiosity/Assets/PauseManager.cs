using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject PauseModule;
    private bool isPaused = false;

    private void Update()
    {
        if(Input.GetButtonUp("Pause"))
        {
            if (!isPaused)
            {
                Debug.Log("Pausing");
                isPaused = true;
                Pause();
            }
            else
            {
                Debug.Log("UnPausing");
                isPaused = false;
                UnPause();
            }
            
        }
        
    }


    private void Pause()
    {
        PauseModule.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void UnPause()
    {
        PauseModule.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ExitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ExitToDesktop()
    {
        Application.Quit();
    }
}
