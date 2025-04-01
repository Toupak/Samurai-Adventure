using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CthuluController : OLDEnemyController
{
    public AudioSource flySound;
    public float flyRange;
    public float flySpeed;

    protected override void MoveToPlayer()
    {
        float newSpeed = speed;
        
        Vector3 positionRight = MainCharacter.Instance.transform.position + Vector3.right;
        Vector3 positionLeft = MainCharacter.Instance.transform.position + Vector3.left;
        Vector3 position = positionLeft;

        if (Vector3.Distance(positionRight, transform.position) < Vector3.Distance(positionLeft, transform.position))
            position = positionRight;

        Vector2 direction = (position - transform.position).normalized;

        if (ComputeDistanceToPlayer() > flyRange)
        {
            PlayFly();
            newSpeed = flySpeed;
        }
        else
        {
            PlayMovement();
            if (flySound != null)
                flySound.Stop();
        }
            

        rb.velocity = direction * newSpeed;
    }

    private void PlayFly()
    {
        animator.Play("Fly");
        
        if (flySound != null && flySound.isPlaying == false)
        {
            flySound.pitch = Random.Range(0.95f, 1.05f);
            flySound.Play();
        }
    }
    protected override void StopMove()
    {
        rb.velocity = Vector2.zero;

        if (flySound != null)
            flySound.Stop();
    }

    protected override void PlayAttack()
    {
        animator.Play("Attack1");

        if (attackSound != null)
        {
            attackSound.pitch = Random.Range(0.95f, 1.05f);
            attackSound.PlayDelayed(attackSoundDelay);
        }
    }
}
