using System.Collections;
using System.Collections.Generic;
using BasicTools.ButtonInspector;
using UnityEngine;

public class AvatarColliderGenerator : MonoBehaviour
{
//    [Button("Generate Mesh Colliders", "GenerateMeshColliders")]
//    public bool generateMeshColliders;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
