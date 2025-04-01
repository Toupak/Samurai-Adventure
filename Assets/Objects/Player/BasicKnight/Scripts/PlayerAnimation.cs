using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    public Transform graphicsTransform;
    private Movement movement;
    private SamuraiAttack attack;
    private Animator playerAnim;
    private Rigidbody2D rb;
    private PlayerHealth playerHealth;

    private void Start()
    {
        movement = GetComponent<Movement>();
        attack = GetComponent<SamuraiAttack>();
        rb = GetComponent<Rigidbody2D>();
        playerHealth = GetComponent<PlayerHealth>();
        playerAnim = graphicsTransform.gameObject.GetComponent<Animator>();
    }

    private void LateUpdate()
    {
        if (rb.velocity.x > 0)
            graphicsTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (rb.velocity.x < 0)
            graphicsTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);

        if (playerHealth.IsDead)
        {
            if (playerHealth.IsDying)
                playerAnim.Play("MC_Death");
            else 
                playerAnim.Play("MC_Dead");
            return;
        }

        if (attack.IsAttacking() == true)
        {
            if (movement.IsCrouching() == true)
                playerAnim.Play("MC_CrouchAttack");
            else
                playerAnim.Play("MC_AttackAnim");
            return;
        }
        
        if (movement.IsRolling() == true)
        {    
            playerAnim.Play("MC_Roll");
            return;
        }

        if (movement.IsCrouching() == true && movement.IsSliding() == false)
        {
            if (rb.velocity.magnitude > 0)
                playerAnim.Play("MC_CrouchWalk");
            else 
                playerAnim.Play("MC_Crouch");
            return;
        }

        if (rb.velocity.magnitude > 0)
        {
            if (movement.IsRunning() == true)
            {
                if (movement.IsSliding() == true)
                    playerAnim.Play("MC_Slide");
                else 
                    playerAnim.Play("MC_Run");
            }
            else 
                playerAnim.Play("MC_Movement");
        }
        else 
            playerAnim.Play("MC_Idle");
    }

    public Vector2 GetLookDirection()
    {
        if (graphicsTransform.localScale.x > 0)
            return Vector2.right;
        return Vector2.left;
    }
}