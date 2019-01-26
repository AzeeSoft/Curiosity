using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public bool inverted = false;
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void RotateWheel(float angle)
    {
        if (inverted)
        {
            angle = -angle;
        }
        
        Quaternion newLocalRotation = transform.localRotation;
        newLocalRotation.y = angle;
        transform.localRotation = newLocalRotation;
    }
}