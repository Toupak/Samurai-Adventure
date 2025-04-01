using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obelisk_Breakable : Obelisk
{
    protected override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        obeliskHP = GetComponent<EnemyHealth>();

        obeliskHP.OnTakeDamage.AddListener((attackType) =>
        {
            if (hasBeenDestroyed == false)
            {
                if (obeliskHP.hp <= 10)
                {
                    obeliskHP.isInvincible = true;
                    hasBeenDestroyed = true;
                    TriggerObeliskDestructEvent();
                }
            }
        });
    }

    protected override void TriggerObeliskDestructEvent()
    {
        SpawnDestroyedObelisk();
    }
}
