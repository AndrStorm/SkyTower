using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsButtons : MonoBehaviour
{

    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSldier;
    [SerializeField] private Slider musicSldier;
    [SerializeField] private Toggle shakerToggle;

    private float masterVolume;
    private float musicVolume;
    

    private void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        CameraShaker.isShakerOn = PlayerPrefs.GetInt("IsShakerOn") != 0 ? true : false;
        
        masterSldier.value = masterVolume;
        musicSldier.value = musicVolume;
        shakerToggle.isOn = CameraShaker.isShakerOn;
        
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
    }
    

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("IsShakerOn", CameraShaker.isShakerOn ? 1 : 0);
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        mixer.SetFloat("MasterVolume", Mathf.Lerp(-80f, 0, GetEaseOutQuint(volume)));
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        mixer.SetFloat("MusicVolume", Mathf.Lerp(-80f, 0, GetEaseOutQuint(volume)));
    }

    public void ToggleCameraShaker(bool isOn)
    {
        CameraShaker.isShakerOn = isOn;
    }

    public void OpenSettings()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        transform.GetChild(0).gameObject.SetActive(true);
    }
    public void CloseSettings()
    {
        SoundManager.Instance.PlaySound("ButtonClick");
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private float GetEaseOutQuint(float value)
    {
        return 1 - Mathf.Pow(1 - value, 4);
    }
    
    
}
