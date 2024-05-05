using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiIntruments : MonoBehaviour
{
    public List<AudioClip> clips;

    public AudioSource source;

    public bool pitchRandom;
    public float minPitch = 0.90f;
    public float maxPitch = 1;

    private void RandomPitch()
    {
        if (!pitchRandom) return;

        source.pitch = Random.Range(minPitch, maxPitch);
    }

    public void PlaySource()
    {
        RandomPitch(); 
        int index = Random.Range(0, clips.Count);
        source.clip = clips[index];
        source.Play();
    }
}
