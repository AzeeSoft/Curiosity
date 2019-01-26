using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject subMenu;
    private bool showMenu;

    void Start()
    {
        menu = GameObject.Find("PauseMenu");
        subMenu = GameObject.Find("ConfirmMenu");
        subMenu.SetActive(false);
        showMenu = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!menu.activeSelf)
            {
                menu.SetActive(true);
                Debug.Log("SetActive set true");
            }
            else
            {
                menu.SetActive(false);
                Debug.Log("else statement");
            }
        }
    }

    public void ResumeGame()
    {
        menu.SetActive(false);
    }

    public void UserConfirm()
    {
        SceneManager.LoadScene("MainMenu");
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
