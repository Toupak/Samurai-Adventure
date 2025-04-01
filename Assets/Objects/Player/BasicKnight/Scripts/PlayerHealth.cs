using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Drawing;
using Color = UnityEngine.Color;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public static UnityEvent<Vector2> OnPlayerDeath = new UnityEvent<Vector2>();
    [HideInInspector] public static UnityEvent OnPlayerRespawn = new UnityEvent();
    [HideInInspector] public static UnityEvent<Vector2, Vector2> OnPlayerTakeDamage = new UnityEvent<Vector2, Vector2>();
    [HideInInspector] public static UnityEvent OnPlayerLowHP = new UnityEvent();
    [HideInInspector] public UnityEvent OnRestoreHealth = new UnityEvent();

    public int maxHearts;
    public int hearts;

    public float bleedDuration;
    public Image bleedScreen;
    
    public float InvincibilityDuration;
    public SpriteRenderer samuraiSpriteRenderer;
    public Color invincibilityColor;

    public float zoom;
    public float zoomDuration;

    public bool IsDead => hearts <= 0f;
    public bool IsDying => Time.time <= deathTimeStamp + deathDuration;

    private Squeeze squeeze;
    private BubbleShield shield;
    private SamuraiMovement samuraiMovement;
    private HeartHealthBar heartHealthBar;
    private SamuraiMana samuraiMana;
    
    private float deathDuration = 0.5f;
    private float deathTimeStamp = 0f;
    private float lastInvincibilityTimeStamp = -5f;

    private int halfHeartLooted;

    private void Awake()
    {
        DontDestroyOnLoad(bleedScreen.transform.parent.gameObject);
        squeeze = GetComponent<Squeeze>();
        shield = GetComponent<BubbleShield>();
        heartHealthBar = GetComponent<HeartHealthBar>();

        HeartItemLoot.OnLootHeartItem.AddListener(AddHalfHeart);
    }

    private void Start()
    {
        hearts = SaveManager.Instance.GetSaveData().maxHearts;
        maxHearts = SaveManager.Instance.GetSaveData().maxHearts;
        heartHealthBar.SetMaxHeart(maxHearts);
    }

    public bool TakeDamage(int damage, Vector2 position, Vector2 direction)
    {
        if (IsDead)
            return false;

        if (IsInvincible())
            return false;

        if (shield.IsShielded())
        {
            return shield.TakeDamage(damage, position, direction);
        }

        hearts -= damage;
        StartCoroutine(Bleed());
        squeeze.Trigger();
        CameraZoom.RequestCameraZoomForDuration.Invoke(zoom, zoomDuration);

        if (IsDead)
        {
             deathTimeStamp = Time.time;
             OnPlayerDeath.Invoke(position);
        }
        else
        {
            lastInvincibilityTimeStamp = Time.time;
            StartCoroutine(BlinkWhenInvincible());
            OnPlayerTakeDamage.Invoke(position, direction);
        }

        return true;
    }

    public void RestoreHealth(int health)
    {
        hearts += health;

        if (hearts > maxHearts)
            hearts = maxHearts;
        
        OnRestoreHealth.Invoke();
        bleedScreen.gameObject.SetActive(false);
    }

    public void Respawn()
    {
        hearts = maxHearts;

        samuraiMovement = GetComponent<SamuraiMovement>();
        samuraiMovement.lastMovement = Vector2.down;

        samuraiMana = GetComponent<SamuraiMana>();
        samuraiMana.RestoreMana(samuraiMana.maxMana);

        OnPlayerRespawn.Invoke();
    }

    private void AddHalfHeart()
    {
        halfHeartLooted = SaveManager.Instance.GetSaveData().halfHeartsLoot;
        halfHeartLooted += 1;

        if (halfHeartLooted == 2)
        {
            maxHearts += 1;
            SaveManager.Instance.GetSaveData().maxHearts = maxHearts;

            hearts += 1;
            halfHeartLooted = 0;

            HeartHealthBar heartHealthBar = MainCharacter.Instance.GetComponent<HeartHealthBar>();
            heartHealthBar.SetMaxHeart(maxHearts);
            heartHealthBar.UpdateHeart();
        }

        SaveManager.Instance.GetSaveData().halfHeartsLoot = halfHeartLooted;
    }

    private IEnumerator Bleed()
    {
        if (IsDead)
            bleedDuration = 1.5f;

        bleedScreen.gameObject.SetActive(true);
        Color color = bleedScreen.color;

        float timer = 0f;
        while (timer < bleedDuration)
        {
            color.a = Tools.NormalizeValue(timer, 0, bleedDuration);
            bleedScreen.color = color;
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        if (hearts == 1)
        {
            OnPlayerLowHP.Invoke();
            yield break;
        }

        timer = 0f;
        while (timer < bleedDuration)
        {
            color.a = 1.0f - Tools.NormalizeValue(timer, 0, bleedDuration);
            bleedScreen.color = color;
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        bleedScreen.gameObject.SetActive(false);
    }

    private IEnumerator BlinkWhenInvincible()
    {
        while (IsInvincible())
        {
            samuraiSpriteRenderer.color = invincibilityColor;
            yield return new WaitForSeconds(0.1f);
            samuraiSpriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
        }
        samuraiSpriteRenderer.color = Color.white;
    }

    private bool IsInvincible()
    {
        return Time.time - lastInvincibilityTimeStamp < InvincibilityDuration || DialoguePanel.Instance.isReading;
    }
}