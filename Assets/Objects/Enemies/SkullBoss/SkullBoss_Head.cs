using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkullBoss_Head : MonoBehaviour
{
    public enum SkullBossAttack
    {
        HandSwipe,
        HandSlam,
        HandClap,
        CircularLaserAttack
    }

    public SkullBoss_Hand leftHand;
    public SkullBoss_Hand rightHand;

    public EnemyHealth leftHandHealth;
    public EnemyHealth rightHandHealth;

    public ColliderDamage leftClapHandColliderDamage;
    public ColliderDamage rightClapHandColliderDamage;

    public GameObject clapImpactParticle;
    public GameObject leftClapParticle;
    public GameObject rightClapParticle;
    public AudioClip clapWhoosh;
    public AudioClip clapTravelSound;
    public AudioClip clapImpact;
    private float clapParticleSpawnInterval = 0.04f;

    public Animator headAnimator;
    public AudioClip angrySound;

    public float cdAfterAttacks;

    public float delayBeforeClap;
    public float delayAfterClap;
    public float clapSpeed;
    public float clapOffsetPosition;

    public GameObject screamParticle;
    public float spawnScreamParticleInterval;
    public float screamPushSpeed;
    public float screamPushDuration;

    public SpriteRenderer skullSprite;
    public Sprite lookDown;
    public Sprite lookLeft;
    public Sprite lookFarLeft;
    public Sprite lookRight;
    public Sprite lookFarRight;

    [HideInInspector] public float lastAttackTimeStamp;

    private bool isAttacking;

    private float activationTimeStamp;
    private float lastAngrySoundTimeStamp;

    void Start()
    {
        leftHandHealth.OnDeath.AddListener(() => ChangeBossStats(true));
        rightHandHealth.OnDeath.AddListener(() => ChangeBossStats(false));

        activationTimeStamp = Time.time;
        PlayAngrySound();
    }

    void Update()
    {
        if (HandCanAttack())
            HandAttack();
    }

    private void LateUpdate()
    {
        if (CanLook())
            LookAtPlayer();
    }

    private void HandAttack()
    {
        SkullBossAttack whichAttack = SelectAttack();

        if (whichAttack == SkullBossAttack.HandClap)
            StartCoroutine(HandClap());
        else
        {
            if (leftHand.isActiveAndEnabled == false)
                rightHand.Attack(whichAttack);
            else if (rightHand.isActiveAndEnabled == false)
                leftHand.Attack(whichAttack);
            else
            {
                if (Tools.RandomBool() == true)
                    leftHand.Attack(whichAttack);
                else
                    rightHand.Attack(whichAttack);
            }
        }
    }

    private SkullBossAttack SelectAttack()
    {
        int whichAttackRandomizer = Random.Range(0, 10);

        if (leftHand.isActiveAndEnabled == true && rightHand.isActiveAndEnabled == true)
        {
            if (whichAttackRandomizer < 4)
                return SkullBossAttack.HandSwipe;
            else if (whichAttackRandomizer <= 7)
                return SkullBossAttack.HandSlam;
            else
                return SkullBossAttack.HandClap;
        }
        else
        {
            if (whichAttackRandomizer < 5)
                return SkullBossAttack.HandSwipe;
            else
                return SkullBossAttack.HandSlam;
        }
    }

    public void ScreamAttack()
    {
        if (!HandCanAttack())
            return;

        PlayAngrySound();
        StartCoroutine(SpawnScreamParticle());
        MainCharacter.Instance.GetComponent<SamuraiMovement>().Stagger(Vector2.down, screamPushSpeed, screamPushDuration);
    }

    private IEnumerator SpawnScreamParticle()
    {
        float screamTimeStamp = Time.time;

        while (Time.time - screamTimeStamp < 0.75f)
        {
            Instantiate(screamParticle, ComputeRandomScreamParticlePosition(), Vector2.left.ToRotation());
            yield return new WaitForSeconds(spawnScreamParticleInterval);
        }
    }

    private Vector2 ComputeRandomScreamParticlePosition()
    {
        Vector2 RandomAngleParticle = (Vector2.down).AddRandomAngleToDirection(-45f, 45f);
        Vector2 RandomLength = RandomAngleParticle * (Random.Range(1, 5));
        return (Vector2)transform.position + RandomLength;
    }

    private IEnumerator HandClap()
    {
        isAttacking = true;

        leftHand.idleHand.SetActive(false);
        rightHand.idleHand.SetActive(false);

        leftHand.clapHand.SetActive(true);
        rightHand.clapHand.SetActive(true);

        Vector2 leftHandStartingPosition = leftHand.transform.position;
        Vector2 rightHandStartingPosition = rightHand.transform.position;
        Vector2 playerPosition = MainCharacter.Instance.transform.position;
        Vector2 leftHandTargetPosition = playerPosition;
        Vector2 rightHandTargetPosition = playerPosition;

        //placement au bon endroit pour swipe
        leftHandTargetPosition += Vector2.left * leftHand.swipeSetupDistanceToPlayer;
        rightHandTargetPosition += Vector2.right * rightHand.swipeSetupDistanceToPlayer;

        while (leftHandTargetPosition.Distance(leftHand.transform.position) > 0.05f || rightHandTargetPosition.Distance(rightHand.transform.position) > 0.05f)
        {
            leftHand.transform.position = Vector2.MoveTowards(leftHand.transform.position, leftHandTargetPosition, leftHand.movingSpeed * Time.deltaTime);
            rightHand.transform.position = Vector2.MoveTowards(rightHand.transform.position, rightHandTargetPosition, rightHand.movingSpeed * Time.deltaTime);
            yield return null;
            playerPosition = MainCharacter.Instance.transform.position;
            leftHandTargetPosition = playerPosition;
            rightHandTargetPosition = playerPosition;
            leftHandTargetPosition += Vector2.left * leftHand.swipeSetupDistanceToPlayer;
            rightHandTargetPosition += Vector2.right * rightHand.swipeSetupDistanceToPlayer;
        }

        //TODO Indicateur visuel
        yield return new WaitForSeconds(delayBeforeClap);

        PlayClapTravelSound();

        //léger recul
        leftHandTargetPosition += Vector2.left * 2;
        rightHandTargetPosition += Vector2.right * 2;

        while (leftHandTargetPosition.Distance(leftHand.transform.position) > 0.05f || rightHandTargetPosition.Distance(rightHand.transform.position) > 0.05f)
        {
            leftHand.transform.position = Vector2.MoveTowards(leftHand.transform.position, leftHandTargetPosition, 20f * Time.deltaTime);
            rightHand.transform.position = Vector2.MoveTowards(rightHand.transform.position, rightHandTargetPosition, 20f * Time.deltaTime);
            yield return null;
        }

        leftClapHandColliderDamage.ActivateDamage();
        rightClapHandColliderDamage.ActivateDamage();
        yield return new WaitForFixedUpdate();

        //clap
        float lastClapParticleSpawnTimeStamp = 0;

        leftHandTargetPosition += Vector2.right * leftHand.swipeSetupDistanceToPlayer + Vector2.right * 2 + Vector2.left * clapOffsetPosition;
        rightHandTargetPosition += Vector2.left * rightHand.swipeSetupDistanceToPlayer + Vector2.left * 2 + Vector2.right * clapOffsetPosition;
        while (leftHandTargetPosition.Distance(leftHand.transform.position) > 0.05f || rightHandTargetPosition.Distance(rightHand.transform.position) > 0.05f)
        {
            if (lastClapParticleSpawnTimeStamp > clapParticleSpawnInterval)
            {
                Instantiate(leftClapParticle, ComputeClapParticlePosition(leftHand.transform.position), Quaternion.identity);
                Instantiate(leftClapParticle, ComputeClapParticlePosition(leftHand.transform.position), Quaternion.identity);
                Instantiate(leftClapParticle, ComputeClapParticlePosition(leftHand.transform.position), Quaternion.identity);
                Instantiate(leftClapParticle, ComputeClapParticlePosition(leftHand.transform.position), Quaternion.identity);
                Instantiate(leftClapParticle, ComputeClapParticlePosition(leftHand.transform.position), Quaternion.identity);

                Instantiate(rightClapParticle, ComputeClapParticlePosition(rightHand.transform.position), Quaternion.identity);
                Instantiate(rightClapParticle, ComputeClapParticlePosition(rightHand.transform.position), Quaternion.identity);
                Instantiate(rightClapParticle, ComputeClapParticlePosition(rightHand.transform.position), Quaternion.identity);
                Instantiate(rightClapParticle, ComputeClapParticlePosition(rightHand.transform.position), Quaternion.identity);
                Instantiate(rightClapParticle, ComputeClapParticlePosition(rightHand.transform.position), Quaternion.identity);

                lastClapParticleSpawnTimeStamp = 0;
            }

            leftHand.rb.MovePosition(Vector2.MoveTowards(leftHand.rb.position, leftHandTargetPosition, clapSpeed * Time.deltaTime));
            rightHand.rb.MovePosition(Vector2.MoveTowards(rightHand.rb.position, rightHandTargetPosition, clapSpeed * Time.deltaTime));
            yield return new WaitForFixedUpdate();
            lastClapParticleSpawnTimeStamp += Time.deltaTime;
        }

        yield return new WaitForFixedUpdate();
        leftClapHandColliderDamage.DeactivateDamage();
        rightClapHandColliderDamage.DeactivateDamage();

        PlayClapImpactAnimationandSound();
        yield return new WaitForSeconds(delayAfterClap);

        //retourne au bon endroit
        leftHand.clapHand.SetActive(false);
        rightHand.clapHand.SetActive(false);
        leftHand.idleHand.SetActive(true);
        rightHand.idleHand.SetActive(true);

        leftHandTargetPosition = leftHandStartingPosition;
        rightHandTargetPosition = rightHandStartingPosition;

        while (leftHandTargetPosition.Distance(leftHand.transform.position) > 0.05f || rightHandTargetPosition.Distance(rightHand.transform.position) > 0.05f)
        {
            leftHand.transform.position = Vector2.MoveTowards(leftHand.transform.position, leftHandTargetPosition, leftHand.movingSpeed * Time.deltaTime);
            rightHand.transform.position = Vector2.MoveTowards(rightHand.transform.position, rightHandTargetPosition, rightHand.movingSpeed * Time.deltaTime);
            yield return null;
        }

        lastAttackTimeStamp = Time.time;
        isAttacking = false;
    }
    private Vector2 ComputeClapParticlePosition(Vector2 handPosition)
    {
        return handPosition + Random.insideUnitCircle.normalized * 0.75f + Vector2.down * 0.9f;
    }

    private void PlayClapTravelSound()
    {
        SFXManager.Instance.PlaySFX(clapWhoosh);
        SFXManager.Instance.PlaySFX(clapTravelSound);
    }

    private void PlayClapImpactAnimationandSound()
    {
        SFXManager.Instance.PlaySFX(clapImpact);
        Instantiate(clapImpactParticle, rightHand.transform.position + (leftHand.transform.position - rightHand.transform.position) / 2, Quaternion.identity);
    }

    private void ChangeBossStats(bool isLeftHand)
    {
        lastAttackTimeStamp = Time.time;

        if (isLeftHand)
        {
            leftHand.isAttacking = false;
            leftHand.StopAllCoroutines();
        }

        if (!isLeftHand)
        {
            rightHand.isAttacking = false;
            rightHand.StopAllCoroutines();
        }

        if (leftHand.isActiveAndEnabled == true || rightHand.isActiveAndEnabled == true)
        {
            PlayAngrySound();
            cdAfterAttacks += 1.5f;
        }

        if (leftHand.isActiveAndEnabled == false && rightHand.isActiveAndEnabled == false)
        {
            PlayAngrySound();
            GoToPhase2();
        }
    }

    private void GoToPhase2()
    {

    }

    public void PlayAngrySound()
    {
        headAnimator.Play("Action");
        lastAngrySoundTimeStamp = Time.time;
        SFXManager.Instance.PlaySFX(angrySound);
    }

    private void LookAtPlayer()
    {
        float playerDegreeToBoss = Tools.DirectionToDegree(MainCharacter.Instance.transform.position - transform.position);

        if (playerDegreeToBoss > -25 && playerDegreeToBoss < 90)
            skullSprite.sprite = lookFarRight;
        else if (playerDegreeToBoss <= -25 && playerDegreeToBoss > -60)
            skullSprite.sprite = lookRight;
        else if (playerDegreeToBoss <= -60 && playerDegreeToBoss > -120)
            skullSprite.sprite = lookDown;
        else if (playerDegreeToBoss <= -120 && playerDegreeToBoss > -155)
            skullSprite.sprite = lookLeft;
        else
            skullSprite.sprite = lookFarLeft;
    }

    private bool CanLook()
    {
        return Time.time - lastAngrySoundTimeStamp > 0.75f;
        //0.75f = durée de l'animation dans l'animator
    }

    private bool HandCanAttack()
    {
        if (leftHand.isActiveAndEnabled == false && rightHand.isActiveAndEnabled == false)
            return false;

        return leftHand.isAttacking == false && rightHand.isAttacking == false && isAttacking == false && Time.time - lastAttackTimeStamp > cdAfterAttacks && Time.time - activationTimeStamp > 3.0f;
    }

    public void RegenBoss()
    {
        isAttacking = false;

        leftHand.RegenHealth();
        rightHand.RegenHealth();

        leftClapHandColliderDamage.DeactivateDamage();
        rightClapHandColliderDamage.DeactivateDamage();

        gameObject.SetActive(false);
        //TODO : se regen lui meme sur phase 2
    }
}
