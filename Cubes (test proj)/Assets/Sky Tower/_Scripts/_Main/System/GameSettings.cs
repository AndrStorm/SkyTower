using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{

    public static event Action<bool> OnSettingsWindowOpen;

    [Range(0f, 1f)] [SerializeField] private float defaultMasterVolume = 1f, defaultMusicVolume = 1f;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private Slider masterSldier;
    [SerializeField] private Slider musicSldier;
    [SerializeField] private Toggle shakerToggle, postProcessingToggle;

    
    private float masterVolume, musicVolume;
    private bool isShakerOn, isPostProcessingOn;

    private bool isOpen;
    private Animator settingsAnimator;
    private static readonly int AnimationOpen = Animator.StringToHash("OpenSettings");
    private static readonly int AnimationClose = Animator.StringToHash("CloseSettings");


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

    private void OnEnable()
    {
        LeaderboardUI.OnLeaderboardOpen += OnOtherWindowOpened;
    }

    private void OnDisable()
    {
        LeaderboardUI.OnLeaderboardOpen -= OnOtherWindowOpened;
        
        
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetInt("IsShakerOn", isShakerOn ? 1 : 0);
        PlayerPrefs.SetInt("IsPostProcessing", isPostProcessingOn ? 1 : 0);
    }
    

    private void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        musicVolume = PlayerPrefs.GetFloat("MusicVolume");
        isShakerOn = PlayerPrefs.GetInt("IsShakerOn") != 0 ? true : false;
        isPostProcessingOn = PlayerPrefs.GetInt("IsPostProcessing") != 0 ? true : false;
        
        
        masterSldier.value = masterVolume;
        musicSldier.value = musicVolume;
        shakerToggle.isOn = isShakerOn;
        postProcessingToggle.isOn = isPostProcessingOn;
        
        
        SetMasterVolume(masterVolume);
        SetMusicVolume(musicVolume);
        SetCameraShaker(isShakerOn);
        SetPostProcessing(isPostProcessingOn);
    }


    private void SetUpApplicationSettings()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(720,1280,true);
        // Screen.orientation = ScreenOrientation.Portrait;
        // Screen.autorotateToPortrait = Screen.autorotateToPortraitUpsideDown = true;
        //Screen.SetResolution(450,800,true);
        //Helper.MainCamera.aspect = 9f / 16f;
    }
    
    private void SetUpGameDefaultSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", defaultMasterVolume);
        PlayerPrefs.SetFloat("MusicVolume", defaultMusicVolume);
        PlayerPrefs.SetInt("IsShakerOn", 1);
        PlayerPrefs.SetInt("IsPostProcessing", 1);
        
        
        PlayerPrefs.SetInt("sound", 1);
        PlayerPrefs.SetInt("music", 1);
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
        SetCameraShaker(isOn);
        SoundManager.Instance.PlaySound("ButtonClick");
    }
    
    public void TogglePostProcessing(bool isOn)
    {
        SetPostProcessing(isOn);
        SoundManager.Instance.PlaySound("ButtonClick");
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
    
    
    
    
    private void SetCameraShaker(bool isOn)
    {
        isShakerOn = isOn;
        CameraShaker.isShakerOn = isOn;
    }
    
    private void SetPostProcessing(bool isOn)
    {
        isPostProcessingOn = isOn;
        Helper.MainCamera.GetUniversalAdditionalCameraData().renderPostProcessing = isOn;
    }
    
    private void OnOtherWindowOpened(bool isWindowOpen)
    {
        if (!isOpen) return;
        if (isWindowOpen) CloseSettings();
    }

    
    

    
    
    
}
