using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestAltarAppear : MonoBehaviour
{
    public List<GameObject> altarTriggers;
    public List<GameObject> objectsToActivate;
    public List<GameObject> objectsToDeactivate;

    public AudioClip success;

    private int triggerCount;

    void Start()
    {
        foreach (GameObject altar in altarTriggers)
        {
            altar.GetComponent<Altar>().OnAltarTrigger.AddListener(CountOneTrigger);
        }
    }

    private void CountOneTrigger()
    {
        triggerCount += 1;

        if (triggerCount == altarTriggers.Count)
        {
            TriggerEvent();
            SFXManager.Instance.PlaySFX(success);
        }
    }

    private void TriggerEvent()
    {
        if (objectsToActivate != null)
        {
            foreach (GameObject objectToActivate in objectsToActivate)
            {
                objectToActivate.SetActive(true);
            }
        }

        if (objectsToDeactivate != null)
        {
            foreach (GameObject objectToDeactivate in objectsToDeactivate)
            {
                objectToDeactivate.SetActive(false);
            }
        }
    }
}
