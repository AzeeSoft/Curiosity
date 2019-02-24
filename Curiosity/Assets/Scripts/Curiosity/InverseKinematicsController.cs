using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicsController : MonoBehaviour
{
    public GameObject IKHandle, Parent, Mid, Child, ParentHandle;

    private Vector3 midPoint;
    private Vector3 startParent;
    //public float scaleFactor; 

    private float length1, length2, maxLength;

    private void Start()
    {
        startParent = Parent.transform.position;

         length1 = (startParent - Mid.transform.position).magnitude;
         length2 = (Mid.transform.position - IKHandle.transform.position).magnitude;
         maxLength = length1 + length2;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    private void FixedUpdate()
    {
        #region attempt1
        /*
        Parent.transform.LookAt(Mid.transform);
      
        Mid.transform.LookAt(Child.transform);
        ArmJoint.transform.position = Mid.transform.position;

        //Vector3 parentDirection = (Mid.transform.position - Parent.transform.position).normalized;
       // Quaternion toRotation = Quaternion.FromToRotation(Parent.transform.up, parentDirection);
       // Parent.transform.rotation = Quaternion.Lerp(Parent.transform.rotation, toRotation, 2f * Time.time);

        Debug.DrawLine(Parent.transform.position, Child.transform.position, Color.red);

        midPoint = (Parent.transform.position + Child.transform.position) / 2;

        Mid.transform.position = new Vector3 (Mid.transform.position.x, (midPoint.y + 1) / Vector3.Distance(Parent.transform.position, Child.transform.position), Mid.transform.position.z);
        */
        #endregion

        #region attempt2
        
        float distanceToGoal = (startParent - IKHandle.transform.position).magnitude;
        //float midToChild = (Mid.transform.position - Child.transform.position).magnitude;
        //if distance greater than length, plot points along line

        if(true)
        {

            Parent.transform.position = ParentHandle.transform.position;
            //BACKWARDS
            //1 suffix stands for prime || no suffix is original point 
            Vector3 p31 = IKHandle.transform.position;
            // Vector3 p31ToP2 = (p31 - Mid.transform.position).normalized;
            // Vector3 p21 = (p31ToP2 * length2);
            Vector3 p21 = LerpByDistance(p31, Mid.transform.position, length2);
            //Vector3 p21ToP1 = (p21 - Parent.transform.position).normalize;
            // Vector3 p11 = (p21ToP1 * length1);
            Vector3 p11 = LerpByDistance(p21, Parent.transform.position, length1);
            
            //FORWARDS
            //11 suffix stands for prime prime || no suffix is original point
            Vector3 p011 = Parent.transform.position;
            // Vector3 po11ToP11 = (p011 - p11).normalized;
            //  Vector3 p111 = (po11ToP11 * length1);
            Vector3 p111 = LerpByDistance(p11, p21, length1);
            // Vector3 p111ToP21 = (p111 - p21).normalized;
            //  Vector3 p211 = (p111ToP21 * length2);
            Vector3 p211 = LerpByDistance(p21, p111, length2);

            //p111 = LerpByDistance(p111, PoleVector.transform.position, maxLength / 2);

            Mid.transform.position = p111;
            Child.transform.position = IKHandle.transform.position;

            Debug.DrawLine(Parent.transform.position, IKHandle.transform.position, Color.yellow);
            Debug.DrawLine(Parent.transform.position, Mid.transform.position, Color.red);
            Debug.DrawLine(Mid.transform.position, Child.transform.position, Color.red);
        }
        
        #endregion

    }

}
