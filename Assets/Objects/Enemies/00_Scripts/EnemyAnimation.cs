using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    public Animator enemyAnimator;

    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;
    private EnemyController enemyController;
    private SandWormController sandWormController;

    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyController = GetComponent<EnemyController>();
        sandWormController = GetComponent<SandWormController>();
        rb = GetComponent<Rigidbody2D>();

        enemyHealth.OnTakeDamage.AddListener((_) => PlayAnimation("Hurt"));
        enemyController.OnAnticipationAttack.AddListener(() => PlayAnimation("Anticipation_Attack"));
        enemyController.OnAttack.AddListener(() => PlayAnimation("Attack"));
        
        if (sandWormController != null)
            sandWormController.OnStartCharge.AddListener(() => PlayAnimation("Travelling_Attack"));
    }

    private void Update()
    {
        if (enemyController.IsStagger())
            return;

        if (enemyController.isAttacking)
            return;

        if (rb.velocity.magnitude > 0)
            PlayAnimation("Walk");
        else
            PlayAnimation("Idle");
    }

    private void PlayAnimation(string animation)
    {
        string direction = Tools.GetLastRecordedDirection(enemyController.LastMovement);
        enemyAnimator.Play($"{animation}_{direction}");
    }
}
