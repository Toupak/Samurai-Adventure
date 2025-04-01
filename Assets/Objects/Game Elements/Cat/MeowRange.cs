using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeowRange : MonoBehaviour
{
    public bool isInRange;

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            isInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            isInRange = false;
        }
    }
}
