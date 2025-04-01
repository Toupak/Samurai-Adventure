using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldTextAppear : MonoBehaviour
{
    public GameObject shieldText;
    public GameObject worm;

    void Start()
    {
        ShieldItemLoot.OnLootShield.AddListener(MakeTextVisible);
    }

    private void MakeTextVisible()
    {
        shieldText.SetActive(true);
        worm.SetActive(true);
    }
}
