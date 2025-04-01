using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepsSounds : MonoBehaviour
{
    public AudioSource stepAudioSource;
    public List<AudioClip> stepSoundsList;

    public void StepSound()
    {
        AudioClip currentStepSound = null;
        currentStepSound = stepSoundsList[Random.Range(0, stepSoundsList.Count)];
        stepAudioSource.clip = currentStepSound;
        stepAudioSource.pitch = Random.Range(0.95f, 1.05f);
        stepAudioSource.Play();
    }
}
