using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static SkullBoss_Head;

public class SkullBoss_Phase2 : MonoBehaviour
{
    public Transform SkullHeadPivot;
    public Transform LeftEyePivot;
    public Transform RightEyePivot;

    public AudioClip laserSound;
    public AudioClip fireSound;

    public GameObject circularLaserPrefab;
    public int circularLaserCount;
    public float circularLaserAttackDuration;
    public float rotationSpeed;

    public GameObject laserTarget;
    public GameObject eyeParticlePrefab;
    public float laserAttackDuration;
    public float laserSpeed;
    public float laserOffset;

    public float cdAfterAttacks;

    private bool isAttacking;
    private float lastAttackTimeStamp;
    private float activationTimeStamp;

    void Start()
    {
        activationTimeStamp = Time.time;
        //se déplace au bon endroit
    }

    void Update()
    {
        if (CanAttack())
            SelectAttack();
    }

    private void SelectAttack()
    {
        StartCoroutine(CircularLaserAttack());
        return;

        int whichAttackRandomizer = Random.Range(0, 3);

        //3 types d'attaque
        if (whichAttackRandomizer == 0)
            return;
        else if (whichAttackRandomizer == 1)
            return;
        else
            return;

        //Rajouter un petit randomizer de PlayAngrySound pour qu'il le fasse de temps en temps sur une attaque
    }

    private IEnumerator LaserAttack()
    {
        isAttacking = true;

        GameObject laserCircle = Instantiate(laserTarget, LeftEyePivot.position, Quaternion.identity);
        GameObject laserLine = Instantiate(circularLaserPrefab, LeftEyePivot.position, Quaternion.identity);
        GameObject eyeParticle = Instantiate(eyeParticlePrefab, LeftEyePivot.position, Quaternion.identity);

        GameObject laserCircle2 = Instantiate(laserTarget, RightEyePivot.position, Quaternion.identity);
        GameObject laserLine2 = Instantiate(circularLaserPrefab, RightEyePivot.position, Quaternion.identity);
        GameObject eyeParticle2 = Instantiate(eyeParticlePrefab, RightEyePivot.position, Quaternion.identity);

        AudioSource laserLoop = SFXManager.Instance.PlaySFX(laserSound, loop: true, delay: 1.5f, volume: 0.04f);
        AudioSource fireLoop = SFXManager.Instance.PlaySFX(fireSound, loop: true, delay: 1.5f, volume: 0.03f);

        float timer = 0;
        while (timer < laserAttackDuration)
        {
            ChasePlayer(laserCircle);
            DrawLaserLine(laserLine, laserCircle.transform.position, LeftEyePivot.position);

            ChasePlayer(laserCircle2);
            DrawLaserLine(laserLine2, laserCircle2.transform.position, RightEyePivot.position);

            yield return null;
            timer += Time.deltaTime;
        }

        Destroy(laserLoop.gameObject);
        Destroy(fireLoop.gameObject);

        Destroy(laserLine, 0.1f);
        Destroy(laserCircle, 0.1f);
        Destroy(eyeParticle, 0.1f);

        Destroy(laserLine2, 0.1f);
        Destroy(laserCircle2, 0.1f);
        Destroy(eyeParticle2, 0.1f);

        lastAttackTimeStamp = Time.time;
        isAttacking = false;

        //Cercle qui part du boss et qui chase le joueur - ok
        //Trait "rayon laser" qui fait la distance entre le boss et le cercle
        //Faire partir un laser de chaque oeil
        //Activer les dégats du collider au bon moment
    }

    private void ChasePlayer(GameObject laserCircle)
    {
        Vector3 playerPosition = MainCharacter.Instance.transform.position;
        Vector3 circleDirection = (playerPosition - laserCircle.transform.position).normalized;

        laserCircle.transform.position += circleDirection * laserSpeed * Time.deltaTime;
    }

    private void DrawLaserLine(GameObject laserLine, Vector2 laserCircle, Vector2 origin)
    {
        Vector2 laserDirection = (laserCircle - origin).normalized;
        laserLine.transform.rotation = laserDirection.ToRotation();

        float laserDistance = (laserCircle - origin).magnitude + laserOffset;

        laserLine.transform.localScale = new Vector3(laserDistance / 25f, 1, 1);
    }

    private IEnumerator CircularLaserAttack()
    {
        isAttacking = true;

        float angleDelta = 360f / circularLaserCount;
        float currentAngle = 0f;

        List<Animator> circularLaserAnimators = new List<Animator>();

        AudioSource laserLoop = SFXManager.Instance.PlaySFX(laserSound, loop: true, delay: 1.5f, volume: 0.05f);

        for (int i = 0; i < circularLaserCount; i++)
        {
            GameObject circularLaser = Instantiate(circularLaserPrefab, SkullHeadPivot.position, Vector2.right.AddAngleToDirection(currentAngle).ToRotation(), SkullHeadPivot);
            circularLaserAnimators.Add(circularLaser.transform.GetChild(0).GetComponent<Animator>());
            currentAngle += angleDelta;
        }

        float timer = 0;
        float currentRotationAngle = 0f;
        bool isDespawning = false;

        while (timer < circularLaserAttackDuration + 1.0f) //1.0f le temps de détruire le gameObject au foreach en dessous
        {
            SkullHeadPivot.rotation = Vector2.right.AddAngleToDirection(currentRotationAngle).ToRotation();
            timer += Time.deltaTime;
            currentRotationAngle += rotationSpeed * Time.deltaTime;
            yield return null;

            if (timer > circularLaserAttackDuration && isDespawning == false)
            {
                isDespawning = true;
                Destroy(laserLoop.gameObject);
                foreach (Animator circularLaser in circularLaserAnimators)
                {
                    circularLaser.Play("despawn");
                    Destroy(circularLaser.transform.parent.gameObject, 0.75f);
                }
            }
        }

        yield return new WaitForSeconds(5.0f);

        lastAttackTimeStamp = Time.time;
        isAttacking = false;
    }

    private bool CanAttack()
    {
        return isAttacking == false && Time.time - lastAttackTimeStamp > cdAfterAttacks && Time.time - activationTimeStamp > 1.0f;
    }



    //1. Rayons laser qui te suit -- zone au sol qui te suit
    //2. Zone rouges au sol qui swipe l'arène ou faut se mettre au bon endroit pour les dodger
    //3. Attaque circulaire rayons lasers qui tournent - DONE
    //Obélisk qui stun / rendent vulnérables le boss - sinon invulnérable
}
