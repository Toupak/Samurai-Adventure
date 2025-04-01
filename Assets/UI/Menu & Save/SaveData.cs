using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int maxHearts;
    public int halfHeartsLoot;

    public Vector3 currentFountainPosition;

    public bool hasLootedFireball;
    public bool hasLootedShield;

    public SaveData()
    {
        maxHearts = 3;
    }
}
