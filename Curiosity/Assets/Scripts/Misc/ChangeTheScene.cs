using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeTheScene : MonoBehaviour
{

    public float timeTillChange = 10f;

    private void Start()
    {
        StartCoroutine("LoadScene");
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(timeTillChange);
        SceneManager.LoadScene("MainMenu");
    }
}
