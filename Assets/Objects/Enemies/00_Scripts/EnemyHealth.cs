using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static ColliderDamage;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public UnityEvent<AttackType> OnTakeDamage = new UnityEvent<AttackType>();
    [HideInInspector] public UnityEvent OnDeath = new UnityEvent();
    [HideInInspector] public UnityEvent OnBump = new UnityEvent();

    public bool isInvincible;
    public bool cantRespawn;

    public float hp;
    public float temperatureDecreaseSpeed;

    [HideInInspector] public float maxHp;
    protected Vector3 startingPosition;

    protected Squeeze squeeze;
    protected EnemyController enemyController;
    protected SamuraiAttack playerAttack;
    protected float temperatureControl;

    protected virtual void Start()
    {
        squeeze = GetComponent<Squeeze>();
        enemyController = GetComponent<EnemyController>();
        playerAttack = MainCharacter.Instance.GetComponent<SamuraiAttack>();

        startingPosition = transform.position;
        maxHp = hp;

        PlayerHealth.OnPlayerRespawn.AddListener(Respawn);
    }

    protected void Update()
    {
        if (temperatureControl > 0)
            temperatureControl -= temperatureDecreaseSpeed * Time.deltaTime;
    }

    public void TakeDamage(float damage, int temperature, AttackType attackType)
    {
        if (hp <= 0)
            return;

        if (!isInvincible)
            hp -= damage;
       
        if (squeeze != null)
            squeeze.Trigger();
        
        if (hp <= 0)
        {
            StartCoroutine(Death());
            OnDeath.Invoke();
        }
        else
        {
            OnTakeDamage.Invoke(attackType);
            IncreaseBumpTemperature(temperature);
        }

    }

    protected void IncreaseBumpTemperature(int temperatureIncrease)
    {
        temperatureControl += temperatureIncrease;
        if (temperatureControl > 3)
        {
            OnBump.Invoke();
            temperatureControl = 0;
        }
    }

    protected IEnumerator Death()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    protected void Respawn()
    {
        if (cantRespawn == true)
            return;

        gameObject.SetActive(true);

        EnemyController enemyController = GetComponent<EnemyController>();
        RangeEnemyController rangeEnemyController = GetComponent<RangeEnemyController>();

        if (enemyController != null)
        {
            enemyController.ResetLastMovement();
        }

        if (rangeEnemyController != null)
        {
            rangeEnemyController.ResetLastMovement();
        }
    
        transform.position = startingPosition;
        hp = maxHp;
    }
}