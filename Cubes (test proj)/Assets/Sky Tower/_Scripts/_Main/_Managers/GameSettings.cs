using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{

    public static event Action<bool> OnSettingsWindowOpen;

    [Range(0f, 1f)] [SerializeField] private float defMasterVolume = 1f, defMusicVolume = 1f;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSldier;
    [SerializeField] private Slider musicSldier;
    [SerializeField] private Toggle shakerToggle;

    
    private float masterVolume;
    private float musicVolume;

    private bool isOpen;
    private Animator settingsAnimator;
    private static readonly int AnimationOpen = Animator.StringToHash("OpenSettings");
    private static readonly int AnimationClose = Animator.StringToHash("CloseSettings");


    private void OnDisable()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("IsShakerOn", CameraShaker.isShakerOn ? 1 : 0);
    }

    private void Awake()
    {
        settingsAnimator = gameObject.GetComponent<Animator>();
        
        if (PlayerPrefs.GetInt("FirstStart") == 0)
        {
            PlayerPrefs.SetInt("FirstStart", 1);
            SetUpGameDefaultSettings();
        }

        SetUpApplicationSettings();
    }

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


    private void SetUpApplicationSettings()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(720,1280,true);
        //Screen.SetResolution(450,800,true);
        //Helper.MainCamera.aspect = 9f / 16f;
    }
    
    private void SetUpGameDefaultSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", defMasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", defMusicVolume);
        PlayerPrefs.SetInt("IsShakerOn", 1);
        PlayerPrefs.SetString("sound", "on");
        PlayerPrefs.SetString("music", "on");
    }
    
    private float GetEaseOutQuint(float value)
    {
        return 1 - Mathf.Pow(1 - value, 4);
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
        isOpen = !isOpen;
        if (!isOpen)
        {
            CloseSettings();
            return;
        }
        
        SoundManager.Instance.PlaySound("ButtonClick");
        settingsAnimator.SetTrigger(AnimationOpen);
        
        OnSettingsWindowOpen?.Invoke(true);
    }
    
    public void CloseSettings()
    {
        isOpen = false;
        
        SoundManager.Instance.PlaySound("ButtonClick");
        settingsAnimator.SetTrigger(AnimationClose);
        
        OnSettingsWindowOpen?.Invoke(false);
    }

    
    
    
}
