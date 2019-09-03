using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] clips;

    private int currentClip = 1;

    public AudioSource source;
    

    private void Update()
    {
        if(source == null)
        {
            return;
        }
        if(source.clip == null)
        {
            source.clip = clips[1];
            source.Play();
        }
        if (source.time >= source.clip.length)
        {
            Debug.Log("Switching");
            source.clip = clips[currentClip];
            source.Play();
            GetNewTrack();
        }
    }

    private void GetNewTrack()
    {
        Debug.Log("Getting New Track");
        currentClip = Random.Range(0, clips.Length);
    }
}
