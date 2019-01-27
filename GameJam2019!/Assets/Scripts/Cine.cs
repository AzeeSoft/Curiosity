using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class Cine : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(MoveOn());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    IEnumerator MoveOn()
    {
        yield return new WaitForSecondsRealtime(33f);

        SceneManager.LoadScene("MainScene");
    }
}
