using System;
using UnityEngine;
using UnityEngine.Audio;


[Serializable]
public class Sound
{
    public string soundName;
    public AudioMixerGroup audioMixerGroup;
    
    [Range(0f,1f)] 
    public float volume=0.5f;
    [Range(-3f,3f)]
    public float pitch=1f;

    public bool loop;

    public AudioClip clip;
    
    [NonSerialized]
    public AudioSource source;
}
