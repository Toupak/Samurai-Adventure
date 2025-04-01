using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenParryZone : MonoBehaviour
{
    public List<EnemyHealth> enemyList;

    public GameObject roadBlock;
    public AudioClip success;

    private int countToNextLevel;

    void Start()
    {
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].OnDeath.AddListener(CountToNextLevel);
        }
    }

    private void CountToNextLevel()
    {
        countToNextLevel += 1;
        OpenNextLevel();
    }

    private void OpenNextLevel()
    {
        if (countToNextLevel == enemyList.Count)
        {
            roadBlock.SetActive(false);
            SFXManager.Instance.PlaySFX(success);
        }
    }
}
