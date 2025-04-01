using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static Unity.Collections.AllocatorManager;

public class RangeEnemyController : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAttack = new UnityEvent();
    [HideInInspector] public UnityEvent OnAnticipationAttack = new UnityEvent();
    [HideInInspector] public UnityEvent OnParried = new UnityEvent();
    [HideInInspector] public UnityEvent OnSelfBump = new UnityEvent();

    public int groupID;
    public bool isPacific;

    public bool isDifficult;
    public bool isEasy;

    public GameObject projectilePrefab;

    [HideInInspector] public bool isAttacking;
    public float aggroRange;
    public float attackRange;
    public float kitingRangeMin;
    public float kitingRangeMax;
    public float attackCD;
    public float cannotMoveAfterAttackDuration;
    public float speed;
    public float bumpSpeed;
    public float attackDelay;
    public float staggerDuration;
    public float bumpDuration;
    public Vector2 offsetProjectileToDirection;
    public float offsetHeight;
    public Vector2 LastMovement => lastMovement;

    private Vector2 lastMovement = Vector2.down;
    protected Rigidbody2D rb;
    protected float lastAttackTimeStamp = -15;
    protected float lastStaggerTimeStamp;
    protected float lastBumpTimeStamp;
    protected bool isLocked;
    protected bool wasPacific;
    private bool isKitingRight;

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

        OnAttack.AddListener(() =>
        {
            if (Tools.RandomBool())
                isKitingRight = !isKitingRight;
        });

        if (isPacific == true)
            GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => EnemyController.OnTriggerAggression.Invoke(groupID));

        EnemyController.OnTriggerAggression.AddListener((_) => TriggerAggression(groupID));
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

        if (distance < aggroRange)
        {
            if (distance <= attackRange && CanAttack())
            {
                StartCoroutine(ThrowGenericProjectile());
            }
            else if (!isAttacking && CanKite())
            {
                MoveToKitingDistance();
            }
            else
                StopMove();
        }
        else
            StopMove();
    }

    #region ACTIONS
    protected IEnumerator ThrowGenericProjectile()
    {
        StopMove();
        isAttacking = true;
        lastAttackTimeStamp = Time.time;
        lastMovement = GetPlayerDirection();

        OnAnticipationAttack.Invoke();

        yield return new WaitForSeconds(attackDelay);
        
        OnAttack.Invoke();
        Vector2 spawnPosition = (Vector2)transform.position + GetPlayerDirection() * offsetProjectileToDirection + Vector2.up * offsetHeight;

        GameObject projectileTemp = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        GenericProjectile projectileComponent = projectileTemp.GetComponent<GenericProjectile>();

        if (isDifficult)
            projectileComponent.Setup(AnticipatePlayerDirection());
        else if (isEasy)
            projectileComponent.Setup(GetPlayerDirection() * Random.Range(0.8f, 1.2f));
        else
            projectileComponent.Setup(GetPlayerDirection());

        isAttacking = false;
    }

    protected virtual void CancelAttack()
    {
        if (isAttacking)
            StopAllCoroutines();

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

    private void MoveToKitingDistance()
    {
        Vector2 direction = GetPlayerDirection();
        float distance = ComputeDistanceToPlayer();

        if (distance < kitingRangeMin)
        {
            rb.velocity = -direction * speed;
            lastMovement = -direction;
        }

        if (distance > kitingRangeMax)
        {
            rb.velocity = direction * speed;
            lastMovement = direction;
        }

        if (kitingRangeMin <= distance && distance <= kitingRangeMax)
        {
            Vector2 perpendicularDirection = direction.AddAngleToDirection(isKitingRight ? 90 : -90);

            rb.velocity = perpendicularDirection * speed;
            lastMovement = perpendicularDirection;
        }
    }

    protected Vector2 GetPlayerDirection()
    {
        Vector3 position = MainCharacter.Instance.transform.position;
        return (position - transform.position).normalized;
    }

    protected Vector2 AnticipatePlayerDirection()
    {
        Vector3 playerPosition1 = MainCharacter.Instance.transform.position;

        float projectileFlyTime = (playerPosition1 - transform.position).magnitude / projectilePrefab.GetComponent<GenericProjectile>().speed;

        Vector3 playerDirection = MainCharacter.Instance.GetComponent<Rigidbody2D>().velocity * (/*attackDelay +*/ projectileFlyTime);

        Vector3 playerPosition2 = playerPosition1 + playerDirection;

        return (playerPosition2 - transform.position).normalized;
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
    private void Unlock()
    {
        isLocked = false;
        isPacific = wasPacific;
    }

    #endregion

    protected bool CanAttack()
    {
        return Time.time - lastAttackTimeStamp >= attackDelay + attackCD && isAttacking == false;
    }

    protected bool CanKite()
    {
        return Time.time - lastAttackTimeStamp > cannotMoveAfterAttackDuration;
    }

    protected bool IsBusy()
    {
        return IsStagger() || IsBumped();
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
