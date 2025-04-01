using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackTrigger : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAttackTrigger = new UnityEvent();

    public float cdBetweenActivation;
    public bool isActivated;

    private float lastTriggerTimeStamp;

    protected void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("PlayerAttack"))
        {
            if (CanBeTriggered())
            {
                lastTriggerTimeStamp = Time.time;
                isActivated = !isActivated;
                TriggerEvent();
            }

        }
    }

    protected virtual void TriggerEvent()
    {
        OnAttackTrigger.Invoke();
    }

    private bool CanBeTriggered()
    {
        return Time.time - lastTriggerTimeStamp > cdBetweenActivation;
    }
}
