using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartHealthBar : MonoBehaviour
{
    public List<Image> heartList;

    public Sprite full;
    public Sprite empty;

    PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        PlayerHealth.OnPlayerTakeDamage.AddListener((_,_) => UpdateHeart());
        PlayerHealth.OnPlayerDeath.AddListener((_) => UpdateHeart());
        playerHealth.OnRestoreHealth.AddListener(UpdateHeart);
        PlayerHealth.OnPlayerRespawn.AddListener(UpdateHeart);
    }

    public void SetMaxHeart(float maxHearts)
    {
        int index = 0;
        while (index < heartList.Count)
        {
            if (index < maxHearts)
            {
                heartList[index].gameObject.SetActive(true);
                heartList[index].sprite = full;
            }
            else
            {
                heartList[index].gameObject.SetActive(false);
            }
            index++;
        }
    }

    public void UpdateHeart()
    {
        float maxHearts = playerHealth.maxHearts;
        float hearts = playerHealth.hearts;

        int index = 0;
        while (index < maxHearts)
        {
            if (index < hearts)
                heartList[index].sprite = full;
            else
                heartList[index].sprite = empty;
            index++;
        }
    }
}