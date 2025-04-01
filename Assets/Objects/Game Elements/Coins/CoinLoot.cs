using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinLoot : ItemLoot
{
    public int coinAmount;
    public float speed;
    public float flyForDuration;

    private float spawnTimeStamp;
    private Vector2 smoothVelocity;

    private Rigidbody2D rb;
    private Squeeze squeeze;

    private void Update()
    {
        if (rb == null)
            return;

        rb.velocity = Vector2.SmoothDamp(rb.velocity, Vector2.zero, ref smoothVelocity, flyForDuration);

        if (Time.time - spawnTimeStamp > flyForDuration)
            rb.velocity = Vector2.zero;
    }

    protected override void PickUpItem()
    {
        PickUpItemAnimation();
        PickUpItemEffect();
    }

    protected override void PickUpItemAnimation()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            SFXManager.Instance.PlaySFX(pickUpSound, delay:0.1f * i);
        }

        Squeeze squeeze = GetComponent<Squeeze>();
        squeeze.Trigger();

        Destroy(gameObject, 0.1f);
    }

    protected override void PickUpItemEffect()
    {
        AddCoinAmount(); // Invoke un event ?
    }

    private void AddCoinAmount()
    {
        
    }

    public void IsThrownInDirection(Vector2 direction)
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        spawnTimeStamp = Time.time;
        rb.velocity = direction * speed;
    }
}
