using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{
    
    public AudioClip mainTheme, shopTheme;
    
    
    [SerializeField] private List<Sound> sounds;


    private bool isSceneLoaded;
    private float musicVolume, adjustedMusicVolume;
    private AudioSource musicAudioSource;
    private AudioSource windAudioSource;

    
    private void OnEnable()
    {
       SceneLoader.OnFininshedLoadingScene += OnSceneLoaded;
    }

    private void OnDisable()
    {
      SceneLoader.OnFininshedLoadingScene -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded()
    {
        isSceneLoaded = true;
        windAudioSource.volume = 0f; //-
    }
    
    
    
    protected override void Awake()
    {
        base.Awake();
        musicAudioSource = GetComponent<AudioSource>();

        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.outputAudioMixerGroup = sound.audioMixerGroup;
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        
        adjustedMusicVolume = musicAudioSource.volume;
        musicVolume = adjustedMusicVolume;
    }

    private void Start()
    {
        windAudioSource = GetSoundSource("Wind");
        
        if (PlayerPrefs.GetInt("sound") == 0)
        {
            musicAudioSource.volume = 0f;
        }
        PlayMusic(mainTheme);
        PlaySound("Wind");
        //PlaySound("Achievment");
    }
    
    
    
    private IEnumerator ChangeMusicVolume(float time)
    {
        if (Mathf.Approximately(time,0f)) time = 0.01f;
        
        const float step = 0.05f;
        float volDif = Mathf.Abs(musicAudioSource.volume - musicVolume);
        float delta = volDif / (time / step); 
        
        
        while (!Mathf.Approximately(musicAudioSource.volume,musicVolume))
        {
            musicAudioSource.volume = Mathf.MoveTowards
                (musicAudioSource.volume, musicVolume, delta);
            yield return Helper.GetWait(step);
        }
    }
    
    private IEnumerator ChangeWindVolume(float time, float vol)
    {
        if (Mathf.Approximately(time,0f)) time = 0.01f;
        
        const float step = 0.05f;
        float volDif = Mathf.Abs(windAudioSource.volume - vol);
        float delta = volDif / (time / step); 
        
        
        while (!Mathf.Approximately(windAudioSource.volume,vol))
        {
            windAudioSource.volume = Mathf.MoveTowards
                (windAudioSource.volume, vol, delta);
            yield return Helper.GetWait(step);
        }
    }
    
    private IEnumerator ChangeMusicOnTransition(AudioClip mus)
    {
        SetMusicVolume(0f,0.7f);
        
        isSceneLoaded = false;
        while (!isSceneLoaded)
        {
            yield return Helper.GetWait(0.05f);
        }

        PlayMusic(mus);
        ResetMusicVolume(0.7f);
        
    }

    
    
    public float GetMusicVolume()
    {
        return musicVolume;
    }
    
    public void PlayMusicOnTransition(AudioClip mus)
    {
        StartCoroutine(ChangeMusicOnTransition(mus));
    }
    
    public void PlayMusic(AudioClip mus)
    {
        musicAudioSource.clip = mus;
        
        if (PlayerPrefs.GetInt("music") == 0) return;
        musicAudioSource.Play();
    }
    
    
    
    public void SetMusicVolume(float vol,float time = 1f)
    {
        musicVolume = vol;
        StartCoroutine(ChangeMusicVolume(time));
    }
    
    /// <summary>
    /// Reset Music Volume if sound on
    /// </summary>
    public void ResetMusicVolume(float time = 1f)
    {
        if (PlayerPrefs.GetInt("sound") == 0) return;

        musicVolume = adjustedMusicVolume;
        StartCoroutine(ChangeMusicVolume(time));
    }
    
    public void SetWindVolume(float vol,float time = 1f)
    {
        StartCoroutine(ChangeWindVolume(time, vol));
    }
    
    /// <summary>
    /// Reset Wind Volume if sound on
    /// </summary>
    public void ResetWindVolume(float time = 1f)
    {
        if (PlayerPrefs.GetInt("sound") == 0) return;
        StartCoroutine(ChangeWindVolume(time, 1f)); //-
    }
    
    
    
    
    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void StartMusic()
    {
        musicAudioSource.Play();
    }
    
    public AudioSource GetSoundSource(string soundName)
    {
        Sound sound = sounds.Find(sound => sound.soundName == soundName);
        return sound.source;
    }
    
    public void PlaySound(string soundName)
    {
        if (PlayerPrefs.GetInt("sound") == 0) return;
        
        Sound sound = sounds.Find(sound => sound.soundName == soundName);

        sound?.source.Play();
    }
    
    public void PlaySound(string soundName, float pitch)
    {
        if (PlayerPrefs.GetInt("sound") == 0) return;
        
        Sound sound = sounds.Find(sound => sound.soundName == soundName);
        if (sound != null)
        {
            sound.source.pitch = pitch;
            sound.source.Play();
        }
        
    }
    
    
    
    
}
