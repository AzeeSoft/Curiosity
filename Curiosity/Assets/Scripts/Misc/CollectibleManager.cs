using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public List<GameObject> researchableGroups;

    public int totalCollectibleCount => _researchScannables.Count;

    public int collectiblesCollected { get; private set; } = 0;

    public event Action<ResearchScannable> onNewCollectibleCollected;

    public static CollectibleManager Instance { get; private set; }
    
    private Dictionary<ResearchScannable, bool> _researchScannables = new Dictionary<ResearchScannable, bool>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        FindCollectibles();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FindCollectibles()
    {
        _researchScannables.Clear();

        researchableGroups.ForEach(group =>
        {
            foreach (var researchScannable in group.GetComponentsInChildren<ResearchScannable>())
            {
                if (researchScannable.collectible)
                {
                    _researchScannables[researchScannable] = false;
                    researchScannable.onResearched += OnCollectibleResearched;
                }
            }
        });

        collectiblesCollected = 0;
    }

    void OnCollectibleResearched(ResearchScannable researchScannable)
    {
        bool wasCollected = _researchScannables[researchScannable];
        if (!wasCollected)
        {
            _researchScannables[researchScannable] = true;
            collectiblesCollected++;
//            collectiblesCollected = _researchScannables.Count(pair => pair.Value);

            onNewCollectibleCollected?.Invoke(researchScannable);
        }
    }
}
