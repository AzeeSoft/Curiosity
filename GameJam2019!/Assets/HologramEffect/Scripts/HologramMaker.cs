using System.Collections;
using System.Collections.Generic;
using BasicTools.ButtonInspector;
using UnityEngine;

public class HologramMaker : MonoBehaviour
{
    public Material HologramMaterial;

    public bool RemoveColliders = true;
    public bool RemoveRigidbodies = true;
    public bool RemoveLights = true;

    [Button("Make Hologram", "MakeHologram")]
    public bool InvokeMakeHologram;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void MakeHologram()
    {
        if (HologramMaterial)
        {
            ApplyMaterialRecursively(HologramMaterial);
            if (RemoveColliders)
            {
                RemoveComponentsRecursively<Collider>();
            }

            if (RemoveRigidbodies)
            {
                RemoveComponentsRecursively<Rigidbody>();
            }

            if (RemoveLights)
            {
                RemoveComponentsRecursively<Light>();
            }
        }
        else
        {
            Debug.LogError("No Hologram Material specified");
        }
    }


    private void ApplyMaterialRecursively(Material material)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer renderer in renderers)
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = HologramMaterial;
            }

            renderer.sharedMaterials = newMaterials;
        }
    }

    private void RemoveComponentsRecursively<T>()
    {
        T[] tList = GetComponentsInChildren<T>();

        foreach (T t in tList)
        {
            DestroyImmediate(t as Object);
        }
    }
}