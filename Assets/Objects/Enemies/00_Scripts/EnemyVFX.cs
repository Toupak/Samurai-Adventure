using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVFX : MonoBehaviour
{
    public GameObject knockbackParticlePrefab;
    public GameObject deathParticle1Prefab;
    public GameObject skullDeathParticlePrefab;
    public GameObject bloodstainDeathParticlePrefab;

    public Vector2 offset;

    protected EnemyController enemyController;
    protected EnemyHealth enemyHealth;

    protected virtual void Start()
    {
        enemyController = GetComponent<EnemyController>();
        enemyHealth = GetComponent<EnemyHealth>();

        enemyController.OnSelfBump.AddListener(() => StartCoroutine(PlaySelfBumpArrivalParticle()));
        enemyHealth.OnDeath.AddListener(PlayDeathParticles);
    }
    
    protected IEnumerator PlaySelfBumpArrivalParticle()
    {
        yield return new WaitWhile(() => enemyController.IsBumped());
        Instantiate(knockbackParticlePrefab, transform.position, Quaternion.identity);
    }

    protected void PlayDeathParticles()
    {
        Vector2 SpawnPositionWithOffset = (Vector2)transform.position + offset;

        Instantiate(deathParticle1Prefab, SpawnPositionWithOffset, Quaternion.identity);
        Instantiate(bloodstainDeathParticlePrefab, transform.position, Quaternion.identity);

        GameObject SkullDeathParticle = Instantiate(skullDeathParticlePrefab, SpawnPositionWithOffset, Quaternion.identity);

        if (enemyController.LastMovement.x < 0)
        {   
            Vector3 size = SkullDeathParticle.transform.localScale;
            SkullDeathParticle.transform.localScale = new Vector3(size.x * -1f, size.y, size.z);
        }
    }
}
