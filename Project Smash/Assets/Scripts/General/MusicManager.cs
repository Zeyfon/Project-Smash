using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioClip relaxedMusic = null;
    [SerializeField] AudioClip tenseMusic = null;
    [SerializeField] float ambientTrackFadeOutTime = 2;
    [SerializeField] float ambientTrackFadeInTime = 2;
    [SerializeField] float ambientMusicVolume = 1;

    AudioSource audioSource;
    bool tenseMusicOn = false;
    bool relaxedMusicOn = false;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = ambientMusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayTenseMusic()
    {
        if (tenseMusicOn) return;
        tenseMusicOn = true;
        relaxedMusicOn = false;
        StartCoroutine(PlayThisTrack(tenseMusic));
    }

    public void PlayRelaxedMusic()
    {
        if (relaxedMusicOn) return;
        tenseMusicOn = false;
        relaxedMusicOn = true;
        StartCoroutine(PlayThisTrack(relaxedMusic));

    }

    IEnumerator PlayThisTrack(AudioClip track)
    {
        if (audioSource.isPlaying)
        {
            yield return FadeOutOtherTrack();
        }
        else
        {
            audioSource.volume = 0;
        }
        audioSource.clip = track;
        audioSource.Play();

        yield return FadeInThisTrack();
    }

    IEnumerator FadeOutOtherTrack()
    {
        //print("Fading Out Music");
        float maxVolume = ambientMusicVolume;
        float time = 0;
        float currentVolume = audioSource.volume;
        while (currentVolume > 0)
        {
            time += Time.deltaTime;
            currentVolume = ((ambientTrackFadeOutTime - time) * maxVolume) / ambientTrackFadeOutTime;
            if (currentVolume < 0) currentVolume = 0;
            //print(currentVolume);
            audioSource.volume = currentVolume;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FadeInThisTrack()
    {
        //print("Fading In Music");
        float maxVolume = ambientMusicVolume;
        float time = 0;
        float currentVolume = audioSource.volume;
        while (currentVolume < maxVolume)
        {
            time += Time.deltaTime;
            currentVolume = ( time * maxVolume) / ambientTrackFadeOutTime;
            if (currentVolume > maxVolume) currentVolume = maxVolume;
            //print(currentVolume);
            audioSource.volume = currentVolume;
            yield return new WaitForEndOfFrame();
        }
    }
}
