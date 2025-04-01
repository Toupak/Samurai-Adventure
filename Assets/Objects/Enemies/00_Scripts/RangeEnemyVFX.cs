using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemyVFX : MonoBehaviour
{
    public GameObject knockbackParticlePrefab;
    public GameObject deathParticle1Prefab;
    public GameObject skullDeathParticlePrefab;
    public GameObject bloodstainDeathParticlePrefab;

    public Vector2 offset;

    private RangeEnemyController rangeEnemyController;
    private EnemyHealth enemyHealth;

    void Start()
    {
        rangeEnemyController = GetComponent<RangeEnemyController>();
        enemyHealth = GetComponent<EnemyHealth>();

        rangeEnemyController.OnSelfBump.AddListener(() => StartCoroutine(PlaySelfBumpArrivalParticle()));
        enemyHealth.OnDeath.AddListener(PlayDeathParticles);
    }

    private IEnumerator PlaySelfBumpArrivalParticle()
    {
        yield return new WaitWhile(() => rangeEnemyController.IsBumped());
        Instantiate(knockbackParticlePrefab, transform.position, Quaternion.identity);
    }

    private void PlayDeathParticles()
    {
        Vector2 SpawnPositionWithOffset = (Vector2)transform.position + offset;

        Instantiate(deathParticle1Prefab, SpawnPositionWithOffset, Quaternion.identity);
        Instantiate(bloodstainDeathParticlePrefab, transform.position, Quaternion.identity);

        GameObject SkullDeathParticle = Instantiate(skullDeathParticlePrefab, SpawnPositionWithOffset, Quaternion.identity);

        if (rangeEnemyController.LastMovement.x < 0)
        {
            Vector3 size = SkullDeathParticle.transform.localScale;
            SkullDeathParticle.transform.localScale = new Vector3(size.x * -1f, size.y, size.z);
        }
    }
}
