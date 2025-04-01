using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardController : OLDEnemyController
{
    public GameObject projectilePrefab;
    public Transform spellStartPosition;

    public float attackCooldown;
    private bool isMovingBackward;

    protected override void Update()
    {
        if (isLocked)
            return;

        if (isStagger())
            return;

        float distance = ComputeDistanceToPlayer();

        if (distance < aggroRange)
        {
            if (distance < attackRange && CanAttack())
            {
                RotateTowardsPlayer();
                StartCoroutine(Projectile());
            }
            else if (isMovingBackward)
            {
                RotateTowardsPlayer();
                MoveAwayFromPlayer();
            }
        }
        else
            StopMove();
    }

    IEnumerator Projectile()
    {
        StopMove();

        lastAttackTimeStamp = Time.time;
        isAttacking = true;

        PlayAttack();

        yield return new WaitForSeconds(attackDelay);

        GameObject projectileTemp = Instantiate(projectilePrefab, spellStartPosition.position, Quaternion.identity);
        Vector2 direction = (MainCharacter.Instance.transform.position - spellStartPosition.position).normalized;
        projectileTemp.GetComponent<Projectile>().Setup(direction);

        yield return new WaitForSeconds(attackDuration);

        isMovingBackward = true;

        yield return new WaitForSeconds(attackCooldown);

        isAttacking = false;
        isMovingBackward = false;
    }

    private void MoveAwayFromPlayer()
    {
        Vector3 position = MainCharacter.Instance.transform.position;

        Vector2 direction = (transform.position - position).normalized;

        rb.velocity = direction * speed;
        PlayMovement();
    }
}
