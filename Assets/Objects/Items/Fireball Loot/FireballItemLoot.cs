using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireballItemLoot : ItemLoot
{
    [HideInInspector] public static UnityEvent OnLootFireball = new UnityEvent();

    protected override void PickUpItemEffect()
    {
        SamuraiFireball samuraiFireball = MainCharacter.Instance.GetComponent<SamuraiFireball>();

        samuraiFireball.hasLootedFireball = true;
        SaveManager.Instance.GetSaveData().hasLootedFireball = true;

        OnLootFireball.Invoke();
    }
}
