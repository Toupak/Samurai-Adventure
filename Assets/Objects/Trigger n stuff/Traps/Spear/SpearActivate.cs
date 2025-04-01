using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class SpearActivate : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D spearCollider;
    private Animator animator;
    private AudioSource spearSound;
    public Color blinkColor;
    public Trigger triggerActivate;
    public Trigger triggerDeactivate;

    private bool hasHit;
    private bool isActivated;
    private bool isCancelled;
    public int damage;
    public float offset;
    public float delay;
    float initialVolume;
    private const float timeStayingExtendedAfterActivation = 1.0f;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        triggerActivate.OnTrigger.AddListener(() => StartCoroutine(Activate()));
        triggerDeactivate.OnTrigger.AddListener(() => isCancelled = true);
        spearCollider = GetComponent<CapsuleCollider2D>();
        spearSound = transform.GetChild(0).GetComponent<AudioSource>();
        initialVolume = spearSound.volume;
        spearCollider.enabled = false;
    }

    private IEnumerator Activate()
    {
        if (isActivated == true)
            yield break;

        isActivated = true;
        yield return new WaitForSeconds(offset);
        while (isCancelled == false)
        {
            hasHit = false;
            yield return BlinkBeforeActivation();
            SpearAnimation();
            yield return new WaitForSeconds(0.05f);
            spearCollider.enabled = true;
            yield return new WaitForSeconds(timeStayingExtendedAfterActivation);
            DeactivateAnimation();
            spearCollider.enabled = false;
            yield return new WaitForSeconds(delay);
        }
        isActivated = false;
        isCancelled = false;
    }

    private IEnumerator BlinkBeforeActivation()
    {
        Color OGColor = spriteRenderer.color;

        for (float i = 0; i < 3; i++)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = OGColor;
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void SpearAnimation()
    {
        animator.Play("Activate");
        spearSound.pitch = Random.Range(0.95f, 1.05f);
        spearSound.volume = Random.Range(-0.02f, 0.02f) + initialVolume;
        spearSound.Play();
    }

    private void DeactivateAnimation()
    {
        animator.Play("Deactivated");
        spearSound.pitch = Random.Range(0.95f, 1.05f);
        spearSound.volume = Random.Range(-0.02f, 0.02f) + initialVolume;
        spearSound.Play();
    }

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && hasHit == false)
        {
            otherCollider.transform.parent.parent.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, otherCollider.ClosestPoint(transform.position), Vector2.up);
            hasHit = true;
        }
    }
}
