using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class BoDController : OLDEnemyController
{
    private bool hasAggroed;
    public float castRange;
    public GameObject spellPrefab;
    private float castDuration = 1.1f;
    private bool isCasting;
    private float lastCastTimeStamp = 0f;

    protected override void Update()
    {
        if (isLocked)
            return;

        if (Time.time - staggerTimeStamp < staggerDuration)
            return;

        float distance = ComputeDistanceToPlayer();

        if (distance < aggroRange && isCasting == false)
        {
            hasAggroed = true;
            
            if (distance < attackRange && CanAttack())
            {
                RotateTowardsPlayer();
                StartCoroutine(Attack());
            }
            else if (!isAttacking)
            {
                RotateTowardsPlayer();
                MoveToPlayer();
            }
        }
        else
            StopMove();

        if (hasAggroed == true && distance < castRange && distance > aggroRange && CanCast())
        {
            StartCoroutine(CastSpell());
        }
    }

    private IEnumerator CastSpell()
    {
        Vector3 position = MainCharacter.Instance.transform.position + new Vector3(0.0f, 1.4f, 0.0f);
        lastCastTimeStamp = Time.time;
        isCasting = true;

        animator.Play("Cast");

        GameObject spellTemp = Instantiate(spellPrefab, position, Quaternion.identity);
        Destroy(spellTemp, 1.0f);

        yield return new WaitForSeconds(1.1f);
        isCasting = false;
    }

    private bool CanCast()
    {
        return Time.time - lastCastTimeStamp >= castDuration && isCasting == false && isAttacking == false; 
    }

    protected override void CancelAttack()
    {
        base.CancelAttack();

        isCasting = false;
    }
}