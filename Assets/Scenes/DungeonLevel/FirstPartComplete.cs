using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FirstPartComplete : MonoBehaviour
{
    public List<EnemyHealth> ennemies;
    private int count;
    private int startingEnemyCount;

    public Collider2D bridgeCollider;
    public GameObject bridge;

    void Start()
    {
        startingEnemyCount = ennemies.Count;

        foreach(EnemyHealth enemy in ennemies)
        {
            enemy.OnDeath.AddListener(OnEnemyDeath);
        }

        bridge.SetActive(false);
    }

    private void OnEnemyDeath()
    {
        count += 1;

        if (count == startingEnemyCount)
        {
            CompleteLevel();
        }
    }

    private void CompleteLevel()
    {
        bridgeCollider.enabled = false;
        bridge.SetActive(true);
        // Spawn récompenses ? -> un coeur ?
        // petit son de Zelda ?
    }
}
