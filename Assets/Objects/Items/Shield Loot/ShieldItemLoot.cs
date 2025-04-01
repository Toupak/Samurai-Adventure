using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShieldItemLoot : ItemLoot
{
    [HideInInspector] public static UnityEvent OnLootShield = new UnityEvent();

    protected override void PickUpItemEffect()
    {
        BubbleShield shield = MainCharacter.Instance.GetComponent<BubbleShield>();

        shield.hasLootedShield = true;
        SaveManager.Instance.GetSaveData().hasLootedShield = true;

        OnLootShield.Invoke();
    }
}
