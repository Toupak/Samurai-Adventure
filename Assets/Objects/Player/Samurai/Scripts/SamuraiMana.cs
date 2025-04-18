using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SamuraiMana : MonoBehaviour
{
    public static UnityEvent<int, int> OnManaUpdate = new UnityEvent<int, int>();

    public int maxMana;

    private int currentMana;
    public int CurrentMana => currentMana;

    void Start()
    {
        currentMana = maxMana;
        OnManaUpdate.Invoke(currentMana, maxMana);

        SamuraiAttack.OnHitEnemy.AddListener((_,attackType) => 
        {
            if (attackType == ColliderDamage.AttackType.Sword)
                RestoreMana(1);
        });
    }

    public void RestoreMana(int mana)
    {
        UpdateMana(mana);
    }

    public void ConsumeMana(int mana)
    {
        UpdateMana(-mana);
    }

    private void UpdateMana(int mana)
    {
        currentMana = Mathf.Clamp(currentMana + mana, 0, maxMana);
        OnManaUpdate.Invoke(currentMana, maxMana);
    }
}
