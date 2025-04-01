using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]

public class SamuraiMovement : MonoBehaviour
{
    public bool saveOn;

    public static UnityEvent OnPlayerDash = new UnityEvent();
    public static UnityEvent OnPlayerStagger = new UnityEvent();
    public List<GameObject> dashlist;
    [Range(1, 100)] public int remnantImageCount;

    public float speed;
    public float dashSpeed;
    public float cooldownDash;
    public float dashDuration;
    public float attackDashSpeed;
    public float decelerationSpeed;
    public float selfKnockbackSpeed;
    public float staggerDuration;
    public float knockbackSpeed;

    private PlayerHealth playerHealth;
    private SamuraiAttack playerAttack;
    private BubbleShield shield;
    private Rigidbody2D rb;
    private SamuraiFireball samuraiFireball;
    private LockEnemies lockEnemies;

    private Vector2 dashDirection;
    [HideInInspector] public float lastDashTimeStamp;
    private float lastStaggerTimeStamp;

    [HideInInspector] public Vector2 lastMovement = Vector2.right;
    public Vector2 LastMovement => lastMovement;


    private IEnumerator Start()
    {
        playerAttack = GetComponent<SamuraiAttack>();
        playerHealth = GetComponent<PlayerHealth>();
        shield  = GetComponent<BubbleShield>();
        samuraiFireball = GetComponent<SamuraiFireball>();
        rb = GetComponent<Rigidbody2D>();
        lockEnemies = GetComponent<LockEnemies>();

        DialoguePanel.OnTriggerDialogue.AddListener(StopMovement);
        PlayerHealth.OnPlayerDeath.AddListener((_) => StopMovement());
        PlayerHealth.OnPlayerTakeDamage.AddListener((_,d) => Stagger(d, knockbackSpeed, staggerDuration));
        SamuraiAttack.OnPlayerAttack.AddListener(AttackDash);
        SamuraiAttack.OnHitEnemy.AddListener((_, attackType) =>
        {
            if (attackType == ColliderDamage.AttackType.Sword)
                SelfKnockback();
        });
        BubbleShield.OnPlayerShield.AddListener(StopMovement);
        BubbleShield.OnPlayerShieldDamage.AddListener((_,d) => Stagger(d, knockbackSpeed, staggerDuration));
        BubbleShield.OnPlayerShieldDestroy.AddListener((_,d) => Stagger(d, knockbackSpeed, staggerDuration));

        yield return new WaitForEndOfFrame();

        if (saveOn == true && SaveManager.Instance.GetSaveData().currentFountainPosition != Vector3.zero)
        {
            MainCharacter.Instance.transform.position = SaveManager.Instance.GetSaveData().currentFountainPosition;
            CameraConstrainer.OnPlayerRespawnPingPosition.Invoke(SaveManager.Instance.GetSaveData().currentFountainPosition);
        }
    }

    private void Update()
    {
        PlayerInput.UpdateInputBuffer();        // Update les buffers mais ne joue pas sur le movement
        
        if (playerHealth.IsDead)
            return;
            
        if (IsBusy())
            return;

        if (CanDash() && !IsBusy() && PlayerInput.GetDashInput())       //mettre PlayerInput à la fin du If
        {
            Dash();
            return;
        }

         Walk();
    }

    private void FixedUpdate()
    {
        if (playerAttack.IsAttacking() || IsStagger())
        {
            SlowDown();
            return;
        }
    }

    private void MovePlayer(Vector2 direction, float newSpeed)
    {
        if (direction.magnitude > 0.01f)
            lastMovement = (direction * newSpeed).normalized;

        rb.velocity = direction * newSpeed;
    }

    public void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    private void Walk()
    {
        Vector2 inputDirection = PlayerInput.ComputeMoveDirection();

        float speedTemp = samuraiFireball.isCharging ? speed / 1.75f : speed; 

        MovePlayer(inputDirection, speedTemp);
    }

    private void AttackDash()
    {
        StopMovement();

        if (!lockEnemies.IsTargeting())
            dashDirection = PlayerInput.ComputeMoveDirection();
        else
            dashDirection = lockEnemies.GetTargetDirection();

        if (dashDirection == Vector2.zero)
            dashDirection = lastMovement;
        
        if (playerAttack.whichCombo < 3)
            MovePlayer(dashDirection, attackDashSpeed);
        if (playerAttack.whichCombo == 3)
            MovePlayer(dashDirection, attackDashSpeed * 1.5f);
    }

    private void Dash()
    {
        lastDashTimeStamp = Time.time;
        dashDirection = PlayerInput.ComputeMoveDirection();

        if (dashDirection == Vector2.zero)
            dashDirection = lastMovement;

        MovePlayer(dashDirection, dashSpeed);
        StartCoroutine(InstantiateRemnantImage());
        OnPlayerDash.Invoke();
    }

    private void SlowDown()
    {
        Vector2 moveVelocity = rb.velocity;

        moveVelocity.x = Mathf.MoveTowards(rb.velocity.x, 0.0f, decelerationSpeed * Time.fixedDeltaTime);
        moveVelocity.y = Mathf.MoveTowards(rb.velocity.y, 0.0f, decelerationSpeed * Time.fixedDeltaTime);

        rb.velocity = moveVelocity;
    }

    public void SelfKnockback()
    {
        Vector2 selfKnockbackDirection = dashDirection * -1.0f;

        rb.velocity = selfKnockbackDirection * selfKnockbackSpeed;
    }

    public void Stagger(Vector2 direction, float speed, float duration)
    {
        lastStaggerTimeStamp = Time.time + duration;

        lastMovement = direction * -1;
        rb.velocity = direction * speed;
        OnPlayerStagger.Invoke();
    }

    private IEnumerator InstantiateRemnantImage()
    {
        float timer = 0f;
        float timeIncrement = dashDuration / remnantImageCount;

        while (timer < dashDuration)
        {
            yield return new WaitForSeconds(timeIncrement);                 // Pour ne pas faire la première image rémanente
            Instantiate(GetRemnantImageDirection(), transform.position, Quaternion.identity);
            timer += timeIncrement;
        }
    }

    private GameObject GetRemnantImageDirection()
    {
        int direction = Tools.GetLastRecordedDirectionAsInt(LastMovement);
        return dashlist[direction];
    }

    #region CONDITIONCHECK
    public bool IsBusy()
    {
        return (IsDashing() || playerAttack.IsAttacking()) || IsStagger() || shield.IsShielded() || DialoguePanel.Instance.isReading;
    }

    public bool IsStagger()
    {
        return Time.time < lastStaggerTimeStamp;
    }

    public bool IsMoving()
    {
        return (rb.velocity.magnitude > 0 && IsBusy() == false);
    }

    public bool CanDash()
    {
        return Time.time - lastDashTimeStamp >= cooldownDash;
    }

    public bool IsDashing()
    {
        return Time.time > dashDuration && Time.time - lastDashTimeStamp < dashDuration;
    }

    #endregion
}