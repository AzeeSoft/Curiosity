using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnFirstTerminal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LevelManager.Instance.OnFirstTerminal += () => { Destroy(this.gameObject); };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
