using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerminalTrigger : MonoBehaviour
{
   public enum State
    {
        First,
        Second,
        Last
    }

    public bool playingAnim;

    public State currentAnim;

    public GameObject poleLight;

    public GameObject spawnPos;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (currentAnim)
            {
                case State.First:
                    break;
                case State.Second:
                    break;
                case State.Last:
                    break;
            }
        }
    }
    
}
