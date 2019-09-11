using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleCounterUI : MonoBehaviour
{
    public TextMeshProUGUI collectedText;
    public TextMeshProUGUI totalText;

    // Start is called before the first frame update
    void Start()
    {
        CollectibleManager.Instance.onNewCollectibleCollected += OnNewCollectibleCollected;
        RefreshCollectibleCounts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewCollectibleCollected(ResearchScannable researchScannable)
    {
        RefreshCollectibleCounts();
    }

    void RefreshCollectibleCounts()
    {
        collectedText.text = CollectibleManager.Instance.collectiblesCollected.ToString();
        totalText.text = CollectibleManager.Instance.totalCollectibleCount.ToString();
    }
}
