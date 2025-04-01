using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SkullLootable : MonoBehaviour
{
    public List<GameObject> lootPrefabs;
    public GameObject destroyParticle;

    public AudioClip destroySound;

    public float maxDirectionLength;
    public float maxLateralDirectionLength;

    private float spawnTimeStamp;
    private bool isActivated;

    void Start()
    {
        spawnTimeStamp = Time.time;
    }


    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("PlayerAttack") && CanBeHit())
        {
            TakeDamageAndLoot();
            isActivated = true;
        }
    }

    private void TakeDamageAndLoot() 
    {
        DestroyAnimation();

        GameObject lootTemp = ComputeWhichLoot();
        CoinLoot lootComponent = null;

        if (lootTemp != null)
        {
            lootTemp = Instantiate(lootTemp, transform.position, Quaternion.identity);
            lootComponent = lootTemp.GetComponent<CoinLoot>();
        }
       
        if (lootComponent != null)
        {
            lootComponent.IsThrownInDirection(ComputeLootPosition());
        }
    }

    private Vector2 ComputeLootPosition()
    {
        Vector2 startingPosition = transform.position;
        Vector2 playerPosition = MainCharacter.Instance.transform.position;
        Vector2 direction = (startingPosition - playerPosition).normalized;

        return Tools.AddRandomAngleToDirection(direction, -30f, 30f);
    }

    private GameObject ComputeWhichLoot()
    {
        int whichLoot = Random.Range(0, 10);

        if (whichLoot <= 5)
            return lootPrefabs[0];
        else if (whichLoot > 5 && whichLoot <= 7)
            return lootPrefabs[1];
        else if (whichLoot > 7 && whichLoot <= 9)
            return lootPrefabs[2];
        else
            return null;
    }

    private void DestroyAnimation()
    {
        SFXManager.Instance.PlaySFX(destroySound);
        Instantiate(destroyParticle, transform.position, Quaternion.identity);

        Destroy(gameObject, 0.2f);
    }

    private bool CanBeHit()
    {
        return !isActivated && Time.time - spawnTimeStamp >= 0.5f;
    }
}
