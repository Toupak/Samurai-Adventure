using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestZoneEnemyHealth : EnemyHealth
{
    protected override void Start()
    {
        squeeze = GetComponent<Squeeze>();
        enemyController = GetComponent<EnemyController>();
        playerAttack = MainCharacter.Instance.GetComponent<SamuraiAttack>();

        startingPosition = transform.position;
        maxHp = hp;
    }
}
