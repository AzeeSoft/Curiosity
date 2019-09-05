using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepTrail : MonoBehaviour
{
    public float yOffset = 0.01f;
    public bool AutoAlign = true;
    public bool AutoHideFootstep = true;

    public Material TransparentMaterial;

    private Scannable _scannable;

    void Awake()
    {
        _scannable = GetComponent<Scannable>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AutoAlign)
        {
            AlignWithGround();
        }

        if (AutoHideFootstep)
        {
            HideFootstep();
            _scannable.UpdateOriginalMaterials();
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void AlignWithGround()
    {
        RaycastHit downHit;
        if (Physics.Raycast(transform.position, Vector3.down, out downHit))
        {
            Vector3 newPos = downHit.point;
            newPos.y += yOffset;

            transform.position = newPos;

            transform.rotation = Quaternion.LookRotation(transform.forward, downHit.normal);
        }
    }

    void HideFootstep()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];
            for (int i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                newMaterials[i] = TransparentMaterial;
            }

            renderer.sharedMaterials = newMaterials;
        }
    }
}