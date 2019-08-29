using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chasm : MonoBehaviour
{
    public Transform spawn;

    public float heightdif;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            LevelManager.Instance.CuriosityModel.curiosityMovementController.enabled = false;

            LevelManager.Instance.CuriosityModel.transform.position = new Vector3(spawn.position.x, LevelManager.Instance.CuriosityModel.transform.position.y + heightdif, spawn.position.z);

            LevelManager.Instance.CuriosityModel.curiosityMovementController.enabled = true;
        }
    }
}
