using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Chasm : MonoBehaviour
{
    public Transform spawn;

    public PlayableDirector fadePlay;
    

    public float heightdif;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine("Respawn");
        }
    }

    public IEnumerator Respawn()
    {
        Time.timeScale = 0;

        fadePlay.Play();

        LevelManager.Instance.CuriosityModel.curiosityMovementController.DrawTrails = false;
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + 2)
        {
            yield return null;
        }

        Time.timeScale = 1;

       // Destroy(LevelManager.Instance.CuriosityModel);

        //var newCuriosity = Instantiate(playerPrefab, spawn.position, Quaternion.identity) as GameObject;

        //LevelManager.Instance.CuriosityModel = newCuriosity.GetComponent<CuriosityModel>();

        //CinemachineCameraManager.Instance.

        LevelManager.Instance.CuriosityModel.curiosityMovementController.enabled = false;
//        LevelManager.Instance.CuriosityModel.Avatar.rotation = Quaternion.Euler(0, spawn.transform.rotation.y, 0);
        LevelManager.Instance.CuriosityModel.Avatar.forward = spawn.forward;

        LevelManager.Instance.CuriosityModel.transform.position = new Vector3(spawn.position.x, LevelManager.Instance.CuriosityModel.transform.position.y + heightdif, spawn.position.z);

        LevelManager.Instance.CuriosityModel.curiosityMovementController.enabled = true;
        LevelManager.Instance.CuriosityModel.curiosityMovementController.DrawTrails = true;

        LevelManager.Instance.CuriosityModel.Respawn(false);


    }


    public static IEnumerator WaitForRealSeconds(float time)
    {
        float start = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < start + time)
        {
            yield return null;
        }
    }
}
