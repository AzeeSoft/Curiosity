using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    private RectTransform myRectTransform;
    // Start is called before the first frame update
    void Start()
    {
       myRectTransform = GetComponent<RectTransform>();
        
    }

    // Update is called once per frame
    void Update()
    {
        myRectTransform.localPosition += Vector3.up;
    }
}
