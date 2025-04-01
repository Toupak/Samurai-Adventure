using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoDSpell : MonoBehaviour
{
    private Animator spellAnim;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D spellCollider;
    public Color blinkColor;
    public AudioSource castSound;
    public AudioSource lightingSound;

    public int damage;
    private bool hasHit;

    void Start()
    {
        spellAnim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spellCollider = GetComponent<CapsuleCollider2D>();
        spellCollider.enabled = false;

        StartCoroutine(SpellCoroutine());
    }

    IEnumerator SpellCoroutine()
    {
        Color OGColor = spriteRenderer.color;
        hasHit = false;

        spellAnim.Play("Spell");
        castSound.pitch = Random.Range(0.95f, 1.05f);
        castSound.Play();
        yield return new WaitForSeconds(0.083f);
        
        int index = 0;                                  
        while (index < 2)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(0.083f);
            spriteRenderer.color = OGColor;
            yield return new WaitForSeconds(0.083f);
            index++;
        }

        yield return new WaitForSeconds(0.75f-(0.083f*5f));

        LightingSoundPlayAndTrick();
        spellCollider.enabled = true;

        yield return new WaitForSeconds(0.25f);         
        spellCollider.enabled = false;
    }

    private void LightingSoundPlayAndTrick()
    {
        lightingSound.pitch = Random.Range(0.95f, 1.05f);
        lightingSound.Play();

        lightingSound.transform.SetParent(null);
        Destroy(lightingSound.gameObject, lightingSound.clip.length + 0.5f);
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && hasHit == false)
        {
            otherCollider.transform.parent.parent.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, otherCollider.ClosestPoint(transform.position), Vector2.zero);
            hasHit = true;
        }
    }
}
