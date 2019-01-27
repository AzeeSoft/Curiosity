using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;

public class Wheel : MonoBehaviour
{
    public bool inverted = false;
    public float radius;
    public float groundHugSpeed = 30;

    public float spinSpeed = 0;

    private GameObject wheelHolder;
    private bool initialized = false;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Wheel");
//        Destroy(GetComponent<MeshCollider>());
        wheelHolder = new GameObject("WheelHolder");

        wheelHolder.transform.SetParent(transform);
        wheelHolder.transform.localPosition = Vector3.zero;

        foreach (MeshFilter meshFilter in transform.GetComponentsInChildren<MeshFilter>())
        {
//                Debug.Log("Child");
            Transform child = meshFilter.transform;
            if (child != wheelHolder.transform)
            {
                child.parent = wheelHolder.transform;
//                    child.transform.localPosition = Vector3.zero;
            }
        }

//        GenerateMeshColliders();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//        Debug.Log("Children " + transform.childCount);

        AlignWithFloor();
        HugTheGround();
        Spin();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * radius));
    }

    public void GenerateMeshColliders()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshCollider meshCollider = meshFilter.GetComponent<MeshCollider>();
            if (!meshCollider)
            {
                meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
            }

            meshCollider.convex = true;
            meshCollider.sharedMesh = meshFilter.sharedMesh;
        }
    }

    public void RotateWheel(float angle)
    {
        if (inverted)
        {
            angle = -angle;
        }

        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.Rotate(transform.up, angle);
    }

    public void UpdateSpinSpeed(float spinSpeed)
    {
        this.spinSpeed = spinSpeed;
    }

    void Spin()
    {
//        wheelHolder.transform.localRotation = Quaternion.Euler(0, 0, 0);
//        wheelHolder.transform.Rotate(wheelHolder.transform.right, spinSpeed * 10);
        Quaternion newRotation = wheelHolder.transform.localRotation;
        newRotation = Quaternion.Euler(newRotation.eulerAngles.x+spinSpeed, newRotation.eulerAngles.y, newRotation.eulerAngles.z);
        /*newRotation.x += spinSpeed;
        if (newRotation.x > 360)
        {
            newRotation.x -= 360;
        }*/

        wheelHolder.transform.localRotation = newRotation;
    }

    void AlignWithFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 targetUp = hit.normal;

//            transform.up = Vector3.Lerp(transform.up, targetUp, Time.fixedDeltaTime * 2);
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
        }

        /*if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            Vector3 targetNormal = hit.normal;

            if (Vector3.Angle(Vector3.up, targetNormal) > 45f)
            {
                transform.position += transform.up * 2f;
            }
        }*/
    }

    void HugTheGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, LayerMask.GetMask("Wheel", "Curiosity")))
        {
            Vector3 newPos = transform.position;
            if (hit.distance > radius)
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y - (hit.distance - radius),
                    Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y - (hit.distance - radius);
            }
            else
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y + (radius - hit.distance),
                    Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y + (radius - hit.distance);
            }

            transform.position = newPos;
        }
    }
}