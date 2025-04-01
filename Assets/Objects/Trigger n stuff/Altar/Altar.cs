using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Altar : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnAltarTrigger = new UnityEvent();

    private Animator animator;

    private bool hasBeenTriggered;
    private float blinkTimeStamp;

    void Start()
    {
        animator = GetComponent<Animator>();

        GetComponent<EnemyHealth>().OnTakeDamage.AddListener((attackType) =>
        {
            if (hasBeenTriggered == false && attackType == ColliderDamage.AttackType.Magic)
            {
                hasBeenTriggered = true;
                TriggerAltarEvent();
            }
        });
    }

    void Update()
    {
        if (hasBeenTriggered == false && Time.time > blinkTimeStamp)
            Blink();
    }

    protected virtual void TriggerAltarEvent()
    {
        animator.Play("Close");
        OnAltarTrigger.Invoke();
    }

    private void Blink()
    {
        float randomizedDuration = Random.Range(20f, 45f);

        blinkTimeStamp = Time.time + randomizedDuration;
        animator.Play("Blink");
    }
}
