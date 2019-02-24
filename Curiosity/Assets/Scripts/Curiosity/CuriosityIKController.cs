using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityIKController : MonoBehaviour
{
    public Transform R_IKMid, L_IKMid, R_IKParent, L_IKParent, R_IKChild, L_IKChild;
    public Transform R_BIKJoint, R_FIKJoint, L_BIKJoint, L_FIKJoint;
    public Transform R_FPointJointParent, R_FPointJointChild, R_BPointJointParent, R_BPointJointChild;
    public Transform L_FPointJointParent, L_FPointJointChild, L_BPointJointParent, L_BPointJointChild;

    private void FixedUpdate()
    {
        BackLegPJ();
        FrontLegPJ();
        BackLegIK();
        BackLegIK();
    }

    void FrontLegPJ()
    {
        R_FPointJointParent.LookAt(R_FPointJointChild, Vector3.up);
        L_FPointJointParent.LookAt(L_FPointJointChild, Vector3.up);
    }

    void BackLegPJ()
    {
        R_BPointJointParent.LookAt(R_BPointJointChild, Vector3.up);
        L_BPointJointParent.LookAt(L_BPointJointChild, Vector3.up);
    }

    void BackLegIK()
    {
        R_BIKJoint.LookAt(R_IKParent, Vector3.up);
        R_FIKJoint.LookAt(R_IKChild, Vector3.up);
        R_FIKJoint.transform.position = R_IKMid.transform.position;
        R_BIKJoint.transform.position = R_IKMid.transform.position;
        L_BIKJoint.LookAt(L_IKParent, Vector3.up);
        L_FIKJoint.LookAt(L_IKChild, Vector3.up);
        L_FIKJoint.transform.position = L_IKMid.transform.position;
        L_BIKJoint.transform.position = L_IKMid.transform.position;
        // L_FIKJoint.transform.position = L_IKMid.position;
    }
    
}
