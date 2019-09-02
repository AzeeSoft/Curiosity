using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneSkip : MonoBehaviour
{
    public PlayableDirector part1, part2;
    private bool skipping = false;
    private bool stopped = false;

    public Animator cameraSwitcherAnim;

    private void Update()
    {
        if (Input.GetButtonUp("Continue"))
        {
            skipping = true;
        }

        if(skipping)
        {
            SkipCutscene();
        }

    
;
    }

    private void SkipCutscene()
    {
        part1.gameObject.SetActive(false);
        part2.gameObject.SetActive(true);
        part2.time = part2.duration - 1;
        part2.Evaluate();
        part2.Play();
        cameraSwitcherAnim.enabled = false;
    }
}
