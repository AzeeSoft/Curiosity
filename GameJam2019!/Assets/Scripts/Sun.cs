using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public float dayCycleSpeed;
    public GameObject sun;
    public GameObject moon;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        sun.transform.RotateAround(Vector3.zero, Vector3.right, dayCycleSpeed * Time.deltaTime);
        sun.transform.LookAt(Vector3.zero);

        moon.transform.RotateAround(Vector3.zero, Vector3.right, dayCycleSpeed * Time.deltaTime);
        moon.transform.LookAt(Vector3.zero);
    }
}
