using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcTrainingDummy : MonoBehaviour
{
    public Transform graphicsTransform;
    private Animator orcAnim;
    private float attackDuration = 1.0f;
    public int attackCountDuringCycle;
    private CapsuleCollider2D axeCollider;
    private bool hasHit;
    public int damage;

    IEnumerator Start()
    {
        orcAnim = graphicsTransform.gameObject.GetComponent<Animator>();
        axeCollider = GetComponent<CapsuleCollider2D>();
        axeCollider.enabled = false;
        
        while (true)
        {
            int attackCount = 0;
            
            while (attackCount < attackCountDuringCycle)
            {
                orcAnim.Play("Attack");
                yield return new WaitForSeconds(0.833f);
                StartAttack();
                attackCount += 1;
                yield return new WaitForSeconds(attackDuration - 0.66f);
                StopAttack();
            }

            yield return new WaitForSeconds(attackDuration * attackCountDuringCycle);
        }
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && hasHit == false)
        {
            otherCollider.transform.parent.parent.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, otherCollider.ClosestPoint(transform.position), Vector2.zero);
            hasHit = true;
        }
    }

    private void StartAttack()
    {
        hasHit = false;
        axeCollider.enabled = true;
    }


    private void StopAttack()
    {
        axeCollider.enabled = false;
    }
}    