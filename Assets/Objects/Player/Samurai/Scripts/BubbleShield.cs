using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class BubbleShield : MonoBehaviour
{
    public static UnityEvent OnPlayerShield = new UnityEvent();
    public static UnityEvent<Vector2, Vector2> OnPlayerShieldDamage = new UnityEvent<Vector2, Vector2>();
    public static UnityEvent<Vector2, Vector2> OnPlayerShieldDestroy = new UnityEvent<Vector2, Vector2>();
    public static UnityEvent OnPlayerShieldRelease = new UnityEvent();
    public static UnityEvent<Vector2> OnPlayerParry = new UnityEvent<Vector2>();

    public GameObject shieldPrefab;
    [HideInInspector] public Animator currentShield; // potentiellement à remettre privé

    public float parryWindow;
    public float parryBumpRange;
    public float parryBumpSpeed;
    public float shieldCD;
    public Vector2 spawnOffset;

    public bool hasLootedShield;

    private SamuraiMovement movement;
    private PlayerHealth playerHealth;

    private float lastShieldCastTimeStamp = -10f;
    private int hp;
    private float parryTimeStamp;
    private bool hasParried;

    private void Start()
    {
        movement = MainCharacter.Instance.GetComponent<SamuraiMovement>();
        playerHealth = GetComponent<PlayerHealth>();

        hasLootedShield = SaveManager.Instance.GetSaveData().hasLootedShield;
    }

    private void Update()
    {
        if (playerHealth.IsDead)
            return;
        
        if (CanShield() && PlayerInput.GetShieldInput())
            ShieldUp();
        else if (IsShielded() == true && PlayerInput.GetRemoveShieldInput() == false)
            Release();

        if (currentShield != null)
            FollowPlayer();
    }

    public bool TakeDamage(int damage, Vector2 position, Vector2 direction)
    {
        if (Time.time - parryTimeStamp < parryWindow && hasParried == false)
        {
            Parry(position);
            return false;
        }

        hp -= damage;
        currentShield.GetComponent<Squeeze>().Trigger();

        if (hp == 1)
            DamagedShield();

        if (hp <= 0)
        {
            OnPlayerShieldDestroy.Invoke(position, direction);
            Break(direction);
        }
        else
            OnPlayerShieldDamage.Invoke(position, direction);

        return true;
    }

    private void ShieldUp()
    {
        OnPlayerShield.Invoke();
        parryTimeStamp = Time.time;
        lastShieldCastTimeStamp = Time.time;
        hasParried = false;
        movement.StopMovement();

        hp = 2;
        currentShield = Instantiate(shieldPrefab, transform.position + (Vector3)spawnOffset, Quaternion.identity).GetComponent<Animator>();
    }

    public void Break(Vector2 direction)
    {
        currentShield.Play("ShieldDestroy");
        Destroy(currentShield.gameObject, 0.5f);
        currentShield = null;
    }

    public void Release()
    {
        OnPlayerShieldRelease.Invoke();

        currentShield.Play("ShieldRelease");
        Destroy(currentShield.gameObject, 0.5f);
        currentShield = null;
    }

    private void DamagedShield()
    {
        currentShield.Play("ShieldDamaged");
    }

    private void Parry(Vector2 position)
    {
        OnPlayerParry.Invoke(position);
        hasParried = true;

        //Flash lumineux puis bump
        currentShield.GetComponent<Squeeze>().Trigger();

        RaycastHit2D[] enemiesInParryRange = Physics2D.CircleCastAll(transform.position + (Vector3)spawnOffset, parryBumpRange, Vector2.zero, 0, LayerMask.GetMask("EnemyHurtbox"));

        foreach (RaycastHit2D enemy in enemiesInParryRange)
            BumpEnemy(enemy.rigidbody.gameObject);

        RaycastHit2D[] projectilesInParryRange = Physics2D.CircleCastAll(transform.position + (Vector3)spawnOffset, parryBumpRange, Vector2.zero, 0, LayerMask.GetMask("EnemyAttack"));

        foreach (RaycastHit2D projectile in projectilesInParryRange)
        {
            if (projectile.rigidbody != null)
                ReflectProjectile(projectile.rigidbody.gameObject);
        }
    }

    private void ReflectProjectile(GameObject projectile)
    {
        GenericProjectile projectileScript = projectile.GetComponent<GenericProjectile>();
        ColliderDamage projectileCollider = projectile.GetComponent<ColliderDamage>();

        if (projectileScript == null)
            return;

        Vector2 newDirection = (projectileScript.positionStart - (Vector2)transform.position).normalized;

        projectileScript.rb.velocity = newDirection * projectileScript.speed;
        projectile.transform.localRotation = newDirection.ToRotation();
        projectile.layer = LayerMask.NameToLayer("PlayerAttack");
        projectileCollider.damage = projectileCollider.damage * 2;
    }

    private void FollowPlayer()
    {
        currentShield.transform.position = MainCharacter.Instance.transform.position + (Vector3)spawnOffset;
    }

    private void BumpEnemy(GameObject enemy)
    {
        EnemyController enemyController = enemy.GetComponent<EnemyController>();
        RangeEnemyController rangeEnemyController = enemy.GetComponent<RangeEnemyController>();

        if (enemyController != null)
            enemyController.SelfBump(parryBumpSpeed);

        if (rangeEnemyController != null)
            rangeEnemyController.SelfBump(parryBumpSpeed);
    }

    public bool IsShielded()
    {
        if (currentShield != null)
            return true;

        return false;
    }

    public bool CanShield()
    {
        return hasLootedShield && Time.time - lastShieldCastTimeStamp > shieldCD && movement.IsBusy() == false;
    }

}
