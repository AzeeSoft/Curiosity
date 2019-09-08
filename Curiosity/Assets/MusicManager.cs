using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] clips;

    private int currentClip = 1;

    public AudioSource source;

    private Randomizer<AudioClip> audioClipRandomizer;

    void Awake()
    {
        audioClipRandomizer = new Randomizer<AudioClip>(clips);
    }

    private void Update()
    {
        if(source == null)
        {
            return;
        }
        
        if (source.time >= source.clip.length)
        {
            Debug.Log("Switching");
            source.clip = audioClipRandomizer.GetRandomItem();
            source.Play();
        }
    }
}
