using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SamuraiAnimation : MonoBehaviour
{
    public Transform graphicsTransform;

    private SamuraiMovement movement;
    private SamuraiAttack playerAttack;
    private Animator playerAnimator;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth;
    private LockEnemies lockEnemies;

    private float hurtTimeStamp;
    private float hurtAnimationDuration = 0.25f; // durée de l'animation dans l'animator potentiellement modifiable
    

    private void Start()
    {
        movement = GetComponent<SamuraiMovement>();
        playerAttack = GetComponent<SamuraiAttack>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnimator = graphicsTransform.gameObject.GetComponent<Animator>();
        lockEnemies = GetComponent<LockEnemies>();

        SamuraiMovement.OnPlayerStagger.AddListener(() =>
        {
            hurtTimeStamp = Time.time;
            PlayAnimation("Hurt");
        });

        PlayerHealth.OnPlayerDeath.AddListener((_) => PlayAnimation("Hurt"));

        SamuraiAttack.OnPlayerAttack.AddListener(() =>
        {
            if (playerAttack.whichCombo < 3)
                PlayAnimation("Attack");
            else
                PlayAnimation("Attack3");
        });
    }

    private void LateUpdate()
    {
        if (playerHealth.IsDead)
            return;

        if (playerAttack.IsAttacking() == true) 
            return;

        if (movement.IsDashing() == true)
        {
            PlayAnimation("Dash");
            return;
        }

        if (Time.time - hurtTimeStamp > hurtAnimationDuration)
        {
            if (rb.velocity.magnitude > 0)
                PlayAnimation("Walk");
            else
                PlayAnimation("Idle");
        }
    }

    private void PlayAnimation(string animation)
    {
        string direction;

        if (lockEnemies.IsTargeting() == false)
            direction = Tools.GetLastRecordedDirection(movement.LastMovement);
        else
            direction = Tools.GetLastRecordedDirection(lockEnemies.GetTargetDirection());


        playerAnimator.Play($"{animation}_{direction}");
    }
}