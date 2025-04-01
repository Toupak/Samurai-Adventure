using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeBoss : MonoBehaviour
{
    public Transform graphicsTransform;

    public float spawnOffset;
    public int minBossHP;

    private GameObject slimeBossPrefab;
    private EnemyHealth slimeHP;
    private int splitCount;

    private void Start()
    {
        slimeHP = GetComponent<EnemyHealth>();
        slimeHP.OnDeath.AddListener(Split);

        slimeBossPrefab = Resources.Load<GameObject>("SlimeBoss");
    }

    private void Split()
    {
        float newHP = slimeHP.maxHp / 2;

        if (newHP < minBossHP)
            return;

        int newSplitCount = splitCount + 1;
        Vector3 newLocalScale = (transform.localScale / 2);

        Vector2 ComputeSplitDirection = Random.insideUnitCircle;

        GameObject slimeTemp1 = Instantiate(slimeBossPrefab, (Vector2)transform.position + ComputeSplitDirection * spawnOffset, Quaternion.identity);
        Setup(slimeTemp1, newHP, newSplitCount);

        GameObject slimeTemp2 = Instantiate(slimeBossPrefab, (Vector2)transform.position  - ComputeSplitDirection * spawnOffset, Quaternion.identity);
        Setup(slimeTemp2, newHP, newSplitCount);
    }

    private void Setup(GameObject newSlime, float hp, int split)
    {
        EnemyHealth enemyHealth = newSlime.GetComponent<EnemyHealth>();
        enemyHealth.maxHp = hp;
        enemyHealth.hp = hp;

        newSlime.transform.localScale /= split + 1;

        SlimeBoss slimeBossScript = newSlime.GetComponent<SlimeBoss>();
        slimeBossScript.splitCount = split;
    }
}
