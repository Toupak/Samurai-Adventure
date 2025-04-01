using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : Trigger
{

    protected void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            OnTrigger.Invoke();
            PlayAnimationPressurePlate();
        }
    }

    protected void PlayAnimationPressurePlate()
    {
        return;
    }
}
