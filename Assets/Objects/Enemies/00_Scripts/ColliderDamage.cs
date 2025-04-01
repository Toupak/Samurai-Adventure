using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ColliderDamage : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnDealDamage = new UnityEvent();

    public enum AttackType
    {
        Sword,
        Magic,
        EnemyProjectile
    };   
    
    public int damage;
    public int temperature;
    public float timeBetweenHits;
    
    public AttackType attackType;

    [HideInInspector] public List<GameObject> enemyHit = new List<GameObject>();

    private float hitTimeStamp;
    private bool hasHit => enemyHit.Count > 0;
    private bool isActivated;

    Collider2D damageAreaCollider;

    private void Start()
    {
        if (damageAreaCollider != null)
            return;
        
        damageAreaCollider = GetComponent<Collider2D>();
        damageAreaCollider.enabled = false;
    }
   
    private void Update()
    {
        if (timeBetweenHits == 0)
            return;

        if (hasHit && Time.time - hitTimeStamp > timeBetweenHits)
            ResetDamage();
    }
    
    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (enemyHit.Contains(otherCollider.gameObject))
            return;

        if (otherCollider.transform.CompareTag("Player") && isActivated == true)
        {
            PlayerHealth playerHealth = MainCharacter.Instance.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                bool hasTakenDamage = playerHealth.TakeDamage(damage, otherCollider.ClosestPoint(transform.position), ComputePlayerDirection(playerHealth));
                
                if (hasTakenDamage == true)
                {
                    MarkAsHit(otherCollider.gameObject);
                    OnDealDamage.Invoke();
                }
            }
        }

        if (otherCollider.transform.CompareTag("Enemy") && isActivated == true)
        {
            EnemyHealth enemyHealth = otherCollider.attachedRigidbody.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
                enemyHealth.TakeDamage(damage, temperature, attackType);

            MarkAsHit(otherCollider.gameObject);
            SamuraiAttack.OnHitEnemy.Invoke(otherCollider.ClosestPoint(transform.position), attackType);
        }

        if (otherCollider.transform.CompareTag("Dummy") && isActivated == true)
        {
            
            DKTrainingDummy dkTrainingDummy = otherCollider.transform.GetComponent<DKTrainingDummy>();
            
            if (dkTrainingDummy != null)
                dkTrainingDummy.DummyTakeDamage();

            SamuraiAttack.OnHitEnemy.Invoke(otherCollider.ClosestPoint(transform.position), attackType);
            MarkAsHit(otherCollider.gameObject);
        }
    }

    public void ResetDamage()
    {
        enemyHit = new List<GameObject>();
    }

    public void ActivateDamage()
    {
        if (damageAreaCollider == null)
            damageAreaCollider = GetComponent<Collider2D>();
        
        ResetDamage();
        isActivated = true;
        damageAreaCollider.enabled = true;
    }

    public void DeactivateDamage()
    {
        isActivated = false;

        if (damageAreaCollider != null)
            damageAreaCollider.enabled = false;
    }
    private void MarkAsHit(GameObject enemy)
    {
        enemyHit.Add(enemy);
        hitTimeStamp = Time.time;
    }

    private Vector2 ComputePlayerDirection(PlayerHealth playerHealth)
    {
        if (transform.parent != null) 
            return (playerHealth.transform.position - transform.parent.position).normalized;
        else
            return (playerHealth.transform.position - transform.position).normalized;
    }
}
