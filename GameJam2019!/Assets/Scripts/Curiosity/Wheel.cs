using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements.StyleEnums;

public class Wheel : MonoBehaviour
{
    public bool inverted = false;
    public float radius;
    public float groundHugSpeed = 30;

    void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Wheel");
        Destroy(GetComponent<MeshCollider>());
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AlignWithFloor();
        HugTheGround();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position - (transform.up * radius));
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
    
    void AlignWithFloor()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 targetUp = hit.normal;
            
//            transform.up = Vector3.Lerp(transform.up, targetUp, Time.fixedDeltaTime * 2);
            transform.rotation = Quaternion.LookRotation(transform.forward, hit.normal);
        }
    }

    void HugTheGround()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, LayerMask.NameToLayer("Wheel")))
        {
            Vector3 newPos = transform.position;
            if (hit.distance > radius)
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y - (hit.distance - radius), Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y - (hit.distance - radius);
            }
            else
            {
                newPos.y = Mathf.Lerp(newPos.y, newPos.y + (radius - hit.distance), Time.fixedDeltaTime * groundHugSpeed);
//                newPos.y = newPos.y + (radius - hit.distance);
            }
            transform.position = newPos;
        }
    }
}