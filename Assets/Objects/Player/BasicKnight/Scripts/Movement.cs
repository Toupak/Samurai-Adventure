using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{   
    public AudioSource audioSourceSlide;

    private PlayerAnimation playerAnimation;
    private PlayerHealth playerHealth;
    private SamuraiAttack attack;
    private Rigidbody2D rb;

    private Vector2 rollDirection;
    private Vector2 slideDirection;
    public float speed;
    public float runSpeed;
    public float crouchSpeed;
    public float rollSpeed;
    public float slideSpeed;
    public float cooldownRoll;
    private float rollDuration = 0.5f;
    private float slideDuration = 0.33f;
    private float lastRollTimeStamp = 0f;
    private float lastSlideTimeStamp = 0f;

    private void Start()
    {
        playerAnimation = GetComponent<PlayerAnimation>();
        attack = GetComponent<SamuraiAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        rb = GetComponent<Rigidbody2D>();
        PlayerHealth.OnPlayerDeath.AddListener((_) => StopMovement());
    }

    private void Update()
    {   
        if (playerHealth.IsDead)
            return;
        

        if (IsBusy())
            return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && CanRoll() && !IsBusy())
        {
            Roll();
            return;
        }

        if (IsCrouching() && !IsRunning())
        {   
            CrouchWalk();
            return;
        }

        if (IsRunning())
        {
            if (Keyboard.current.leftAltKey.wasPressedThisFrame && CanSlide() && !IsBusy())
                Slide();
            else 
                Run();
        }
        else 
            Walk();
    }

    private Vector2 ComputeMoveDirection()
    {
        Vector2 inputDirection = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
            inputDirection.y = 1.0f;

        if (Keyboard.current.sKey.isPressed)
            inputDirection.y = -1.0f;

        if (Keyboard.current.dKey.isPressed)
            inputDirection.x = 1.0f;

        if (Keyboard.current.aKey.isPressed)
            inputDirection.x = -1.0f;

        return inputDirection.normalized;
    }

    private void MovePlayer(Vector2 direction, float newSpeed)
    {
        rb.velocity = direction * newSpeed;
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void Walk()
    {
        Vector2 startingPosition = transform.position;
        Vector2 inputDirection = ComputeMoveDirection();

        MovePlayer(inputDirection, speed);
    }

    private void Run()
    {
        Vector2 startingPosition = transform.position;
        Vector2 inputDirection = ComputeMoveDirection();
        
        MovePlayer(inputDirection, runSpeed);
    }

    private void CrouchWalk()
    {
        Vector2 startingPosition = transform.position;
        Vector2 inputDirection = ComputeMoveDirection();
        
        MovePlayer(inputDirection, crouchSpeed);
    }

    private void Roll()
    {
        lastRollTimeStamp = Time.time;
        rollDirection = ComputeMoveDirection();

        if (rollDirection == Vector2.zero)
            rollDirection = playerAnimation.GetLookDirection();

        MovePlayer(rollDirection, rollSpeed);
    }

    private void Slide()
    {
        lastSlideTimeStamp = Time.time;
        slideDirection = ComputeMoveDirection();

        if (slideDirection == Vector2.zero)
            slideDirection = playerAnimation.GetLookDirection();

        MovePlayer(slideDirection, slideSpeed);

        audioSourceSlide.pitch = Random.Range(0.95f, 1.05f);
        audioSourceSlide.Play();
    }    
    
    #region CONDITIONCHECK
    public bool IsBusy()
    {
        return (IsSliding() || IsRolling() || attack.IsAttacking());
    }

    public bool IsMoving()
    {
        return (rb.velocity.magnitude > 0 && IsBusy() == false);
    }

    public bool CanSlide()
    {
        return Time.time - lastSlideTimeStamp >= slideDuration;
    }

    public bool CanRoll()
    {
        return Time.time - lastRollTimeStamp >= rollDuration + cooldownRoll;
    }

    public bool IsRolling()
    {
        return Time.time > rollDuration && Time.time - lastRollTimeStamp < rollDuration;
    }

    public bool IsSliding()
    {
        return Time.time > slideDuration && Time.time - lastSlideTimeStamp < slideDuration;
    }

    public bool IsRunning()
    {
        return Keyboard.current.leftShiftKey.isPressed;
    }

    public bool IsCrouching()
    {
        return Keyboard.current.leftAltKey.isPressed;
    }
    #endregion
}