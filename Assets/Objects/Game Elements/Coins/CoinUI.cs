using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    private TextMeshProUGUI textMeshCoinAmountUI;

    void Start()
    {
        textMeshCoinAmountUI = GetComponent<TextMeshProUGUI>();

        CoinManager.OnCoinAmountUpdate.AddListener(UpdateUI);
    }


    private void UpdateUI(int amount)
    {
        textMeshCoinAmountUI.text = $"x{amount}";
    }
}
