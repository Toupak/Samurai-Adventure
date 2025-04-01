using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticSpike : MonoBehaviour
{
    public AttackTrigger triggerTrap;
    public AudioClip upSound;
    public AudioClip whooshUpSound;
    public AudioClip downSound;

    public float stayDownDuration;
    public float stayUpDuration;

    private Animator animator;
    private ColliderDamage colliderDamage;

    private void Start()
    {
        animator = GetComponent<Animator>();
        colliderDamage = GetComponent<ColliderDamage>();

        triggerTrap.OnAttackTrigger.AddListener(OnTrigger);
    }

    private void OnTrigger()
    {
        if (triggerTrap.isActivated)
            StartCoroutine(Activate());
    }

    private IEnumerator Activate()
    {
        while (triggerTrap.isActivated)
        {
            yield return new WaitForSeconds(stayDownDuration);
            PlayAnimationSoundUp();
            colliderDamage.ActivateDamage();

            yield return new WaitForSeconds(stayUpDuration);
            PlayAnimationSoundDown();
            colliderDamage.DeactivateDamage();
        }
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
}
