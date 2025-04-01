using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyAnimation : MonoBehaviour
{
    public Animator enemyAnimator;

    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;
    private RangeEnemyController rangeEnemyController;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        rangeEnemyController = GetComponent<RangeEnemyController>();
        rb = GetComponent<Rigidbody2D>();

        enemyHealth.OnTakeDamage.AddListener((_) => PlayAnimation("Hurt"));
        rangeEnemyController.OnAnticipationAttack.AddListener(() => PlayAnimation("Anticipation_Attack"));
        rangeEnemyController.OnAttack.AddListener(() => PlayAnimation("Attack"));
    }

    private void Update()
    {
        if (rangeEnemyController.IsStagger())
            return;

        if (rangeEnemyController.isAttacking)
            return;

        if (rb.velocity.magnitude > 0)
            PlayAnimation("Walk");
        else
            PlayAnimation("Idle");
    }

    private void PlayAnimation(string animation)
    {
        string direction = Tools.GetLastRecordedDirection(rangeEnemyController.LastMovement);
        enemyAnimator.Play($"{animation}_{direction}");
    }
}
