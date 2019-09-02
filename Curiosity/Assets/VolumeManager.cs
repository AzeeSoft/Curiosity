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
        masterSlide.value = currentMasterVol;

        float currentMusicVol;
        Music.audioMixer.GetFloat("MusicVol", out currentMusicVol);
        MusicSlide.value = currentMusicVol;

        float currentSFXVol;
        SFX.audioMixer.GetFloat("SFXVol", out currentSFXVol);
        SFXSlide.value = currentSFXVol;
    }

    public void setMasterVolume(float _value)
    {
        masterVol.audioMixer.SetFloat("MasterVol", _value);
        Debug.Log(_value);
    }
    public void setSFXVolume(float _value)
    {
        SFX.audioMixer.SetFloat("SFXVol", _value);
        Debug.Log(_value);
    }
    public void setMusicVolume(float _value)
    {
        Music.audioMixer.SetFloat("MusicVol", _value);
        Debug.Log(_value);
    }
}
