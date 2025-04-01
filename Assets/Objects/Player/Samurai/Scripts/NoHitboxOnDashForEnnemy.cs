using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoHitboxOnDashForEnnemy : MonoBehaviour
{
    public SamuraiMovement movement;

    void Start()
    {
        SamuraiMovement.OnPlayerDash.AddListener(() => StartCoroutine(DisableHitboxForEnnemies()));
    }

    private IEnumerator DisableHitboxForEnnemies()
    {
        //Layer 7 : Player, Layer 8 : Hitbox
        Physics2D.IgnoreLayerCollision(7, 8, true);
        yield return new WaitForSeconds(movement.dashDuration);
        Physics2D.IgnoreLayerCollision(7, 8, false);
    }
}
