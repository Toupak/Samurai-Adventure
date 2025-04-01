using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarUI : MonoBehaviour
{
    public Image fillBar;

    private void Awake()
    {
        fillBar.fillAmount = 1;

        SamuraiMana.OnManaUpdate.AddListener(UpdateManaBar);

        FireballItemLoot.OnLootFireball.AddListener(ActivateUI);
    }

    private IEnumerator Start()
    {
        yield return null;

        if (SaveManager.Instance.GetSaveData().hasLootedFireball == false)
            gameObject.SetActive(false);
    }

    private void UpdateManaBar(int currentMana, int maxMana)
    {
        fillBar.fillAmount = Tools.NormalizeValue(currentMana, 0, maxMana); 
    }

    private void ActivateUI()
    {
        gameObject.SetActive(true);
    }
}
