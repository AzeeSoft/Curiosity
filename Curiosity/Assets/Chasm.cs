using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasm : MonoBehaviour
{
    public GameObject spawn;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            LevelManager.Instance.CuriosityModel.gameObject.transform.localPosition = spawn.transform.position;
        }
    }
}
