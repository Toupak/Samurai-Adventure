using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class MeowSounds : MonoBehaviour
{
    public AudioSource meowAudioSource;
    public List<AudioClip> meowSounds;
    public MeowRange meowRange;

    private bool isInRange;

    public void MeowSound()
    {
        if (meowRange.isInRange == true)
        {
            AudioClip currentMeowSound = null;
            currentMeowSound = meowSounds[Random.Range(0, meowSounds.Count)];
            meowAudioSource.clip = currentMeowSound;
            meowAudioSource.pitch = Random.Range(0.95f, 1.05f);
            meowAudioSource.Play();
        }
    }
}
