using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SandWormController : EnemyController
{
    [HideInInspector] public UnityEvent OnStartCharge = new UnityEvent();

    public float chargingSpeed;
    public float recoveryAfterAttack;

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
                StartCoroutine(Charge());
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


    protected IEnumerator Charge()
    {
        StopMove();
        isAttacking = true;
        lastAttackTimeStamp = Time.time;
        lastMovement = GetPlayerDirection();
        int directionIndex = Tools.GetLastRecordedDirectionAsInt(lastMovement);

        OnAnticipationAttack.Invoke();
        //ChargingAnimation() - on rajoute particules et effets visuels cool;
        yield return new WaitForSeconds(attackDelay);

        OnStartCharge.Invoke();

        weaponColliders[directionIndex].ActivateDamage();
        rb.velocity = lastMovement * chargingSpeed;

        float timer = 0;
        while (timer < attackDuration)
        {
            if (weaponColliders[directionIndex].enemyHit.Count > 0)
            {
                StopMove();
                OnAttack.Invoke();
                weaponColliders[directionIndex].DeactivateDamage();
                yield return new WaitForSeconds(recoveryAfterAttack);
                isAttacking = false;
                yield break;
            }
            yield return null;
            timer += Time.deltaTime;
        }

        OnAttack.Invoke();
        weaponColliders[directionIndex].DeactivateDamage();
        StopMove();
        yield return new WaitForSeconds(recoveryAfterAttack);
        isAttacking = false;
    }
}
