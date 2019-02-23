using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Scannable : MonoBehaviour
{
    public Material scannableMaterial;

    private Dictionary<Renderer, List<Material>> originalMaterials = new Dictionary<Renderer, List<Material>>();
    private SonarScanner _sonarScanner;

    [SerializeField] [ReadOnly] private bool scannableMode = false;


    void Awake()
    {
        _sonarScanner = FindObjectOfType<SonarScanner>();
    }

    // Start is called before the first frame update
    void Start()
    {
        UpdateOriginalMaterials();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, _sonarScanner.transform.position) <=
            _sonarScanner.getCurrentScanRadius())
        {
            if (!scannableMode)
            {
                SwitchToScannableMaterial();
            }
        }
        else
        {
            if (scannableMode)
            {
                SwitchToOriginalMaterial();
            }
        }
    }

    public void UpdateOriginalMaterials()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            originalMaterials[renderer] = new List<Material>();
            foreach (Material material in renderer.sharedMaterials)
            {
                originalMaterials[renderer].Add(material);
            }
        }
    }

    void SwitchToScannableMaterial()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < newMaterials.Length; i++)
            {
                newMaterials[i] = scannableMaterial;
            }

            renderer.sharedMaterials = newMaterials;
        }

        scannableMode = true;
    }

    void SwitchToOriginalMaterial()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            Material[] newMaterials = new Material[renderer.sharedMaterials.Length];

            for (int i = 0; i < originalMaterials[renderer].Count; i++)
            {
                newMaterials[i] = originalMaterials[renderer][i];
            }

            renderer.sharedMaterials = newMaterials;
        }

        scannableMode = false;
    }
}