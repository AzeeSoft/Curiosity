using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Wheel : MonoBehaviour
{
    public class GizmosData
    {
        public Vector3 normalToPlane = Vector3.up;
    }

    public bool inverted = false;
    public float radius;

    public float spinSpeed = 0;
    public bool onGround { get; private set; } = false;

    public GameObject WheelHolder;
    public GameObject WheelParentJoint;

    private bool initialized = false;
    private float _origParentDist;
    private float groundHugSpeed => _curiosityMovementController.GroundHugSpeed;
    private Vector3 _origOffsetFromBody;

    private TrailRenderer _trailRenderer;
    private CuriosityModel _curiosityModel;
    private CuriosityMovementController _curiosityMovementController;
    private ThrusterController _thrusterController;

    private GizmosData _gizmosData = new GizmosData();

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Wheel");
//        Destroy(GetComponent<MeshCollider>());
        /*wheelHolder = new GameObject("WheelHolder");

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
        }*/

//        GenerateMeshColliders();

        _trailRenderer = GetComponentInChildren<TrailRenderer>();
        _curiosityModel = GetComponentInParent<CuriosityModel>();
        _curiosityMovementController = GetComponentInParent<CuriosityMovementController>();
        _thrusterController = GetComponentInParent<ThrusterController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 wheelBottomPos = WheelHolder.transform.position;
        wheelBottomPos.y -= radius;
        _origParentDist = Vector3.Distance(transform.position, WheelParentJoint.transform.position);
        _origOffsetFromBody = _curiosityModel.Body.InverseTransformVector(_curiosityModel.Body.position - transform.position);
    }

    private void Update()
    {
        UpdateTrailRendererState();
    }

    void FixedUpdate()
    {
//        Debug.Log("Children " + transform.childCount);

        if (_curiosityModel.wheelsReposition)
        {
            Reposition();
        }

        AlignWithFloor();
        Spin();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, _gizmosData.normalToPlane * 5);
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

    public void TurnWheel(float angle)
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
        Quaternion newRotation = WheelHolder.transform.localRotation;
        newRotation = Quaternion.Euler(newRotation.eulerAngles.x + spinSpeed, newRotation.eulerAngles.y,
            newRotation.eulerAngles.z);
        /*newRotation.x += spinSpeed;
        if (newRotation.x > 360)
        {
            newRotation.x -= 360;
        }*/

        WheelHolder.transform.localRotation = newRotation;
    }

    void AlignWithFloor()
    {
        if (!onGround)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(transform.forward, Vector3.up), Time.fixedDeltaTime);
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(WheelHolder.transform.position, Vector3.down, out hit))
        {
            Vector3 targetUp = hit.normal;

            _gizmosData.normalToPlane = hit.normal;

//            transform.up = Vector3.Lerp(transform.up, targetUp, Time.fixedDeltaTime * 2);
//            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
            transform.rotation = Quaternion.Lerp(transform.rotation,
                Quaternion.LookRotation(transform.forward, hit.normal), Time.fixedDeltaTime * groundHugSpeed);
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

    /*void Reposition()
    {
        Vector3 wheelBottomPos = WheelHolder.transform.position;
        wheelBottomPos.y -= radius;
        
    }*/

    public void ResetWheel()
    {
        transform.position = _curiosityModel.Body.position - _curiosityModel.Body.TransformVector(_origOffsetFromBody);
    }

    void Reposition()
    {
        RaycastHit upHit;
        RaycastHit downHit;

        float upHitDistance = 0;

//        Vector3 upDir = Vector3.up;
        Vector3 upDir = _curiosityModel.Body.up;

        bool upFound = Physics.Raycast(WheelHolder.transform.position + (upDir * radius * 2), Vector3.down,
            out upHit,
            LayerMask.GetMask("Wheel", "Curiosity"));
        bool downFound = Physics.Raycast(WheelHolder.transform.position, -upDir, out downHit,
            LayerMask.GetMask("Wheel", "Curiosity"));
        bool isHuggingTheGround = true;

        if (upFound)
        {
            if (upHit.point.y > WheelHolder.transform.position.y)
            {
                upHitDistance = Vector3.Distance(upHit.point, WheelHolder.transform.position);
//                Debug.Log("Up: " + upHitDistance);
            }
            else
            {
                upFound = false;
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


        Vector3 newPos = transform.position;
        onGround = false;

        if (upFound)
        {
            newPos.y = Mathf.Lerp(newPos.y, newPos.y + (upHitDistance + radius),
                Time.fixedDeltaTime * groundHugSpeed);
        }
        else if (downFound)
        {
            if (downHit.distance > _curiosityMovementController.GroundHugMaxDistance)
            {
                isHuggingTheGround = false;
            }

            if (downHit.distance > radius)
            {
                float targetY = newPos.y - (downHit.distance - radius);

                /*if (WheelParentJoint.transform.position.y - targetY > _curiosityMovementController.MaxDistanceFromBody)
                {
                    targetY = WheelParentJoint.transform.position.y - _curiosityMovementController.MaxDistanceFromBody;
                }*/

                /*if (_curiosityModel.Body.transform.position.y - targetY >
                    _curiosityMovementController.MaxDistanceFromBody)
                {
                    targetY = _curiosityModel.Body.transform.position.y -
                              _curiosityMovementController.MaxDistanceFromBody;
                }*/

                newPos.y = Mathf.Lerp(newPos.y, targetY,
                    Time.fixedDeltaTime * groundHugSpeed * GetGravityFactor(downHit.distance));
                // newPos.y = newPos.y - (hit.distance - radius);
            }
            else
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y + (radius - downHit.distance),
                    Time.fixedDeltaTime * groundHugSpeed);
                // newPos.y = newPos.y + (radius - hit.distance);
            }
        }
        else
        {
            isHuggingTheGround = false;
        }

        onGround = isHuggingTheGround;

        float oldLocalX = transform.localPosition.x;
        float oldLocalZ = transform.localPosition.z;

        /*Vector3 parentDir = newPos - WheelParentJoint.transform.position;
        if (Mathf.Abs(parentDir.magnitude - _origParentDist) < 1)
        {
        }*/
        transform.position = newPos;

        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.x = oldLocalX;
        newLocalPos.z = oldLocalZ;
        transform.localPosition = newLocalPos;
    }

    float GetGravityFactor(float groundDistance)
    {
        if (groundDistance < radius)
        {
            return 1;
        }

        float gravityFactor = (float) (_curiosityMovementController.Gravity /
                                       Math.Pow(groundDistance, _curiosityMovementController.GravityModifier));
        gravityFactor = Mathf.Clamp01(gravityFactor);

        return gravityFactor;
    }
}