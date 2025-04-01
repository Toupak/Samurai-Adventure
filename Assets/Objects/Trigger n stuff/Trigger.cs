using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Trigger : MonoBehaviour
{
    protected bool isWithinRange;

    [HideInInspector] public UnityEvent OnTrigger = new UnityEvent();

    protected void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
            isWithinRange = true;
    }

    protected void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
            isWithinRange = false;
    }
}
