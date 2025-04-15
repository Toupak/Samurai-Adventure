using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class SamuraiFireball : MonoBehaviour
{
    [HideInInspector] public UnityEvent OnCastFireball = new UnityEvent();
    [HideInInspector] public UnityEvent OnCastBigFireball = new UnityEvent();

    public GameObject smallFireballPrefab;
    public GameObject bigFireballPrefab;
    public GameObject castingParticlePrefab;
    //transform.position + offset ?

    public AudioClip channeling;
    public AudioClip fullyChannelled;
    public AudioClip oom;

    //////
    public AudioClip fullyChanneledLoop;
    public GameObject fullyChanneledParticleLoopPrefab;
    private GameObject fullyChanneledParticleLoop;
    //////

    public int manaCostBigFireball;
    public int manaCostSmallFireball;

    public float cdFireball;
    public float bigFireballChargingTime;

    public float offsetToCharacter;
    public float offsetHeight;
    
    [HideInInspector] public bool isCharging;
    public bool hasLootedFireball;

    private SamuraiMovement movement;
    private PlayerHealth playerHealth;
    private LockEnemies lockEnemies;
    private SamuraiMana samuraiMana;

    private AudioSource channelingTemp;
    private AudioSource fullyChanneledTemp;
    private GameObject castingParticleTemp;
    
    private bool isIndicatorDisplayed;
    private float lastFireballTimeStamp;
    private float lastChannelTimeStamp;

    private void Start()
    {
        movement = GetComponent<SamuraiMovement>();
        playerHealth = GetComponent<PlayerHealth>();   
        lockEnemies = GetComponent<LockEnemies>();
        samuraiMana = GetComponent<SamuraiMana>();

        hasLootedFireball = SaveManager.Instance.GetSaveData().hasLootedFireball;

        PlayerHealth.OnPlayerDeath.AddListener((_) =>
        {
            if (channelingTemp != null)
                Destroy(channelingTemp.gameObject);
        });
    }

    private void Update()
    {
        if (playerHealth.IsDead)
            return;

        if (CanCast() && PlayerInput.GetFireballInput())
        {
            if (samuraiMana.CurrentMana >= manaCostBigFireball)
                ChannelCast();
            else if (samuraiMana.CurrentMana >= manaCostSmallFireball)
                CastFireball(false);
            else
                SFXManager.Instance.PlaySFX(oom);
        }

        if (!isIndicatorDisplayed && isCharging && IsBigFireball())
            DisplayIndicator();

        if (isCharging && PlayerInput.GetReleaseFireballInput())
            CastFireball(IsBigFireball());
    }

    private void CastFireball(bool isBigFireball)
    {
        lastFireballTimeStamp = Time.time;

        DestroyTemporarySoundsOnBigFireballCast();


        isCharging = false;

        Vector2 spawnDirection;
        if (!lockEnemies.IsTargeting())
            spawnDirection = movement.LastMovement;
        else
            spawnDirection = lockEnemies.GetTargetDirection();

        Vector2 spawnPosition = (Vector2)MainCharacter.Instance.transform.position + spawnDirection * offsetToCharacter + Vector2.up * offsetHeight;

        GameObject fireballTempPrefab = isBigFireball ? bigFireballPrefab : smallFireballPrefab;

        GameObject fireballTemp = Instantiate(fireballTempPrefab, spawnPosition, Quaternion.identity);
        fireballTemp.GetComponent<FireballProjectile>().Setup(spawnDirection);

        samuraiMana.ConsumeMana(isBigFireball ? manaCostBigFireball : manaCostSmallFireball);

        OnCastFireball.Invoke();
    }

    private void DestroyTemporarySoundsOnBigFireballCast()
    {
        if (castingParticleTemp != null)
            Destroy(castingParticleTemp);

        if (channelingTemp != null)
            Destroy(channelingTemp.gameObject);

        if (fullyChanneledTemp != null)
            Destroy(fullyChanneledTemp.gameObject);

        if (fullyChanneledParticleLoop != null)
            Destroy(fullyChanneledParticleLoop);
    }

    private void ChannelCast()
    {
        lastChannelTimeStamp = Time.time;
        isCharging = true;
        isIndicatorDisplayed = false;

        if (channelingTemp == null)
            channelingTemp = SFXManager.Instance.PlaySFX(channeling, volume: 0.05f, delay: 0.2f, loop:true);

        castingParticleTemp = Instantiate(castingParticlePrefab, transform.position + Vector3.up * 0.2f, Quaternion.identity, transform);
    }

    private void DisplayIndicator()
    {
        isIndicatorDisplayed = true;

        if (channelingTemp != null)
            Destroy(channelingTemp.gameObject);

        SFXManager.Instance.PlaySFX(fullyChannelled);
        fullyChanneledTemp = SFXManager.Instance.PlaySFX(fullyChanneledLoop, volume: 0.3f, loop: true);

        castingParticleTemp.GetComponent<Animator>().Play("FireBallParticle");

        fullyChanneledParticleLoop = Instantiate(fullyChanneledParticleLoopPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity, transform);
    }

    private bool CanCast()
    {
        return hasLootedFireball && movement.IsBusy() == false && Time.time - lastFireballTimeStamp > cdFireball;
    }

    public bool IsBigFireball()
    {
        return Time.time - lastChannelTimeStamp > bigFireballChargingTime;
    }
}
