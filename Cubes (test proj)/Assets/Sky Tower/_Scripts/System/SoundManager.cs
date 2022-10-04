using UnityEngine;

public class SoundManager : PersistentSingleton<SoundManager>
{

    [SerializeField] AudioClip buttonClickSound, explodeSound, cubeSpawnSound;

    [Range(0f,1f)]
    public float mainVolume = 1f, cubeSpawningVolume = 1f;

    AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();
        audioSource = GetComponent<AudioSource>();   
    }

    public void PlayButtonSound()
    {
        PlaySound(buttonClickSound, mainVolume);   
    }

    public void PlayExplodeSound()
    {
        PlaySound(explodeSound, mainVolume);
    }

    public void PlayCubeSpawnSound()
    {
        
        PlaySound(cubeSpawnSound, mainVolume * cubeSpawningVolume);
    }

    private void PlaySound (AudioClip clip, float volume)
    {
        if (PlayerPrefs.GetString("sound") != "on") return;

        audioSource.volume = volume;
        audioSource.clip = clip;
        audioSource.Play();
    }
}
