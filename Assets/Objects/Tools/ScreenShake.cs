using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScreenShake : MonoBehaviour
{
    CinemachineImpulseSource cinemachineImpulseSource;
    SamuraiMovement movement;
    //EnemyController enemyController;

    public float forceTemp;

    void Start()
    {
        //enemyController = GetComponent<EnemyController>();
        movement = MainCharacter.Instance.GetComponent<SamuraiMovement>();
        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();

        SamuraiAttack.OnHitEnemy.AddListener((spawnPosition,_) => Trigger());
        Teleporter.OnTakeTeleporter.AddListener(Trigger);
        PlayerHealth.OnPlayerTakeDamage.AddListener((_,_) => Trigger());
        PlayerHealth.OnPlayerDeath.AddListener((_) => Trigger());

        //enemyController.OnParried.AddListener(Trigger);
    }

    void Trigger()
    {
        cinemachineImpulseSource.GenerateImpulseWithVelocity(movement.LastMovement * forceTemp);
    }

    // mettre 3 niveaux de screenshake différent : heavy / normal / low /// expérimenter via public float forceTemp.
    // En mettre pas forcément selon le lastMovement i.e. pour OnTakeDamage.
}
