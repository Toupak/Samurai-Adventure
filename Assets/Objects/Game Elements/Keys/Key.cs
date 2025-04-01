using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    public enum KeyPart
    {
        FirstHalf,
        SecondHalf
    }
    
    public KeyPart part;

    public AudioSource pickUpSound;

    public float timeBeforePickUp;
    private float keySpawnTimeStamp;

    private void Start()
    {
        keySpawnTimeStamp = Time.time;
    }

    protected void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player") && CanBePickedUp())
            PickUpKey();
    }

    private void PickUpKey()
    {
        PickUpKeyAnimation();
        MainCharacter.Instance.GetComponent<Inventory>().SetDoorKey(part, true);
    }

    private void PickUpKeyAnimation()
    {
        pickUpSound.pitch = Random.Range(0.95f, 1.05f);
        pickUpSound.transform.SetParent(null);
        pickUpSound.Play();        
        Destroy(pickUpSound.gameObject, 2.0f);

        Destroy(gameObject);
    }

    private bool CanBePickedUp()
    {
        return Time.time - keySpawnTimeStamp > timeBeforePickUp;
    }
}