using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class OLDEnemyController : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAttack = new UnityEvent();

    public Transform rotationPivotTransform;
    public Animator animator;
    public AudioSource attackSound;
    public ColliderDamage weaponColliderScript;
    protected Rigidbody2D rb;

    public float aggroRange;
    public float attackRange;
    public float speed;
    public float attackDelay;
    public float attackDuration;
    public float staggerDuration;
    public float attackSoundDelay;

    protected float lastAttackTimeStamp;
    protected float staggerTimeStamp;

    [HideInInspector] public bool isAttacking;
    protected bool isLocked;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GetComponent<EnemyHealth>().OnDeath.AddListener(Lock);
        GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => Stagger());
    }

    protected virtual void Update()
    {
        if (isLocked)
            return;

        if (isStagger())
            return;

        float distance = ComputeDistanceToPlayer();

        if (distance < aggroRange)
        {
            if (distance <= attackRange && CanAttack())
            {
                RotateTowardsPlayer();
                StartCoroutine(Attack());
            }
            else if (!isAttacking)
            {
                RotateTowardsPlayer();
                MoveToPlayer();
            }
        }
        else
            StopMove();
    }

    #region ACTIONS
    protected IEnumerator Attack()
    {
        StopMove();
        isAttacking = true;
        lastAttackTimeStamp = Time.time;
        OnAttack.Invoke();

        PlayAttack();

        yield return new WaitForSeconds(attackDelay);

        weaponColliderScript.ActivateDamage();

        yield return new WaitForSeconds(attackDuration - attackDelay);

        weaponColliderScript.DeactivateDamage();

        isAttacking = false;
    }

    protected virtual void CancelAttack()
    {
        StopAllCoroutines();
        if (attackSound.isPlaying == true)
            attackSound.Stop();

        if (weaponColliderScript != null)
            weaponColliderScript.DeactivateDamage();
        
        isAttacking = false;
    }

    
    #endregion

    #region MOVEMENT
    protected float ComputeDistanceToPlayer()
    {
        return Vector3.Distance(MainCharacter.Instance.transform.position, transform.position);
    }

    protected virtual void MoveToPlayer()
    {
        Vector3 positionRight = MainCharacter.Instance.transform.position + Vector3.right;
        Vector3 positionLeft = MainCharacter.Instance.transform.position + Vector3.left;
        Vector3 position = positionLeft;

        if (Vector3.Distance(positionRight, transform.position) < Vector3.Distance(positionLeft, transform.position))
            position = positionRight;

        Vector2 direction = (position - transform.position).normalized;
        rb.velocity = direction * speed;
        PlayMovement();
    }
    protected virtual void StopMove()
    {
        rb.velocity = Vector2.zero;
    }

    protected void RotateTowardsPlayer()
    {
        Vector2 direction = (MainCharacter.Instance.transform.position - transform.position).normalized;

        if (direction.x > 0)
            rotationPivotTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (direction.x < 0)
            rotationPivotTransform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    }
    protected void Stagger()
    {
        if (staggerDuration != 0)
        {
            staggerTimeStamp = Time.time;
            StopMove();
            if (isAttacking == true)
                CancelAttack();
        }
    }

    protected void Lock()
    {
        isLocked = true;
        StopMove();
    }

    #endregion

    #region ANIMATION/SOUND
    protected virtual void PlayAttack()
    {
        animator.Play("Attack");
        
        if (attackSound != null)
        {
            attackSound.pitch = Random.Range(0.95f, 1.05f);
            attackSound.PlayDelayed(attackSoundDelay);
        }
    }
    protected virtual void PlayMovement()
    {
        animator.Play("Walk");
    }

    protected void PlayDeath()
    {
        animator.Play("Death");
    }
    #endregion


    protected bool CanAttack()
    {
        return Time.time - lastAttackTimeStamp >= attackDuration && isAttacking == false;
    }

    protected bool isStagger()
    {
        return (Time.time - staggerTimeStamp < staggerDuration && staggerDuration !=0);
    }
}
