using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    public static UnityEvent<int> OnCoinAmountUpdate = new UnityEvent<int>();

    public static CoinManager Instance;

    private int coinAmount;

    void Start()
    {
        Instance = this;
    }

    public void AddCoinAmount(int coin)
    {
        coinAmount += coin;
        OnCoinAmountUpdate.Invoke(coinAmount);
    }

    public void RemoveCoinAmount(int coin)
    {
        coinAmount -= coin;
        OnCoinAmountUpdate.Invoke(coinAmount);
    }

    //+ Sauvegarde
}
