using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiSFX : MonoBehaviour
{
    public AudioClip swordImpact;
    public AudioClip swordImpact2;
    public AudioClip swordAttack1;
    public AudioClip swordAttack2;
    public AudioClip swordAttack3;
    public AudioClip castFireball;
    public AudioClip shield;
    public AudioClip shieldDamage;
    public AudioClip shieldDestroy;
    public AudioClip shieldRelease;
    public AudioClip parry;
    public AudioClip parryMagicEffect;
    public AudioClip dash;
    public AudioClip dashEnd;
    public AudioClip hurt;
    public AudioClip hurt2;
    public AudioClip LowHPpanting;
    public AudioClip death;
    public AudioClip death2;

    private SamuraiMovement samuraiMovement;
    private SamuraiAttack samuraiAttack;
    private SamuraiFireball samuraiFireball;
    private PlayerHealth playerHealth;

    //Footsteps sur un autre script attaché aux graphics/animation
    void Start()
    {
        samuraiMovement = GetComponent<SamuraiMovement>();
        samuraiAttack = GetComponent<SamuraiAttack>();
        samuraiFireball = GetComponent<SamuraiFireball>();
        playerHealth = GetComponent<PlayerHealth>();

        SamuraiAttack.OnHitEnemy.AddListener((spawnPosition,attackType) =>
        {
            if (attackType == ColliderDamage.AttackType.Sword)
                PlaySwordImpact();
        });
        SamuraiAttack.OnPlayerAttack.AddListener(PlaySwordAttack);
        BubbleShield.OnPlayerShield.AddListener(PlayShield);
        BubbleShield.OnPlayerShieldDamage.AddListener((_,_) => PlayShieldDamage());
        BubbleShield.OnPlayerShieldDestroy.AddListener((_,_) => PlayShieldDestroy());
        BubbleShield.OnPlayerParry.AddListener((_) => PlayParry());
        BubbleShield.OnPlayerShieldRelease.AddListener(PlayReleaseShield);
        SamuraiMovement.OnPlayerDash.AddListener(PlayDash);
        PlayerHealth.OnPlayerTakeDamage.AddListener((_, _) => PlayHurt());
        PlayerHealth.OnPlayerDeath.AddListener((_) => PlayDeath());
        PlayerHealth.OnPlayerLowHP.AddListener(() => StartCoroutine(PlayLowHP()));
    }

    private void PlaySwordImpact()
    {
        SFXManager.Instance.PlaySFX(swordImpact);

        if (samuraiAttack.whichCombo == 3)
            SFXManager.Instance.PlaySFX(swordImpact2);
    }

    private void PlaySwordAttack()
    {
        if (samuraiAttack.whichCombo == 1)
            SFXManager.Instance.PlaySFX(swordAttack1);

        if (samuraiAttack.whichCombo == 2)
            SFXManager.Instance.PlaySFX(swordAttack2);

        if (samuraiAttack.whichCombo == 3)
            SFXManager.Instance.PlaySFX(swordAttack3);
    }
    private void PlayDash()
    {
        SFXManager.Instance.PlaySFX(dash);
        SFXManager.Instance.PlaySFX(dashEnd, 0.02f, samuraiMovement.dashDuration);
    }

    private void PlayHurt()
    {
        SFXManager.Instance.PlaySFX(hurt);
        SFXManager.Instance.PlaySFX(hurt2);
    }

    private void PlayDeath()
    {
        SFXManager.Instance.PlaySFX(death);
        SFXManager.Instance.PlaySFX(death2);
    }

    private void PlayShield()
    {
        SFXManager.Instance.PlaySFX(shield, volume:0.05f);
    }

    private void PlayShieldDamage()
    {
        SFXManager.Instance.PlaySFX(shieldDamage);
    }

    private void PlayParry()
    {
        SFXManager.Instance.PlaySFX(parry, volume:0.05f);
        SFXManager.Instance.PlaySFX(parryMagicEffect, volume:0.05f);
    }

    private void PlayShieldDestroy()
    {
        SFXManager.Instance.PlaySFX(shieldDestroy);
    }

    private void PlayReleaseShield()
    {
        SFXManager.Instance.PlaySFX(shieldRelease, volume:0.05f);
    }

    private IEnumerator PlayLowHP()
    {
        while (playerHealth.hearts == 1)
        {
            SFXManager.Instance.PlaySFX(LowHPpanting);
            yield return new WaitForSeconds(3.0f);
        }
    }
}
