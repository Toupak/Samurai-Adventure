using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Obelisk : Trigger
{
    [HideInInspector] public UnityEvent OnObeliskDestructTrigger = new UnityEvent();

    public bool isSpawner;
    public GameObject triggerObject;
    public AudioClip success;

    public List<Sprite> brokenPillars;
    public List<SpriteRenderer> shadowsBrokenPillars;
    public AudioClip rubble;
    public AudioClip explode;
    public GameObject destroyParticle;

    public GameObject hurtbox;
    public GameObject hurtboxBroken;
    public GameObject hitbox;
    public GameObject hitboxBroken;

    public SpriteRenderer shadowSpriteRenderer;
    protected SpriteRenderer spriteRenderer;
    protected Color OGColor;
    protected EnemyHealth obeliskHP;
    protected bool hasBeenDestroyed;
    protected bool hasBeenTriggered;
    protected bool isActivated;

    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        obeliskHP = GetComponent<EnemyHealth>();
        OGColor = spriteRenderer.color;

        obeliskHP.OnTakeDamage.AddListener((attackType) =>
        {
            if (hasBeenDestroyed == false)
            {
                if (obeliskHP.hp == 2)
                {
                    obeliskHP.isInvincible = true;
                    hasBeenDestroyed = true;
                    TriggerObeliskDestructEvent();
                }
                else
                    obeliskHP.hp = obeliskHP.maxHp;
            }
        });

        if (triggerObject != null)
            triggerObject.GetComponent<Trigger>().OnTrigger.AddListener(ActivateObelisk);
    }

    protected void ActivateObelisk()
    {
        if (hasBeenTriggered == true)
            return;

        hasBeenTriggered = true;
        isActivated = true;
        StartCoroutine(Blink());
    }

    protected virtual void TriggerObeliskDestructEvent()
    {
        isActivated = false;
        spriteRenderer.color = OGColor;

        if (success != null)
            SFXManager.Instance.PlaySFX(success);

        SpawnDestroyedObelisk();
        OnTrigger.Invoke();
    }

    protected void SpawnDestroyedObelisk()
    {
        int brokenPillarRandomizer = Random.Range(0, brokenPillars.Count);

        hitbox.SetActive(false);
        hurtbox.SetActive(false);
        hitboxBroken.SetActive(true);
        hurtboxBroken.SetActive(true);

        spriteRenderer.sprite = brokenPillars[brokenPillarRandomizer];
        shadowSpriteRenderer.sprite = shadowsBrokenPillars[brokenPillarRandomizer].sprite;

        SFXManager.Instance.PlaySFX(explode, volume: 0.05f);
        SFXManager.Instance.PlaySFX(rubble, volume: 0.05f);
        Instantiate(destroyParticle, (Vector2)transform.position + Vector2.up * 1f, Quaternion.identity);
    }

    protected IEnumerator Blink()
    {
        while (isActivated == true)
        {
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = OGColor;
            yield return new WaitForSeconds(0.2f);
        }
    }
}
