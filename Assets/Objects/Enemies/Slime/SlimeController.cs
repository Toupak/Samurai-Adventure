using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeController : EnemyController
{
    public float chargeSpeed;
    public ColliderDamage colliderDamage;

    protected override void Update()
    {
        if (isLocked)
            return;

        if (isPacific)
            return;

        if (IsBusy() || isAttacking)
            return;

        float distance = ComputeDistanceToPlayer();

        if (distance < aggroRange)
        {
            if (distance <= attackRange && CanAttack())
            {
                StartCoroutine(Attack());
            }
            else if (distance > attackRange && !isAttacking)
            {
                MoveToPlayer();
            }
            else
                StopMove();
        }
        else
            StopMove();


    }

    protected override IEnumerator Attack()
    {
        StopMove();
        isAttacking = true;
        lastMovement = GetPlayerDirection();
        int directionIndex = Tools.GetLastRecordedDirectionAsInt(lastMovement);
        lastAttackTimeStamp = Time.time;

        OnAnticipationAttack.Invoke();
        yield return new WaitForSeconds(attackDelay);

        rb.velocity = lastMovement * chargeSpeed;

        colliderDamage.ActivateDamage();
        OnAttack.Invoke();
        //Charge()

        yield return new WaitForSeconds(attackDuration);
        colliderDamage.DeactivateDamage();
        isAttacking = false;
    }

    protected override void CancelAttack()
    {
        StopAllCoroutines();
        isAttacking = false;
    }

}
