using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFadeOnEnable : MonoBehaviour
{

    private void OnEnable()
    {
        GetComponentInChildren<Animator>().SetTrigger("Fade");
    }
}
