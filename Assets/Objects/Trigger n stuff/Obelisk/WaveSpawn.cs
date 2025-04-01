using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyType
{
    Ogre,
    Dryad,
    Wyrm,
    Worm
}


[Serializable]
public class WaveData
{
    public float spawnInterval;
    public Transform where;
    public EnemyType enemy;
    public int maxCount;
}


public class WaveSpawn : MonoBehaviour
{
    public List<WaveData> waveData;

    public GameObject ogrePrefab;
    public GameObject dryadPrefab;
    public GameObject wyrmPrefab;
    public GameObject wormPrefab;

    public GameObject triggerObject;
    public GameObject stopTriggerObject;

    private bool isActivated;

    private Dictionary<EnemyType, int> waveSpawnDict;
    private List<EnemyHealth> enemiesSpawned = new List<EnemyHealth>();

    void Start()
    {
        triggerObject.GetComponent<Trigger>().OnTrigger.AddListener(StartWaveCoroutines);
        stopTriggerObject.GetComponent<Trigger>().OnTrigger.AddListener(StopSpawnEnemies);
    
        waveSpawnDict = new Dictionary<EnemyType, int>();
        foreach (WaveData data in waveData)
        {
            if (waveSpawnDict.ContainsKey(data.enemy) == false)
                waveSpawnDict.Add(data.enemy, 0);
        }
    }

    private void StartWaveCoroutines()
    {
        isActivated = true;

        foreach (WaveData data in waveData)
        {
            StartCoroutine(SpawnWave(data));
        }
    }

    private IEnumerator SpawnWave(WaveData data)
    {
        while (isActivated == true)
        {
            if (waveSpawnDict[data.enemy] < data.maxCount)
            {
                GameObject enemySpawned = Instantiate(GetEnemyOnType(data.enemy), data.where.position, Quaternion.identity);
                enemySpawned.GetComponent<EnemyHealth>().OnDeath.AddListener(() => DecreaseCount(data.enemy));
                enemiesSpawned.Add(enemySpawned.GetComponent<EnemyHealth>());
                waveSpawnDict[data.enemy] += 1;
            }
            yield return new WaitForSeconds(data.spawnInterval);
        }
    }

    private void DecreaseCount(EnemyType enemyType)
    {
        waveSpawnDict[enemyType] -= 1;
    }

    private void StopSpawnEnemies()
    {
        isActivated = false;
        KillAllEnemies();
    }

    private void KillAllEnemies()
    {
        foreach(EnemyHealth enemy in enemiesSpawned)
        {
            enemy.TakeDamage(enemy.maxHp, 0, ColliderDamage.AttackType.EnemyProjectile);
        }
    }

    private GameObject GetEnemyOnType(EnemyType enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Ogre:
                return ogrePrefab;
            case EnemyType.Dryad:
                return dryadPrefab;
            case EnemyType.Wyrm:
                return wyrmPrefab;
            case EnemyType.Worm:
                return wormPrefab;
            default: 
                return null;
        }
    }
}