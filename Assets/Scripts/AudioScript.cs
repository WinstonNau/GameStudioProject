using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip audioWon;
    public AudioClip audioLost;
    public AudioClip audioButtonClick;

    public void PlayMoveAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void LoopAudio()
    {
        audioSource.PlayOneShot(audioClip);
    }

    public void PlayWonAudio()
    {
        audioSource.PlayOneShot(audioWon);
    }

    public void PlayLostAudio()
    {
        audioSource.PlayOneShot(audioLost);
    }

    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(audioButtonClick);
    }

    private void Awake()
    {
        if (this.name == "Audio")
        {
            Debug.Log("playing audio");
            LoopAudio();
        }
    }
}
