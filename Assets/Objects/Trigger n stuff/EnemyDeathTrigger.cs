using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyDeathTrigger : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnTrigger = new UnityEvent();

    public List<EnemyHealth> enemyList;

    private int countToNextLevel;

    void Start()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].OnDeath.AddListener(CountToNextLevel);
        }

        PlayerHealth.OnPlayerRespawn.AddListener(ResetEnemyCount);
    }

    private void CountToNextLevel()
    {
        countToNextLevel += 1;
        TriggerEvent();
    }

    private void ResetEnemyCount()
    {
        countToNextLevel = 0;
    }

    private void TriggerEvent()
    {
        if (countToNextLevel == enemyList.Count)
            OnTrigger.Invoke();
    }
}
