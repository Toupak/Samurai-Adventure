using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : MonoBehaviour
{
    public AudioClip pickUpSound;
    public float timeBeforePickUp;
    
    public float speed;
    public float flyForDuration;

    private float spawnTimeStamp;
    private Vector2 smoothVelocity;
    private Rigidbody2D rb;
    private float itemSpawnTimeStamp;
    private bool hasBeenLooted;

    private void Start()
    {
        itemSpawnTimeStamp = Time.time;
    }

    private void Update()
    {
        if (rb == null)
            return;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref smoothVelocity, flyForDuration);

        if (Time.time - spawnTimeStamp > flyForDuration)
            rb.velocity = Vector2.zero;
    }

    protected void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && CanBePickedUp() && hasBeenLooted == false)
        {
            hasBeenLooted = true;
            PickUpItem();
        }
    }

    protected virtual void PickUpItem()
    {
        PickUpItemAnimation();
        PickUpItemEffect();
    }
    protected virtual void PickUpItemEffect()
    {
        PlayerHealth playerHealth = MainCharacter.Instance.GetComponent<PlayerHealth>();
        Squeeze squeeze = GetComponent<Squeeze>();

        squeeze.Trigger();
        playerHealth.RestoreHealth(1);
    }

    protected virtual void PickUpItemAnimation()
    {
        SFXManager.Instance.PlaySFX(pickUpSound);
        Destroy(gameObject, 0.1f);
    }

    public void IsThrownInDirection(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        spawnTimeStamp = Time.time;
        rb.velocity = direction * speed;
    }

    private bool CanBePickedUp()
    {
        return Time.time - itemSpawnTimeStamp > timeBeforePickUp;
    }
}
