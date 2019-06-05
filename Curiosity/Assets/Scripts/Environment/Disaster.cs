using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Disaster : MonoBehaviour
{
    public float DisasterRadius = 50f;
    public float DisasterCameraLockDuration = 3f;
    public float killTime = 5f;
    private float currentKillTime = 0;
    public bool DisasterOccured = false;

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            currentKillTime = 0;
        }
    }

}