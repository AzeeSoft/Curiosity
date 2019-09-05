using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterUnlock : MonoBehaviour
{
    public Color startingColor;
    private AudioSource source;
    public Light light;

    private void Start()
    {
        this.GetComponentInChildren<MeshRenderer>().sharedMaterial.SetColor("_EmissionColor", startingColor * 4);
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(other.GetComponentInParent<ThrusterController>().enabled == false)
            {
                other.GetComponentInParent<ThrusterController>().enabled = true;

                this.GetComponentInChildren<MeshRenderer>().sharedMaterial.SetColor("_EmissionColor", Color.black);
                source.Play();
                light.enabled = false;
            }
           
        }
    }
}
