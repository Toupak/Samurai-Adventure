using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoHurtboxOnDash : MonoBehaviour
{
    public SamuraiMovement movement;

    void Start()
    {
        SamuraiMovement.OnPlayerDash.AddListener(() => StartCoroutine(DisableHitboxForEnnemies()));
    }

    private IEnumerator DisableHitboxForEnnemies()
    {
        //Layer 16 : PlayerHurtbox, Layer 11 : EnemyAttack
        Physics2D.IgnoreLayerCollision(16, 11, true);
        yield return new WaitForSeconds(movement.dashDuration);
        Physics2D.IgnoreLayerCollision(16, 11, false);
    }
}
