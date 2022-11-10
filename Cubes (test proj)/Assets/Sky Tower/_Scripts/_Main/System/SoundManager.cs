using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{

    public List<Sound> sounds;
    public AudioClip MainTheme, ShopTheme;

    /*[Range(0f, 1f)] 
    public float mainVolume = 1f;*/


    private bool sceneLoaded;
    private float musicVolume, musicAdjustedVolume;

    private WaitForSeconds waiter;
    private AudioSource audioSource;

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
        audioSource = GetComponent<AudioSource>();

        foreach (var sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();

            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            
        }
        
        musicAdjustedVolume = audioSource.volume;
        musicVolume = musicAdjustedVolume;
    }

    
    private void Start()
    {
        if (PlayerPrefs.GetString("sound") != "on")
        {
            audioSource.volume = 0f;
        }
        PlayMusic(MainTheme);
    }

    private void OnSceneLoaded()
    {
        sceneLoaded = true;
    }
    
    
    private IEnumerator ChangeVolume(float time)
    {
        if (Mathf.Approximately(time,0f)) time = 0.01f;
        
        const float step = 0.05f;
        float volDif = Mathf.Abs(audioSource.volume - musicVolume);
        float delta = volDif / (time / step); 
        
        waiter = new WaitForSeconds(step);
        while (!Mathf.Approximately(audioSource.volume,musicVolume))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, musicVolume, delta);
            yield return Helper.GetWait(step);
        }
    }

    
    private IEnumerator ChangeMusicOnTransition(AudioClip mus)
    {
        SetMusicVolume(0f,0.7f);
        
        sceneLoaded = false;
        while (!sceneLoaded)
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
        audioSource.clip = mus;
        
        if (PlayerPrefs.GetString("music") != "on") return;
        audioSource.Play();
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

        musicVolume = musicAdjustedVolume;
        StartCoroutine(ChangeVolume(time));
    }

    
    public void StopMusic()
    {
        audioSource.Stop();
    }
    
    
    public void StartMusic()
    {
        audioSource.Play();
    }

    
    public void PlaySound(string soundName)
    {
        if (PlayerPrefs.GetString("sound") != "on") return;
        
        var s = sounds.Find(sound => sound.soundName == soundName);
        s?.source.Play();
    }
    
}
