using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Teleporter : MonoBehaviour
{
    public Transform destination;
    public Transform targetLevel;
    public AudioSource sound;

    public static UnityEvent OnTakeTeleporter = new UnityEvent();

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            StartTP();
        }
    }

    private void StartTP()
    {
        OnTakeTeleporter.Invoke();
        sound.pitch = Random.Range(0.95f, 1.05f);
        sound.Play();
        MainCharacter.Instance.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y);
        TPScriptLDTK.OnPlayerTeleport.Invoke(targetLevel);
    }
}
