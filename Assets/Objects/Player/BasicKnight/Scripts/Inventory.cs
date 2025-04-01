using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    private bool keyFirstHalf;
    private bool keySecondHalf;

    public static UnityEvent OnLootKey = new UnityEvent();

    public bool HasFullKey()
    {
        return keyFirstHalf && keySecondHalf;
    }

    public void SetDoorKey(Key.KeyPart part,  bool newstate)
    {
        if (part == Key.KeyPart.FirstHalf)
            keyFirstHalf = newstate;

        if (part == Key.KeyPart.SecondHalf)
            keySecondHalf = newstate;

        OnLootKey.Invoke();
    }

    public (bool, bool) GetKeyState()
    {
        return (keyFirstHalf, keySecondHalf);
    }
}
