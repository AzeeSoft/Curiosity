using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Wheel : MonoBehaviour
{
    public bool inverted = false;
    public float radius;

    public float spinSpeed = 0;
    public bool onGround { get; private set; } = false;

    private GameObject wheelHolder;
    private bool initialized = false;
    private float groundHugSpeed => _curiosityMovementController.GroundHugSpeed;

    private TrailRenderer _trailRenderer;
    private CuriosityMovementController _curiosityMovementController;

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

        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _curiosityMovementController = GetComponentInParent<CuriosityMovementController>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        UpdateTrailRendererState();
    }

    void FixedUpdate()
    {
//        Debug.Log("Children " + transform.childCount);

        HugTheGround();
        AlignWithFloor();
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

    void UpdateTrailRendererState()
    {
        if (_trailRenderer)
        {
            _trailRenderer.emitting = onGround;
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
        newRotation = Quaternion.Euler(newRotation.eulerAngles.x + spinSpeed, newRotation.eulerAngles.y,
            newRotation.eulerAngles.z);
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
        RaycastHit upHit;
        RaycastHit downHit;

        float upHitDistance = 0;

        bool upFound = Physics.Raycast(transform.position + (Vector3.up * radius * 2), Vector3.down, out upHit,
            LayerMask.GetMask("Wheel", "Curiosity"));
        bool downFound = Physics.Raycast(transform.position, Vector3.down, out downHit,
            LayerMask.GetMask("Wheel", "Curiosity"));

        if (upFound)
        {
            if (upHit.point.y > transform.position.y)
            {
                upHitDistance = Vector3.Distance(upHit.point, transform.position);
//                Debug.Log("Up: " + upHitDistance);
            }
            else
            {
                upFound = false;
            }
        }

        if (downFound)
        {
            if (downHit.distance > _curiosityMovementController.GroundHugMaxDistance)
            {
                downFound = false;
            }
        }

        if (upFound && downFound)
        {
            if (downHit.distance <= upHitDistance)
            {
                upFound = false;
            }
            else
            {
                downFound = false;
            }
        }

        onGround = true;
        Vector3 newPos = transform.position;

        if (downFound)
        {
            if (downHit.distance > radius)
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y - (downHit.distance - radius),
                    Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y - (hit.distance - radius);
            }
            else
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y + (radius - downHit.distance),
                    Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y + (radius - hit.distance);
            }
        }
        else if (upFound)
        {
            newPos.y = Mathf.Lerp(newPos.y, newPos.y + (upHitDistance + radius),
                Time.fixedDeltaTime * groundHugSpeed);
        }
        else
        {
            onGround = false;

            // Applying Normal Gravity   
            newPos.y = Mathf.Lerp(newPos.y, newPos.y - _curiosityMovementController.Gravity,
                Time.fixedDeltaTime);
        }

        transform.position = newPos;
    }
}