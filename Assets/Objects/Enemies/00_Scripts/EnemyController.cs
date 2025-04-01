using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;


public class EnemyController : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAttack = new UnityEvent();
    [HideInInspector] public UnityEvent OnAnticipationAttack = new UnityEvent();
    [HideInInspector] public UnityEvent OnSelfBump = new UnityEvent();
    [HideInInspector] public static UnityEvent<int> OnTriggerAggression = new UnityEvent<int>();
    
    public int groupID;
    public bool isPacific; 

    public bool isPatrolling;
    public List<Transform> patrolPoints;
    public float delayBeforeBouncing;

    public List<ColliderDamage> weaponColliders;

    [HideInInspector] public bool isAttacking;
    
    public float aggroRange;
    public float attackRange;
    public float attackCD;
    public float speed;
    public float bumpSpeed;
    public float attackDelay;
    public float attackDuration;
    public float attackActivationWindow;
    public float staggerDuration;
    public float bumpDuration;
    public float cannotMoveAfterAttackDuration;
    public Vector2 LastMovement => lastMovement;

    protected Vector2 lastMovement = Vector2.down;
    protected Rigidbody2D rb;
    protected float lastAttackTimeStamp = -1f;
    protected float lastStaggerTimeStamp;
    protected float lastBumpTimeStamp;
    protected bool isLocked;
    protected bool wasPacific;
    protected bool patrolActivated;
    protected Coroutine patrolCoroutine;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        wasPacific = isPacific;
        
        GetComponent<EnemyHealth>().OnDeath.AddListener(Lock);
        GetComponent<EnemyHealth>().OnBump.AddListener(() => 
        {
            Stagger();
            SelfBump(bumpSpeed);
        });

        PlayerHealth.OnPlayerDeath.AddListener((_) => Lock());
        PlayerHealth.OnPlayerRespawn.AddListener(Unlock);

        if (isPacific == true)
            GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => OnTriggerAggression.Invoke(groupID));

        OnTriggerAggression.AddListener((_) => TriggerAggression(groupID));

        if (isPatrolling == true)
            patrolCoroutine = StartCoroutine(Patrol());
    }

    protected virtual void Update()
    {
        if (isLocked)
            return;

        if (isPacific)
            return;

        if (IsBusy())
            return;

        float distance = ComputeDistanceToPlayer();

        if (isPatrolling == true && distance < aggroRange)
            StopPatrol();

        if (isPatrolling == true)
            return;

        if (distance < aggroRange && CanMove())
        {
            if (distance <= attackRange && CanAttack())
            {
                StartCoroutine(Attack());
            }
            else if (distance > attackRange && !isAttacking)
            {
                MoveToPlayer();
            }
            else
                StopMove();
        }
        else
            StopMove();
    }
            
    #region ACTIONS
    protected virtual IEnumerator Attack()
    {
        StopMove();
        isAttacking = true;
        lastMovement = GetPlayerDirection();
        int directionIndex = Tools.GetLastRecordedDirectionAsInt(lastMovement);
        lastAttackTimeStamp = Time.time;

        OnAnticipationAttack.Invoke();
        yield return new WaitForSeconds(attackDelay);
        OnAttack.Invoke();
        //Charge()
        yield return new WaitForSeconds(attackActivationWindow);
        weaponColliders[directionIndex].ActivateDamage();
        
        yield return new WaitForSeconds(attackActivationWindow);
        weaponColliders[directionIndex].DeactivateDamage();
        
        yield return new WaitForSeconds(attackDuration - attackActivationWindow * 2);
        isAttacking = false;
    }

    protected virtual void CancelAttack()
    {
        StopAllCoroutines();

        if (weaponColliders != null)
        {
            int directionIndex = Tools.GetLastRecordedDirectionAsInt(lastMovement);
            weaponColliders[directionIndex].DeactivateDamage();
        }
            
        isAttacking = false;
    }

    private void TriggerAggression(int currentGroupID)
    {
        if (groupID == 0)
            return;

        if (groupID == currentGroupID)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            isPacific = false;
        }
    }
    private IEnumerator Patrol()
    {
        if (patrolActivated == true)
            yield break;

        patrolActivated = true;

        int currentPoint = 0;
        bool isGoingForward = true;

        while (isPatrolling == true)
        {
            if (isGoingForward && currentPoint >= patrolPoints.Count - 1)
            {
                isGoingForward = false;
                yield return new WaitForSeconds(delayBeforeBouncing);
            }
            else if (!isGoingForward && currentPoint <= 0)
            {
                isGoingForward = true;
                yield return new WaitForSeconds(delayBeforeBouncing);
            }

            int nextPoint = isGoingForward ? currentPoint + 1 : currentPoint - 1;
            Vector2 nextPosition = patrolPoints[nextPoint].position;

            while (Vector2.Distance(nextPosition, transform.position) > 0.05f)
            {
                Vector2 direction = (nextPosition - (Vector2)transform.position).normalized;
                rb.velocity = direction * speed;
                lastMovement = direction;
                yield return null;
            }

            rb.velocity = Vector2.zero;
            transform.position = nextPosition;

            currentPoint = nextPoint;
        }

        patrolActivated = false;
    }

    private void StopPatrol()
    {
        isPatrolling = false;
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolActivated = false;
            patrolCoroutine = null;
        }
    }
    public void ResetLastMovement()
    {
        lastMovement = Vector2.down;
    }

    #endregion

    #region MOVEMENT
    protected float ComputeDistanceToPlayer()
    {
        return Vector3.Distance(MainCharacter.Instance.transform.position, transform.position);
    }

    protected virtual void MoveToPlayer()
    {
        Vector2 direction = GetPlayerDirection();

        rb.velocity = direction * speed;
        lastMovement = direction;
    }

    protected Vector2 GetPlayerDirection()
    {
        Vector3 position = MainCharacter.Instance.transform.position;
        return (position - transform.position).normalized;
    }

    protected virtual void StopMove()
    {
        rb.velocity = Vector2.zero;
    }

    protected void Stagger()
    {
        if (staggerDuration != 0)
        {
            lastStaggerTimeStamp = Time.time;
            StopMove();
            if (isAttacking == true)
                CancelAttack();
        }
    }

    public void SelfBump(float bumpSpeed)
    {
        StopMove();
        lastBumpTimeStamp = Time.time;

        if (isAttacking == true)
            CancelAttack();

        OnSelfBump.Invoke();
        rb.velocity = (transform.position - MainCharacter.Instance.transform.position).normalized * bumpSpeed;
    }


    protected void Lock()
    {
        isLocked = true;
        StopMove();

        if (isAttacking == true)
            CancelAttack();
    }

    protected void Unlock()
    {
        isLocked = false;
        isPacific = wasPacific;
    }

    #endregion

    protected bool CanAttack()
    {
        return Time.time - lastAttackTimeStamp >= attackDuration + attackCD && isAttacking == false;
    }

    protected bool IsBusy()
    {
        return IsStagger() || IsBumped();
    }

    private bool CanMove()
    {
        return Time.time - lastAttackTimeStamp > cannotMoveAfterAttackDuration;
    }

    public bool IsStagger()
    {
        return (Time.time - lastStaggerTimeStamp < staggerDuration && staggerDuration != 0);
    }

    public bool IsBumped()
    {
        return (Time.time - lastBumpTimeStamp < bumpDuration);
    }
}
