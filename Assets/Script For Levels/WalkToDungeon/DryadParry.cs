using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DryadParry : MonoBehaviour
{
    public List<EnemyHealth> enemyList;

    private bool isInRange;

    void Start()
    {
        BubbleShield.OnPlayerParry.AddListener((_) => KillDryad());
    }

    protected void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            isInRange = true;
        }
    }

    protected void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            isInRange = false;
        }
    }

    private void KillDryad()
    {
        if (isInRange == true)
        {
            for (int i = 0; i < enemyList.Count; i++)
            {
                enemyList[i].GetComponent<EnemyHealth>().TakeDamage(1, 0, ColliderDamage.AttackType.EnemyProjectile);
            }
        }
    }
}
