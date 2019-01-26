using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuriosityModel : MonoBehaviour
{
    public Transform CamTarget;
    [ReadOnly] public ThirdPersonPlayerCamera thirdPersonPlayerCamera;

    // Start is called before the first frame update
    void Start()
    {
        AvatarColliderGenerator avatarColliderGenerator = GetComponentInChildren<AvatarColliderGenerator>();
        avatarColliderGenerator.GenerateMeshColliders();
    }

    // Update is called once per frame
    void Update()
    {
    }
}