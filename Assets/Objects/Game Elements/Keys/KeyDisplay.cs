using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyDisplay : MonoBehaviour
{
    private Image key;

    public Sprite spriteFirstHalf;
    public Sprite spriteSecondHalf;
    public Sprite spriteFull;
    public Sprite empty;

    private void Start()
    {
        key = GetComponent<Image>();
        Inventory.OnLootKey.AddListener(ChangeDisplay);

        // s'update selon le chargement
        key.sprite = empty;
    }

    private void ChangeDisplay()
    {
        (bool firstHalf, bool secondHalf) = MainCharacter.Instance.GetComponent<Inventory>().GetKeyState();

        if (firstHalf && secondHalf)
            key.sprite = spriteFull; 

        if (firstHalf && !secondHalf)
            key.sprite = spriteFirstHalf;

        if (secondHalf && !firstHalf)
            key.sprite = spriteSecondHalf;

        if (!firstHalf && !secondHalf)
            key.sprite = null;
    }
}
