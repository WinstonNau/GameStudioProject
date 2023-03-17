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
    public AudioClip audioBattleMusic;
    public AudioClip audioDamage;
    public AudioClip audioHeal;

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

    public void PlayBattleMusic()
    {
        audioSource.PlayOneShot(audioBattleMusic);
    }

    public void PlayDamage()
    {
        audioSource.PlayOneShot(audioDamage);
    }

    public void PlayHeal()
    {
        audioSource.PlayOneShot(audioHeal);
    }

    public void StopAudio()
    {
        audioSource.Stop();
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
