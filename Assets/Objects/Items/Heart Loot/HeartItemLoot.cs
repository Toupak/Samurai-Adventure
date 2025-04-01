using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HeartItemLoot : ItemLoot
{
    [HideInInspector] public static UnityEvent OnLootHeartItem = new UnityEvent();

    protected override void PickUpItemEffect()
    {
        OnLootHeartItem.Invoke();
    }
}
