using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericProjectile : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnDestroy = new UnityEvent();

    public float maxRange;
    public float speed;

    public bool rotateProjectile;

    public AudioClip hashitSound;
    public AudioClip nohitSound;

    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Vector2 positionStart;

    protected Animator animator;

    protected bool hasHit;
    protected bool hasBeenRemoved;

    protected ColliderDamage colliderDamage;

    protected void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliderDamage = GetComponent<ColliderDamage>();
    }

    protected void Update()
    {
        float currentRange = ((Vector2)transform.position - positionStart).magnitude;

        if (currentRange > maxRange)
            Remove();

        if (colliderDamage != null && colliderDamage.enemyHit.Count > 0)
            Remove();
    }

    public virtual void Setup(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (colliderDamage == null)
            colliderDamage = GetComponent<ColliderDamage>();

        rb.velocity = direction * speed;
        positionStart = transform.position;

        if (colliderDamage != null)
            colliderDamage.ActivateDamage();

        if (rotateProjectile == true)
            transform.localRotation = direction.ToRotation();
    }

    protected virtual void Remove()
    {
        if (hasBeenRemoved)
            return;

        hasBeenRemoved = true;
        
        if (colliderDamage == null)
            colliderDamage = GetComponent<ColliderDamage>();

        rb.velocity = Vector2.zero;
        colliderDamage.DeactivateDamage();

        PlayAnimationandSoundRemove();

        OnDestroy.Invoke();
        Destroy(gameObject, 0.5f);
    }

    private void PlayAnimationandSoundRemove()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        animator.Play("Remove");

        if (colliderDamage.enemyHit.Count > 0)
            SFXManager.Instance.PlaySFX(hashitSound);
        else
            SFXManager.Instance.PlaySFX(nohitSound);
    }
}
