using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

[Tooltip("This script is only for testing and should be deleted. Will cause (unharmful) errors.")]
public class AudioBarController : MonoBehaviour
{
    [SerializeField] private Slider mySlider;
    [SerializeField] private TextMeshProUGUI myText;

    private bool disableUpdate = false;

    void Awake()
    {
        mySlider.onValueChanged.AddListener(TimeChange);
    }
    
    void Update()
    {
        updateUI();
    }


    //Updates the slider to the current volume of the source
    private void updateUI()
    {
        if (MusicController.GetAudioSource() == null || MusicController.GetAudioSource().clip == null) return;
        mySlider.value = MusicController.GetAudioSource().time / MusicController.GetAudioSource().clip.length;
        myText.text = MusicController.GetAudioSource().time.ToString("F2") + " / " + MusicController.GetAudioSource().clip.length.ToString("F2");
        disableUpdate = true;
    }

    private void TimeChange(float time)
    {
        MusicController.GetAudioSource().time = time * MusicController.GetAudioSource().clip.length;
    }
}

    