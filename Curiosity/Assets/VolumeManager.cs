using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeManager : MonoBehaviour
{
    public AudioMixerGroup masterVol, Music, SFX;

    public Slider masterSlide, SFXSlide, MusicSlide;

    private void Start()
    {
        float currentMasterVol;
        masterVol.audioMixer.GetFloat("MasterVol", out currentMasterVol);
        masterSlide.value = Mathf.Pow(10.0f, currentMasterVol / 20.0f);

        float currentMusicVol;
        Music.audioMixer.GetFloat("MusicVol", out currentMusicVol);
        MusicSlide.value = Mathf.Pow(10.0f, currentMusicVol / 20.0f);

        float currentSFXVol;
        SFX.audioMixer.GetFloat("SFXVol", out currentSFXVol);
        SFXSlide.value = Mathf.Pow(10.0f, currentSFXVol / 20.0f);
    }

    public void setMasterVolume(float _value)
    {
        masterVol.audioMixer.SetFloat("MasterVol", 20.0f * Mathf.Log10(_value));
        Debug.Log(_value);
    }
    public void setSFXVolume(float _value)
    {
        SFX.audioMixer.SetFloat("SFXVol", 20.0f * Mathf.Log10(_value));
        Debug.Log(_value);
    }
    public void setMusicVolume(float _value)
    {
        Music.audioMixer.SetFloat("MusicVol", 20.0f * Mathf.Log10(_value));
        Debug.Log(_value);
    }
}
