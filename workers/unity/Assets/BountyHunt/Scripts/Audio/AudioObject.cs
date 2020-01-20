using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (AudioSource))]
public class AudioObject : MonoBehaviour
{

    AudioSource source;


    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.loop = false;
        source.playOnAwake = false;
    }
    public void Play(AudioClip clip)
    {
        source.clip = clip;
        StartCoroutine(PlayAndDestroyEnumerator());
    }

    IEnumerator PlayAndDestroyEnumerator()
    {
        source.Play();
        yield return new WaitWhile(() => source.isPlaying);
        Destroy(gameObject);
    }
}