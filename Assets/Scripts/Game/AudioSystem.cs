using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSystem : SceneSingleton<AudioSystem>
{
    AudioSource audioData;
    AudioClip song;

    void Start()
    {
        song = Resources.Load<AudioClip>("kinotrack");
        audioData = GetComponent<AudioSource>();
        
        Debug.Log("started");
    }

    public void PlaySong()
    {
        audioData.PlayOneShot(song);
        Debug.Log("Playing song");
    }
}
