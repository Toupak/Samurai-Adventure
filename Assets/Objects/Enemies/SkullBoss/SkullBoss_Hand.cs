using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SkullBoss_Hand : MonoBehaviour
{
    public GameObject idleHand;
    public GameObject swipeHand;
    public GameObject slamHand;
    public GameObject clapHand;

    public GameObject slamHandGraphics;

    public Transform otherHand;

    public ColliderDamage swipeHandColliderDamage;
    public ColliderDamage slamHandColliderDamage;

    public SkullBoss_Head bossHead;

    public GameObject slamImpactParticle;
    public AudioClip slamSound;
    public AudioClip slamWhoosh;
    public List<AudioClip> rubbleSounds;

    public GameObject swipeParticle;
    public AudioClip swipeWhoosh;
    public AudioClip swipeTravelSound;
    public float swipeParticleSpawnInterval;

    public bool isLeftHand;

    public float movingSpeed;

    public float swipeSpeed;
    public float swipeSetupDistanceToPlayer;
    public float swipeAttackDistance;
    public float delayBeforeSwipe;

    public float slamSpeed;
    public float slamSetupDistanceToPlayer;
    public float slamAttackDistance;
    public float delayBeforeSlam;
    public float delayAfterSlam;

    [HideInInspector] public bool isAttacking;
    [HideInInspector] public Rigidbody2D rb;

    private Vector2 startingPosition;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        startingPosition = transform.position;
    }

    public void Attack(SkullBoss_Head.SkullBossAttack whichAttack)
    {
        switch (whichAttack)
        {
            case SkullBoss_Head.SkullBossAttack.HandSwipe:
                StartCoroutine(HandSwipe());
                break;
            case SkullBoss_Head.SkullBossAttack.HandSlam:
                StartCoroutine(HandSlam());
                break;
            default:
                break;
        }
    }

    private IEnumerator HandSwipe()
    {
        isAttacking = true;

        idleHand.SetActive(false);
        swipeHand.SetActive(true);

        Vector2 playerPosition = MainCharacter.Instance.transform.position;
        Vector2 targetPosition = playerPosition;

        //placement au bon endroit pour swipe
        targetPosition += isLeftHand ? Vector2.left * swipeSetupDistanceToPlayer : Vector2.right * swipeSetupDistanceToPlayer;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);
            yield return null;
            playerPosition = MainCharacter.Instance.transform.position;
            targetPosition = playerPosition;
            targetPosition += isLeftHand ? Vector2.left * swipeSetupDistanceToPlayer : Vector2.right * swipeSetupDistanceToPlayer;
        }

        //TODO Indicateur visuel
        yield return new WaitForSeconds(delayBeforeSwipe);

        PlaySwipeSound();

        //léger recul
        targetPosition += isLeftHand ? Vector2.left * 2 : Vector2.right * 2;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, 20f * Time.deltaTime);
            yield return null;
        }

        swipeHandColliderDamage.ActivateDamage();
        yield return new WaitForFixedUpdate();

        //swipe
        float lastSwipeParticleSpawnTimeStamp = 0;

        targetPosition += isLeftHand ? Vector2.right * swipeAttackDistance : Vector2.left * swipeAttackDistance;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            if (lastSwipeParticleSpawnTimeStamp > swipeParticleSpawnInterval)
            {
                Instantiate(swipeParticle, ComputeSwipeParticlePosition(), Quaternion.identity);
                Instantiate(swipeParticle, ComputeSwipeParticlePosition(), Quaternion.identity);
                Instantiate(swipeParticle, ComputeSwipeParticlePosition(), Quaternion.identity);
                Instantiate(swipeParticle, ComputeSwipeParticlePosition(), Quaternion.identity);
                Instantiate(swipeParticle, ComputeSwipeParticlePosition(), Quaternion.identity);
                lastSwipeParticleSpawnTimeStamp = 0;
            }

            rb.MovePosition(Vector2.MoveTowards(rb.position, targetPosition, swipeSpeed * Time.fixedDeltaTime));
            yield return new WaitForFixedUpdate();
            lastSwipeParticleSpawnTimeStamp += Time.deltaTime;
        }

        yield return new WaitForFixedUpdate();
        swipeHandColliderDamage.DeactivateDamage();

        //retourne au bon endroit
        swipeHand.SetActive(false);
        idleHand.SetActive(true);
        targetPosition = startingPosition;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);
            yield return null;
        }

        bossHead.lastAttackTimeStamp = Time.time;
        isAttacking = false;
    }

    private Vector2 ComputeSwipeParticlePosition()
    {
        return (Vector2)transform.position + Random.insideUnitCircle.normalized * 0.75f + Vector2.down * 0.9f;
    }

    private void PlaySwipeSound()
    {
        SFXManager.Instance.PlaySFX(swipeWhoosh);
        SFXManager.Instance.PlaySFX(swipeTravelSound, volume: 0.05f);
    }

    private IEnumerator HandSlam()
    {
        isAttacking = true;

        idleHand.SetActive(false);
        slamHand.SetActive(true);

        Vector2 playerPosition = MainCharacter.Instance.transform.position;
        Vector2 targetPosition = playerPosition;

        //se place au bon endroit
        targetPosition = Vector3.up * slamSetupDistanceToPlayer;
        while (targetPosition.Distance(slamHandGraphics.transform.localPosition) > 0.05f)
        {
            slamHandGraphics.transform.localPosition = Vector2.MoveTowards(slamHandGraphics.transform.localPosition, targetPosition, movingSpeed * Time.deltaTime);
            yield return null;
        }

        targetPosition = playerPosition;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);
            yield return null;
            playerPosition = MainCharacter.Instance.transform.position;
            targetPosition = playerPosition;
        }

        yield return new WaitForSeconds(delayBeforeSlam);
        SFXManager.Instance.PlaySFX(slamWhoosh, volume: 0.5f);

        //léger recul
        targetPosition = Vector3.up * slamSetupDistanceToPlayer + Vector3.up * 2;
        while (targetPosition.Distance(slamHandGraphics.transform.localPosition) > 0.05f)
        {
            slamHandGraphics.transform.localPosition = Vector2.MoveTowards(slamHandGraphics.transform.localPosition, targetPosition, 20f * Time.deltaTime);
            yield return null;
        }

        slamHandColliderDamage.ActivateDamage();
        yield return new WaitForFixedUpdate();


        //slam
        targetPosition = Vector2.zero;
        while (targetPosition.Distance(slamHandGraphics.transform.localPosition) > 0.05f)
        {
            slamHandGraphics.transform.localPosition = Vector2.MoveTowards(slamHandGraphics.transform.localPosition, targetPosition, swipeSpeed * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }

        SlamAnimationAndSound();

        yield return new WaitForFixedUpdate();
        slamHandColliderDamage.DeactivateDamage();

        yield return new WaitForSeconds(delayAfterSlam);

        //retourne au bon endroit
        slamHand.SetActive(false);
        idleHand.SetActive(true);
        targetPosition = startingPosition;
        while (targetPosition.Distance(transform.position) > 0.05f)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);
            yield return null;
        }

        bossHead.lastAttackTimeStamp = Time.time;
        isAttacking = false;
    }

    private void SlamAnimationAndSound()
    {
        Instantiate(slamImpactParticle, transform.position, Quaternion.identity);
        SFXManager.Instance.PlaySFX(slamSound);
        SFXManager.Instance.PlayRandomSFXAtLocation(rubbleSounds.ToArray(), transform);
    }

    public void RegenHealth()
    {
        EnemyHealth handHealth = GetComponent<EnemyHealth>();
        handHealth.hp = handHealth.maxHp;

        transform.position = startingPosition;

        isAttacking = false;

        idleHand.SetActive(true);
        swipeHand.SetActive(false);
        slamHand.SetActive(false);
        clapHand.SetActive(false);

        swipeHandColliderDamage.DeactivateDamage();
        slamHandColliderDamage.DeactivateDamage();

        gameObject.SetActive(false);
    }
}
