using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamuraiVFX : MonoBehaviour
{
    public GameObject onHitEnemyParticlePrefab;
    public GameObject groundBloodParticlePrefab;
    public GameObject bloodParticlePrefab;
    public GameObject dashParticlePrefab;
    public GameObject playerTakeDamagePrefab;
    public GameObject parryPrefab;
    public GameObject parry2Prefab;
    public Vector2 parryParticleSpawnPositionOffset;
    public GameObject shieldDamageParticle;
    public List<GameObject> onHitEnemyBloodParticles;

    private SamuraiAttack samuraiAttack;
    private PlayerHealth playerHealth;

    void Start()
    {
        samuraiAttack = GetComponent<SamuraiAttack>();
        playerHealth = GetComponent<PlayerHealth>();

        SamuraiAttack.OnHitEnemy.AddListener(SpawnOnHitParticle);
        PlayerHealth.OnPlayerTakeDamage.AddListener((p,_) => SpawnOnTakeDamageParticle(p));
        PlayerHealth.OnPlayerLowHP.AddListener(() => StartCoroutine(SpawnGroundBlood()));
        BubbleShield.OnPlayerShieldDamage.AddListener((p,_) => SpawnShieldDamageParticle(p));
        BubbleShield.OnPlayerShieldDestroy.AddListener((p,_) => SpawnShieldDamageParticle(p));
        SamuraiMovement.OnPlayerDash.AddListener(() => StartCoroutine(SpawnDashParticle()));
        BubbleShield.OnPlayerParry.AddListener(SpawnOnParryParticle);
    }
    
    
    //Spawn différents types de particules selon l'attaque ligne 37
    private void SpawnOnHitParticle(Vector2 spawnPosition, ColliderDamage.AttackType attackType)
    {
        Instantiate(onHitEnemyParticlePrefab, spawnPosition, Quaternion.identity);

        int randomIndex = Random.Range(0, onHitEnemyBloodParticles.Count);
        Instantiate(onHitEnemyBloodParticles[randomIndex], spawnPosition, Quaternion.identity);

        if (samuraiAttack.whichCombo == 3)
            Instantiate(groundBloodParticlePrefab, spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnDashParticle()
    {
        Instantiate(dashParticlePrefab, transform.position, Quaternion.identity);
        yield return new WaitWhile(() => MainCharacter.Instance.GetComponent<SamuraiMovement>().IsDashing());
        Instantiate(dashParticlePrefab, transform.position, Quaternion.identity);
    }

    private void SpawnOnTakeDamageParticle(Vector2 spawnPosition)
    {
        Instantiate(playerTakeDamagePrefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnOnParryParticle(Vector2 spawnPosition)
    {
        Instantiate(parryPrefab, transform.position + (Vector3)parryParticleSpawnPositionOffset, Quaternion.identity);
        Instantiate(parry2Prefab, spawnPosition, Quaternion.identity);
    }

    private void SpawnShieldDamageParticle(Vector2 spawnPosition)
    {
        Instantiate(shieldDamageParticle, spawnPosition, Quaternion.identity);
    }

    private IEnumerator SpawnGroundBlood()
    {
        while (playerHealth.hearts == 1)
        {
            Instantiate(bloodParticlePrefab, transform.position + (Vector3)parryParticleSpawnPositionOffset, Quaternion.identity);
            Instantiate(groundBloodParticlePrefab, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(3.0f);
        }
    }
}
