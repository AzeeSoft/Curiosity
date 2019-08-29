using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Tutorial : MonoBehaviour
{
    public PlayableAsset intro2;

    public PlayableDirector dir1, dir2;

    private bool intro1done = false;

    private void Awake()
    {
        intro1done = true;
    }

    private void Update()
    {
        
        if(intro1done)
        {
            if (Input.GetButtonUp("Scan"))
            {
                Debug.Log("starting part 2");
                Destroy(dir1);
                dir2.gameObject.SetActive(true);
            }
            
        }
    }
}
