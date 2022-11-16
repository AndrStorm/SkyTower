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

    
    private void OnEnable()
    {
       SceneLoader.OnFininshedLoadingScene += OnSceneLoaded;
    }

    private void OnDisable()
    {
      SceneLoader.OnFininshedLoadingScene -= OnSceneLoaded;
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
        if (PlayerPrefs.GetString("sound") != "on")
        {
            musicAudioSource.volume = 0f;
        }
        PlayMusic(mainTheme);
        //PlaySound("Wind");
        //PlaySound("Achievment");
    }

    private void OnSceneLoaded()
    {
        isSceneLoaded = true;
    }
    
    
    private IEnumerator ChangeVolume(float time)
    {
        if (Mathf.Approximately(time,0f)) time = 0.01f;
        
        const float step = 0.05f;
        float volDif = Mathf.Abs(musicAudioSource.volume - musicVolume);
        float delta = volDif / (time / step); 
        
        
        while (!Mathf.Approximately(musicAudioSource.volume,musicVolume))
        {
            musicAudioSource.volume = Mathf.MoveTowards(musicAudioSource.volume, musicVolume, delta);
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
        
        if (PlayerPrefs.GetString("music") != "on") return;
        musicAudioSource.Play();
    }
    
    
    public void SetMusicVolume(float vol,float time = 1f)
    {
        musicVolume = vol;
        StartCoroutine(ChangeVolume(time));
    }
    
    
    /// <summary>
    /// Reset Music Volume if sound on
    /// </summary>
    public void ResetMusicVolume(float time = 1f)
    {
        if (PlayerPrefs.GetString("sound") != "on") return;

        musicVolume = adjustedMusicVolume;
        StartCoroutine(ChangeVolume(time));
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
        if (PlayerPrefs.GetString("sound") != "on") return;
        
        Sound sound = sounds.Find(sound => sound.soundName == soundName);
        sound?.source.Play();
    }
    
}
