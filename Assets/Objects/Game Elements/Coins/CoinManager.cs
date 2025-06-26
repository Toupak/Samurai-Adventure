using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoinManager : MonoBehaviour
{
    public static UnityEvent<int> OnCoinAmountUpdate = new UnityEvent<int>();

    public static CoinManager Instance;

    public int coinAmountUpperLimit;

    private int coinAmount;

    void Start()
    {
        Instance = this;

        coinAmount = SaveManager.Instance.GetSaveData().coinAmount;
        OnCoinAmountUpdate.Invoke(coinAmount);

        OnCoinAmountUpdate.AddListener(UpdateCoinAmountInSave);
    }

    public void AddCoinAmount(int coin)
    {
        coinAmount = Mathf.Min(coinAmount + coin, coinAmountUpperLimit);

        OnCoinAmountUpdate.Invoke(coinAmount);
    }

    public void RemoveCoinAmount(int coin)
    {
        coinAmount = Mathf.Max(coinAmount - coin, 0);

        OnCoinAmountUpdate.Invoke(coinAmount);
    }

    private void UpdateCoinAmountInSave(int coin)
    {
        SaveManager.Instance.GetSaveData().coinAmount = coin;
    }
}
