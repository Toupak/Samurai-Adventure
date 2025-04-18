using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinUI : MonoBehaviour
{
    

    void Start()
    {
        CoinManager.OnCoinAmountUpdate.AddListener(UpdateUI);
    }


    private void UpdateUI(int amount)
    {

    }
}
