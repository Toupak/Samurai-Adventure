using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpawnerOnTrigger : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPosition;
    public Trigger trigger;

    private void Start()
    {
        trigger.OnTrigger.AddListener(SpawnEnemy);
    }

    private void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
    }
}
