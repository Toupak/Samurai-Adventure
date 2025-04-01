using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

public class SawTrap : MonoBehaviour
{
    private ColliderDamage sawColliderDamage;
    public Animator animator;
    public AudioSource sawSound;
    public Transform sawTransform;
    public Trigger triggerActivate;
    public Trigger triggerDeactivate;
    public List<Transform> pathPoints;

    private bool hasHit;
    private bool isActivated;   
    private bool isCancelled;   //
    private bool isGoingForward = true; //

    private int currentPoint;   //
    private float initialVolume;
    public float damage;
    public float delayBeforeBouncing;   //
    public float speed;

    void Start()
    {
        sawColliderDamage = transform.GetChild(1).GetComponent<ColliderDamage>();
        initialVolume = sawSound.volume;
        sawColliderDamage.DeactivateDamage();

        triggerActivate.OnTrigger.AddListener(() => StartCoroutine(Activate()));
        triggerDeactivate.OnTrigger.AddListener(() => isCancelled = true);
    }

    private IEnumerator Activate()
    {
        if (isActivated == true)
            yield break;

        isActivated = true;
        SawAnimation();
        sawColliderDamage.ActivateDamage();

        while (isCancelled == false)
        {
            yield return MoveToNextPoint();
        }

        DeactivateAnimation();
        sawColliderDamage.DeactivateDamage();
        isActivated = false;
        isCancelled = false;
    }

    IEnumerator MoveToNextPoint()
    {
        if (currentPoint >= pathPoints.Count - 1)
        {
            isGoingForward = false;
            yield return new WaitForSeconds(delayBeforeBouncing);
        }

        if (currentPoint <= 0 && isGoingForward == false)
        {
            isGoingForward = true;
            currentPoint = 0;
            yield return new WaitForSeconds(delayBeforeBouncing);
        }

        Vector2 startPosition = pathPoints[currentPoint].position;
        Vector2 nextPosition = pathPoints[isGoingForward ? currentPoint + 1 : currentPoint - 1].position;

        Vector2 direction = (nextPosition - startPosition).normalized;

        while (Vector2.Distance(nextPosition, sawTransform.position) > 0.05f)
        {
            sawTransform.position = sawTransform.position + (Vector3)direction * (speed * Time.deltaTime);
            yield return null;
        }
        currentPoint = isGoingForward ? currentPoint + 1 : currentPoint - 1;
    }

    private void SawAnimation()
    {
        animator.Play("Moving");
        sawSound.pitch = Random.Range(0.95f, 1.05f);
        sawSound.volume = Random.Range(initialVolume - 0.02f, initialVolume + 0.02f);
        sawSound.Play();
    }

    private void DeactivateAnimation()
    {
        animator.Play("Deactivated");
        sawSound.Stop();
    }
}
