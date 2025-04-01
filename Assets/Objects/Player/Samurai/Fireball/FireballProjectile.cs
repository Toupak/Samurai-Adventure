using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireballProjectile : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnDestroy = new UnityEvent();
    
    public float maxRange;
    public float speed;

    public GameObject explodeVFXPrefab;

    public AudioClip cast;
    public AudioClip travelling;
    public AudioClip explode;

    private Animator animator;
    private Rigidbody2D rb;

    private bool hasHit;
    private Vector2 positionStart;

    private ColliderDamage colliderDamage;
    private SamuraiMovement samuraiMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliderDamage = GetComponent<ColliderDamage>();
        samuraiMovement = MainCharacter.Instance.GetComponent<SamuraiMovement>();
    }

    void Update()
    {
        float currentRange = ((Vector2)transform.position - positionStart).magnitude;

        if (currentRange > maxRange)
            Remove();

        if (colliderDamage != null && colliderDamage.enemyHit.Count > 0)
            Remove();
    }

    public void Setup(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (samuraiMovement == null)
            samuraiMovement = MainCharacter.Instance.GetComponent<SamuraiMovement>();

        if (colliderDamage == null)
            colliderDamage = GetComponent<ColliderDamage>();       

        rb.velocity = direction * speed;
        positionStart = transform.position;

        colliderDamage.ActivateDamage();

        transform.localRotation = direction.ToRotation();
        SFXManager.Instance.PlaySFX(cast);
        SFXManager.Instance.PlaySFX(travelling, delay: 0.15f);
    }

    void Remove()
    {
        if (colliderDamage == null)
            colliderDamage = GetComponent<ColliderDamage>();

        rb.velocity = Vector2.zero;
        colliderDamage.DeactivateDamage();

        SFXManager.Instance.PlaySFX(explode);
        Instantiate(explodeVFXPrefab, transform.position, Quaternion.identity);

        OnDestroy.Invoke();
        Destroy(gameObject);
    }
}
