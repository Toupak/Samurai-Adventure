using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBoss_ScreamAttack : MonoBehaviour
{
    public SkullBoss_Head boss;
    public float timeForAttack;
    
    private float timeSpentInZone;

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            timeSpentInZone += Time.deltaTime;
            if (timeSpentInZone >= timeForAttack)
            {
                boss.ScreamAttack();
                timeSpentInZone = 0;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            timeSpentInZone = 0;
        }
    }
}
