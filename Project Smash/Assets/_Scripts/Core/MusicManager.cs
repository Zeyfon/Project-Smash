using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class MusicManager : MonoBehaviour
    {
        //CONFIG
        [SerializeField] AudioClip levelMusic = null;
        [Range(0,1)]
        [SerializeField] float levelMusicVolume = 1;

        [SerializeField] AudioClip bossMusic = null;
        [Range(0, 1)]
        [SerializeField] float bossMusicVolume = 1;

        [SerializeField] float fadeTime = 2;


        //STATE
        AudioSource audioSource;

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
            PlayLevelMusic();
        }

        public void PlayBossMusic()
        {
            print("Play Boss Music");
            StartCoroutine(PlayThisTrack(bossMusic, bossMusicVolume));
        }

        public void PlayLevelMusic()
        {
            print("PlayLevel Music");
            StartCoroutine(PlayThisTrack(levelMusic, levelMusicVolume));
        }

        IEnumerator PlayThisTrack(AudioClip track, float volume)
        {
            if (audioSource.isPlaying)
            {
                yield return FadeOutOtherTrack(volume);
            }
            else
            {
                audioSource.volume = 0;
            }
            audioSource.clip = track;
            audioSource.Play();

            yield return FadeInThisTrack(volume);
        }

        IEnumerator FadeOutOtherTrack(float maxVolume)
        {
            print("Fading Out Music");
            float currentVolume = audioSource.volume;
            while (currentVolume > 0)
            {
                currentVolume -= ((fadeTime - Time.deltaTime) * maxVolume) / fadeTime;
                if (currentVolume < 0) currentVolume = 0;
                audioSource.volume = currentVolume;
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator FadeInThisTrack(float maxVolume)
        {
            print("Fading In Music");
            float currentVolume = audioSource.volume;
            while (currentVolume < maxVolume)
            {
                currentVolume += (Time.deltaTime * maxVolume) / fadeTime;
                if (currentVolume > maxVolume) currentVolume = maxVolume;
                audioSource.volume = currentVolume;
                yield return new WaitForEndOfFrame();
            }
        }
    }


}
