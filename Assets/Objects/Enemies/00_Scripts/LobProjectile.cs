using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobProjectile : GenericProjectile
{
    public GameObject dangerParticle1;
    public GameObject dangerParticle2;
    public GameObject dangerCircle;
    public GameObject explosionPrefab;

    public AudioClip launchProjectileSound;
    public AudioClip dangerParticlesSound;
    public AudioClip dangerElectricSound;
    public AudioClip landOnGroundSound;

    public Transform lobbedProjectileTransform;

    public float explosionRadius;

    public float spawnParticleDuration;
    public float spawnParticleInterval;
    public float spawnParticleRadius;
    public Vector3 offsetDangerCircle;

    private AudioSource dangerParticleSoundTemp;
    private AudioSource dangerElectricSoundTemp;

    public override void Setup(Vector2 direction)
    {
        base.Setup(direction);

        maxRange = ((Vector2)MainCharacter.Instance.transform.position - positionStart).magnitude;

        dangerElectricSoundTemp = SFXManager.Instance.PlaySFX(dangerElectricSound, volume: 0.02f, loop: true);

        StartCoroutine(DoLobVisualEffect());
    }

    protected override void Remove()
    {
        if (hasBeenRemoved)
            return;

        hasBeenRemoved = true;

        rb.velocity = Vector2.zero;
        SFXManager.Instance.PlaySFX(landOnGroundSound);

        StartCoroutine(SpawnDangerParticles());
    }

    private IEnumerator SpawnDangerParticles()
    {
        GameObject circleTemp = Instantiate(dangerCircle, transform.position + offsetDangerCircle, Quaternion.identity);
        circleTemp.transform.localScale = Vector3.one * explosionRadius;

        dangerParticleSoundTemp = SFXManager.Instance.PlaySFX(dangerParticlesSound, volume: 0.02f, loop: true);

        float durationIndex = 0;

        while (durationIndex < spawnParticleDuration)
        {
            Instantiate(dangerParticle1, ComputeExplosionLocation(), Quaternion.identity);
            Instantiate(dangerParticle2, ComputeExplosionLocation(), Quaternion.identity);
            yield return new WaitForSeconds(spawnParticleInterval);
            durationIndex += spawnParticleInterval;
        }

        Destroy(circleTemp.transform.gameObject);

        GameObject explosionTemp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        if (dangerElectricSoundTemp != null)
            Destroy(dangerElectricSoundTemp.gameObject);
        
        if (dangerParticleSoundTemp != null)
            Destroy(dangerParticleSoundTemp.gameObject);

        Destroy(transform.gameObject);
    }

    private Vector2 ComputeExplosionLocation()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * spawnParticleRadius;
    }

    private IEnumerator DoLobVisualEffect()
    {
        float y = lobbedProjectileTransform.localPosition.y;
        
        float currentDistance = ((Vector2)transform.position - positionStart).magnitude;

        while (currentDistance < maxRange / 2)
        {
            y = Tools.NormalizeValue(currentDistance, 0, maxRange/2) * maxRange;
            lobbedProjectileTransform.localPosition = new Vector3(0, y, 0);
            yield return null;
            currentDistance = ((Vector2)transform.position - positionStart).magnitude;
        }

        while (currentDistance <= maxRange)
        {
            y = maxRange - Tools.NormalizeValue(currentDistance, maxRange/2, maxRange) * maxRange;
            lobbedProjectileTransform.localPosition = new Vector3(0, y, 0);
            yield return null;
            currentDistance = ((Vector2)transform.position - positionStart).magnitude;
        }
    }
}
