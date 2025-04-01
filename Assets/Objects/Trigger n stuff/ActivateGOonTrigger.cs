using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateGOonTrigger : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public GameObject trigger;

    void Start()
    {
        trigger.GetComponent<Trigger>().OnTrigger.AddListener(ActivateGameObjects);
    }

    private void ActivateGameObjects()
    {
        foreach (GameObject GO in gameObjects)
        {
            GO.SetActive(true);
        }
    }
}
