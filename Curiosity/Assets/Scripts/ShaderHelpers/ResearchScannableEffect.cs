using System.Collections;
using BasicTools.ButtonInspector;
using UnityEngine;

public class ResearchScannableEffect : MonoBehaviour
{
    public Transform top;
    public Transform bottom;

    public float scanAnimationDuration = 3f;
    
    [Button("Play Scan Effect", "PlayScanEffect")]
    public bool btn_PlayScanEffect;

    private float curScanPos = 0f;
    private IEnumerator scanEffectCoroutine;

    private void Update()
    {
        UpdateShaderProps();
    }

    private void UpdateShaderProps()
    {
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            foreach (Material material in renderer.materials)
            {
                material.SetVector("_Top", top.localPosition);
                material.SetVector("_Bottom", bottom.localPosition);
                material.SetFloat("_ScanlinePos", curScanPos);
            }
        }
    }

    private void PlayScanEffect()
    {
        if (scanEffectCoroutine != null)
        {
            StopCoroutine(scanEffectCoroutine);
        }

        scanEffectCoroutine = AnimateScanEffect();
        StartCoroutine(scanEffectCoroutine);
    }

    private IEnumerator AnimateScanEffect()
    {
        curScanPos = 0f;

        while (curScanPos < 1f)
        {
            curScanPos += (1 / scanAnimationDuration) * Time.deltaTime;
            if (curScanPos > 1f)
            {
                curScanPos = 1f;
            }
            yield return new WaitForEndOfFrame();
        }

        scanEffectCoroutine = null;
    }
}