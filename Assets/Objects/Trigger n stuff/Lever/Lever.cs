using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lever : Interactable
{
    protected Animator leverAnimator;
    protected AudioSource leverSound;

    protected Squeeze squeeze;

    protected virtual void Start()
    {
        leverAnimator = GetComponent<Animator>();
        leverSound = transform.GetChild(0).GetComponent<AudioSource>();
        squeeze = GetComponent<Squeeze>();

        OnTrigger.AddListener(LeverEvent);
    }

    private void LeverEvent()
    {
        LeverAnimation();

        if (squeeze != null)
            squeeze.Trigger();
    }

    protected void LeverAnimation()
    {
        leverAnimator.Play("lever1");
        leverSound.pitch = Random.Range(0.95f, 1.05f);
        leverSound.volume = Random.Range(0.98f, 1.02f);
        leverSound.Play();
    }
}
