using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    private static MusicController myInstance;

    public static MusicController GetMusicController()
    {
        return myInstance;
    }
    public static AudioSource GetAudioSource()
    {
        if (myInstance == null) return null;
        return myInstance.myAudioSource;
    }

    private AudioSource myAudioSource;
    private AudioClip myMusicLoop;

    [SerializeField]
    private AudioClip StartSong;
    [SerializeField]
    private AudioClip LoopSong;

    void Awake()
    {
        DontDestroyOnLoad(this);

        //Singleton check
        if (myInstance == null)
        {
            myInstance = this;
            myAudioSource = GetComponent<AudioSource>();
            PlayStartMusic();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Update()
    {
        if (!myAudioSource.isPlaying && myMusicLoop != null)
        {
            myAudioSource.clip = myMusicLoop;
            myAudioSource.Play();
            myAudioSource.loop = true;
            myMusicLoop = null;
        }
    }

    public void PlayStartMusic(float fadeInTime = 0f, float fadeOutTime = 1.5f)
    {
        PlayMusic2(StartSong, LoopSong, fadeInTime, fadeOutTime);
    }

    public void PlayMusic2(AudioClip musicStart, AudioClip musicLoop = null, float fadeInTime = 0f, float fadeOutTime = 1.5f)
    {
        StartCoroutine(PlayMusic(musicStart, musicLoop, fadeInTime, fadeOutTime));
    }

    public void PlayMusicLoopFirst(AudioClip musicLoop, AudioClip musicStart = null, float fadeInTime = 0f, float fadeOutTime = 1.5f)
    {
        if (musicStart == null)
            StartCoroutine(PlayMusic(musicLoop, musicStart, fadeInTime, fadeOutTime));
        else
            StartCoroutine(PlayMusic(musicStart, musicLoop, fadeInTime, fadeOutTime));
    }

    public IEnumerator PlayMusic(AudioClip musicStart, AudioClip musicLoop = null, float fadeInTime = 0f, float fadeOutTime = 1.5f)
    {
        if (musicStart == myAudioSource.clip)
        {
            yield break;
        }

        if (myAudioSource.isPlaying)
        {
            yield return AudioFader.FadeOut(myAudioSource, fadeOutTime);
        }

        myAudioSource.clip = musicStart;
        myAudioSource.Play();
        if (fadeInTime == 0f)
        {
        }
        else
        {
            myAudioSource.Pause();
            yield return AudioFader.FadeIn(myAudioSource, fadeInTime);
        }

        myMusicLoop = musicLoop;
        if (myMusicLoop != null)
        {
            myAudioSource.loop = false;
        }
        else
        {
            myAudioSource.loop = true;
        }
    }

}
