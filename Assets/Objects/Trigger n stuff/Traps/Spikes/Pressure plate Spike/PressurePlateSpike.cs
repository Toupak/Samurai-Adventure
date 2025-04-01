using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlateSpike : MonoBehaviour
{
    public AudioClip upSound;
    public AudioClip whooshUpSound;
    public AudioClip stepOnTrapSound;
    public AudioClip downSound;

    public ColliderDamage colliderDamage;

    public float stayDownDuration;
    public float stayUpDuration;
    public float cdBetweenActivation;

    private float lastActivationTimeStamp;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && CanBeSteppedOn())
        {
            lastActivationTimeStamp = Time.time;
            StartCoroutine(Activate());
        }
    }

    private IEnumerator Activate()
    {
        SFXManager.Instance.PlaySFX(stepOnTrapSound);

        yield return new WaitForSeconds(stayDownDuration);
        PlayAnimationSoundUp();
        colliderDamage.ActivateDamage();

        yield return new WaitForSeconds(stayUpDuration);
        PlayAnimationSoundDown();
        colliderDamage.DeactivateDamage();
    }

    private void PlayAnimationSoundUp()
    {
        animator.Play("Activated");
        SFXManager.Instance.PlaySFX(upSound);
        SFXManager.Instance.PlaySFX(whooshUpSound);
    }

    private void PlayAnimationSoundDown()
    {
        animator.Play("Deactivated");
        SFXManager.Instance.PlaySFX(downSound);
    }

    private bool CanBeSteppedOn()
    {
        return Time.time - lastActivationTimeStamp > cdBetweenActivation;
    }
}
