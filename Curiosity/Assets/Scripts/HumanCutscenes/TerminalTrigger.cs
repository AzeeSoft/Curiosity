using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TerminalTrigger : MonoBehaviour
{
    public enum State
    {
        First,
        Second,
        Last
    }

    public State currentAnim;

    public bool hasPlayed = false;

    public GameObject scene1Obj, scene2Obj, scene3Obj, initLight;

    private void Start()
    {
        scene1Obj.SetActive(false);
        scene2Obj.SetActive(false);
        scene3Obj.SetActive(false);
    }

    private void Update()
    {
        if (hasPlayed)
        {
            initLight.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("cutscene playing");
            switch (currentAnim)
            {
                case State.First:
                    Debug.Log("cutscene playing");
                    scene1Obj.SetActive(true);
                    //scene1Obj.GetComponent<DirectorControlPlayable>().director.time = 0;
                    //  scene1Obj.GetComponent<DirectorControlPlayable>().director.Play();
                    hasPlayed = true;
                    break;
                case State.Second:
                    scene2Obj.SetActive(true);
                    //  scene2Obj.GetComponent<DirectorControlPlayable>().director.time = 0;
                    // scene2Obj.GetComponent<DirectorControlPlayable>().director.Play();
                    hasPlayed = true;
                    break;
                case State.Last:
                    scene3Obj.SetActive(true);
                    //   scene3Obj.GetComponent<DirectorControlPlayable>().director.time = 0;
                    //   scene3Obj.GetComponent<DirectorControlPlayable>().director.Play();
                    hasPlayed = true;
                    break;
            }

            LevelManager.Instance.OnTerminalExplored(currentAnim);
        }
    }
}