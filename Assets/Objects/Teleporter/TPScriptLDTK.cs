using LDtkUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TPScriptLDTK : MonoBehaviour
{
    [HideInInspector] public static UnityEvent<Transform> OnPlayerTeleport = new UnityEvent<Transform>();

    private Transform targetPosition;
    private Transform targetLevel;

    private void Start()
    {
        LDtkReferenceToAnEntityInstance referenceToNextDoor = GetComponent<LDtkFields>().GetEntityReference("TargetDoor");

        if (referenceToNextDoor.GetEntity() == null)
        {
            targetPosition = TeleporterReceiver.Instance.transform;
            targetLevel = TeleporterReceiver.Instance.transform;
        }
        else
        {
            targetPosition = referenceToNextDoor.GetEntity().transform.GetChild(1).transform;
            targetLevel = referenceToNextDoor.GetLevel().transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
            TeleportPlayerToNextDoor();
    }

    private void TeleportPlayerToNextDoor()
    {
        MainCharacter.Instance.transform.position = targetPosition.position;
        OnPlayerTeleport.Invoke(targetLevel);
    }
}
