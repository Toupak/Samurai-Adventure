using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandWormVFX : EnemyVFX
{
    /*
    public GameObject chargingTowardsPlayerParticlePrefab;

    private SandWormController sandWormController;

    protected override void Start()
    {
        base.Start();

        sandWormController = GetComponent<SandWormController>();

        sandWormController.OnStartCharge.AddListener(() => StartCoroutine(PlayChargingTowardsPlayerParticle()));
    }

    private IEnumerator PlayChargingTowardsPlayerParticle()
    {
        while (sandWormController.isAttacking)
        {
            Instantiate(chargingTowardsPlayerParticlePrefab, transform.position, Quaternion.identity); //Rotate la particule a
            yield return new WaitForSeconds(0.1f);
        }
    }
    */
}
