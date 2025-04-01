using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class DKTrainingDummy : MonoBehaviour
{
    public Animator animator;
    Squeeze squeeze;
    public AudioClip hurtSound;

    private void Start()
    {
        squeeze = GetComponent<Squeeze>();
    }

    public void DummyTakeDamage()
    {
        animator.Play("Hurt_Down");
        squeeze.Trigger();
        SFXManager.Instance.PlaySFX(hurtSound);
    }
}