using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkOnHit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color blinkColor;
    public float duration;

    void Start()
    {
        GetComponent<EnemyHealth>().OnTakeDamage.AddListener((_) => Blink());
    }

    void Blink()
    {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        Color OGColor = spriteRenderer.color;
        spriteRenderer.color = blinkColor;
        yield return new WaitForSeconds(duration);
        spriteRenderer.color = OGColor;
    }
}