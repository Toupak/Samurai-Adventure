using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobProjectileExplosion : MonoBehaviour
{
    public ColliderDamage explosionColliderDamage;

    public AudioClip explosionSound;

    private float attackDelay = 0.1f;
    private float explosionDuration = 0.2f;

    private void Awake()
    {
        explosionColliderDamage = GetComponent<ColliderDamage>();

        StartCoroutine(Explode());
    }

    public IEnumerator Explode()
    {
        if (explosionColliderDamage == null)
            explosionColliderDamage = GetComponent<ColliderDamage>();

        PlayExplosionSound();

        yield return new WaitForSeconds(attackDelay);
        explosionColliderDamage.ActivateDamage();

        yield return new WaitForSeconds(explosionDuration);
        explosionColliderDamage.DeactivateDamage();

        Destroy(gameObject, 0.5f);
    }

    private void PlayExplosionSound()
    {
        SFXManager.Instance.PlaySFX(explosionSound, 0.05f);
    }
}
