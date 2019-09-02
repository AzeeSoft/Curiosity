using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameTrigger : MonoBehaviour
{
    public string SceneToLoad = "Credits";

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(SceneToLoad);
        }
    }
}
