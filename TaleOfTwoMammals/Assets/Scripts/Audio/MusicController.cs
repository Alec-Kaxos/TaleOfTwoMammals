using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            StartCoroutine(PlayMusic(StartSong, LoopSong));
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

    public IEnumerator PlayMusic(AudioClip musicStart, AudioClip musicLoop = null, float fadeInTime = 0f, float fadeOutTime = .5f)
    {
        if (myAudioSource.isPlaying)
        {
            yield return AudioFader.FadeOut(myAudioSource, fadeOutTime);
        }
        
        if(fadeInTime == 0f)
        {
            myAudioSource.clip = musicStart;
            myAudioSource.Play();
        }
        else
        {
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
