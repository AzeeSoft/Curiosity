using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePopout : MonoBehaviour
{
    public GameObject collectibleCounter;
    public float showDuration = 3f;

    private Coroutine _showAndHideCoroutine = null;

    void Awake()
    {
        collectibleCounter.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        CollectibleManager.Instance.onNewCollectibleCollected += OnNewCollectibleCollected;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnNewCollectibleCollected(ResearchScannable researchScannable)
    {
        if (_showAndHideCoroutine != null)
        {
            StopCoroutine(_showAndHideCoroutine);
        }

        _showAndHideCoroutine = StartCoroutine(ShowAndHide());
    }

    IEnumerator ShowAndHide()
    {
        collectibleCounter.SetActive(true);
        yield return new WaitForSecondsRealtime(showDuration);
        collectibleCounter.SetActive(false);

        _showAndHideCoroutine = null;
    }
}
