using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIControl : MonoBehaviour
{
    public GameObject master;
    public GameObject mainMenu;
    public GameObject pauseMenu;

    private bool hasInitiated;

    void Start()
    {
        master = GameObject.Find("UI");
        mainMenu = GameObject.Find("MMenu");
        pauseMenu = GameObject.Find("PMenu");

        mainMenu.SetActive(true);
        pauseMenu.SetActive(false);

        hasInitiated = true;
    }

    void Update()
    {
        if (hasInitiated)
        {
            pauseMenu.SetActive(true);
        }
    }
}
