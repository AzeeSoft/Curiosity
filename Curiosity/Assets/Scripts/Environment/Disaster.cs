using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Playables;

public class Disaster : MonoBehaviour
{
    public float DisasterRadius = 50f;
    public float DisasterCameraLockDuration = 3f;
    public float killTime = 5f;
    private float currentKillTime = 0;
    public bool DisasterOccured = false;

    public Transform spawn;

    public PlayableDirector fadePlay;

    public float heightdif;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (!DisasterOccured)
        {
            CheckAndTriggerDisaster();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, DisasterRadius);
    }

    void CheckAndTriggerDisaster()
    {
        float distFromPlayer = Vector3.Distance(LevelManager.Instance.CuriosityModel.Body.transform.position,
            transform.position);

        if (distFromPlayer <= DisasterRadius)
        {
            StartCoroutine(TriggerDisaster());
        }
    }

    IEnumerator TriggerDisaster()
    {
        CinemachineCameraManager.Instance.SwitchCameraState(
            CinemachineCameraManager.CinemachineCameraState.EventLock, new EventLockCamera.StateData()
            {
                LookAtTarget = transform,
                ZoomInOut = true,
                EventLockDuration = DisasterCameraLockDuration
            });

        DisasterOccured = true;

        yield return new WaitForSeconds(DisasterCameraLockDuration);

        CinemachineCameraManager.Instance.SwitchToPreviousCameraState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentKillTime = 0;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentKillTime += Time.deltaTime;
        }
        if(currentKillTime >= killTime)
        {
            StartCoroutine("Respawn");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentKillTime = 0;
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

}