using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormSFX : EnemySFX
{
    public AudioClip whooshSound;

    protected override void Start()
    {
        //Listener sur OnAttack enlevé, ajouté sur OnPlayerTakeDamage à la place ligne 29

        EnemyController enemyController = GetComponent<EnemyController>();
        RangeEnemyController rangeEnemyController = GetComponent<RangeEnemyController>();

        if (enemyController != null)
        {
            enemyController.OnAnticipationAttack.AddListener(PlayAnticipationAttack);
            for (int i = 0; i < enemyController.weaponColliders.Count; i++)
            {
                enemyController.weaponColliders[i].OnDealDamage.AddListener(PlayAttack);
            }
        }

        if (rangeEnemyController != null)
        {
            rangeEnemyController.OnAttack.AddListener(PlayAttack);
        }

        GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => PlayHurt());
        GetComponent<EnemyHealth>().OnDeath.AddListener(PlayDeath);
    }


    protected override void PlayAnticipationAttack()
    {
        base.PlayAnticipationAttack();
        SFXManager.Instance.PlaySFX(whooshSound, delay: GetComponent<EnemyController>().attackDelay);
    }

    protected override void PlayAttack()
    {
        SFXManager.Instance.PlaySFX(weaponSound);
    }
}
