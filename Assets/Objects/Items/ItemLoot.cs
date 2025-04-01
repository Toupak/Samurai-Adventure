using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemLoot : MonoBehaviour
{
    public AudioClip pickUpSound;
    public float timeBeforePickUp;

    private float itemSpawnTimeStamp;
    private bool hasBeenLooted;

    private void Start()
    {
        itemSpawnTimeStamp = Time.time;
    }

    protected void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && CanBePickedUp() && hasBeenLooted == false)
        {
            hasBeenLooted = true;
            PickUpItem();
        }
    }

    protected virtual void PickUpItem()
    {
        LockPlayerMomentarily();
        PickUpItemAnimation();
        PickUpItemEffect();
    }

    protected virtual void PickUpItemAnimation()
    {
        SFXManager.Instance.PlaySFX(pickUpSound);
        Destroy(gameObject, 0.2f);
    }

    private void LockPlayerMomentarily()
    {
        SamuraiMovement movement = MainCharacter.Instance.GetComponent<SamuraiMovement>();

        movement.StopMovement();
        movement.lastMovement = Vector2.down;

        //pour que IsBusy() = true
        movement.lastDashTimeStamp = Time.time + 3.0f - movement.dashDuration;
    }

    protected virtual void PickUpItemEffect()
    {

    }

    private bool CanBePickedUp()
    {
        return Time.time - itemSpawnTimeStamp > timeBeforePickUp;
    }
}
