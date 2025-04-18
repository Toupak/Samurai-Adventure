using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGOonTrigger : MonoBehaviour
{
    public List<GameObject> gameObjectsToActivate;
    public List<GameObject> gameObjectsToDeactivate;
    public Trigger trigger;

    void Start()
    {
        trigger.GetComponent<Trigger>().OnTrigger.AddListener(ActivateGameObjects);
        trigger.GetComponent<Trigger>().OnTrigger.AddListener(DeactivateGameObjects);
    }

    private void ActivateGameObjects()
    {
        foreach (GameObject GO in gameObjectsToActivate)
        {
            GO.SetActive(true);
        }
    }

    private void DeactivateGameObjects()
    {
        foreach (GameObject GO in gameObjectsToDeactivate)
        {
            GO.SetActive(false);
        }
    }
}
