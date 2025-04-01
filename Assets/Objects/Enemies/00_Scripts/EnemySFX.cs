using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class EnemySFX : MonoBehaviour
{
    public List<AudioClip> hurtSounds;
    public AudioClip attackSound;
    public AudioClip weaponSound;
    public AudioClip deathSound;

    protected virtual void Start()
    {
        EnemyController enemyController = GetComponent<EnemyController>();
        RangeEnemyController rangeEnemyController = GetComponent<RangeEnemyController>();
        
        if (enemyController != null)
        {
            enemyController.OnAnticipationAttack.AddListener(PlayAnticipationAttack);
            enemyController.OnAttack.AddListener(PlayAttack);
        }

        if (rangeEnemyController != null)
        {
            rangeEnemyController.OnAttack.AddListener(PlayAttack);
        }

        GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => PlayHurt());
        GetComponent<EnemyHealth>().OnDeath.AddListener(PlayDeath);
    }

    protected virtual void PlayHurt()
    {
        SFXManager.Instance.PlayRandomSFXAtLocation(hurtSounds.ToArray(), transform);
    }

    protected virtual void PlayAnticipationAttack()
    {
        SFXManager.Instance.PlaySFX(attackSound);
    }

    protected virtual void PlayAttack()
    {
        SFXManager.Instance.PlaySFX(weaponSound);
    }

    protected void PlayDeath()
    {
        SFXManager.Instance.PlaySFX(deathSound);
    }
}
