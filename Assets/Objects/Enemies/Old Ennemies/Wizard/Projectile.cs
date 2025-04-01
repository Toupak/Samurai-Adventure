using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;

    private bool hasHit;

    private Vector2 positionStart;
    public float maxRange;
    public int damage;
    public float speed;

    void Start()
    {
        animator = transform.GetChild(0).GetChild(0).GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float currentRange = ((Vector2)transform.position - positionStart).magnitude;

        if (currentRange > maxRange)
            Remove();
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && hasHit == false)
        {
            otherCollider.transform.parent.parent.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, otherCollider.ClosestPoint(transform.position), Vector2.zero);
            hasHit = true;
            Remove();
        }

        if (otherCollider.transform.CompareTag("Wall") && hasHit == false)
        {
            hasHit = true;
            Remove();
        }
    }

    public void Setup(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        rb.velocity = direction * speed;
        positionStart = transform.position;

        transform.GetChild(0).localRotation = direction.ToRotation();
    }

    void Remove()
    {
        rb.velocity = Vector2.zero;
        animator.Play("Explode");
        Destroy(gameObject, 0.5f);
    }
}
