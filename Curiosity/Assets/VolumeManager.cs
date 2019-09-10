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
        masterSlide.value = PlayerPrefs.GetFloat("MasterVol", 1);
        masterVol.audioMixer.SetFloat("MasterVol", 20.0f * Mathf.Log10(masterSlide.value));

        MusicSlide.value = PlayerPrefs.GetFloat("MusicVol", 1);
        Music.audioMixer.SetFloat("MusicVol", 20.0f * Mathf.Log10(MusicSlide.value));

        SFXSlide.value = PlayerPrefs.GetFloat("SFXVol", 1);
        SFX.audioMixer.SetFloat("SFXVol", 20.0f * Mathf.Log10(SFXSlide.value));
    }

    public void setMasterVolume(float _value)
    {
        masterVol.audioMixer.SetFloat("MasterVol", 20.0f * Mathf.Log10(_value));
        Debug.Log(_value);
        PlayerPrefs.SetFloat("MasterVol", _value);
        PlayerPrefs.Save(); 
    }
    public void setSFXVolume(float _value)
    {
        SFX.audioMixer.SetFloat("SFXVol", 20.0f * Mathf.Log10(_value));
        PlayerPrefs.SetFloat("SFXVol", _value);
        PlayerPrefs.Save();
        Debug.Log(_value);
    }
    public void setMusicVolume(float _value)
    {
        Music.audioMixer.SetFloat("MusicVol", 20.0f * Mathf.Log10(_value));
        PlayerPrefs.SetFloat("MusicVol", _value);
        PlayerPrefs.Save();
        Debug.Log("saved music volume. " + PlayerPrefs.GetFloat("MusicVol", 0));
        Debug.Log(_value);
    }
}
