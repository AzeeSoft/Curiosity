using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{

    public Text percent;

    private void Start()
    {
        StartCoroutine("LoadAsyncOperation");
    }
    public IEnumerator LoadAsyncOperation()
    {
        AsyncOperation gameLevel = SceneManager.LoadSceneAsync("MainSceneMars");
        yield return new WaitForEndOfFrame();
    }
}
