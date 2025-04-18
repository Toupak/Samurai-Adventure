using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoShitOnEnemyDeathTrigger : MonoBehaviour
{
    public List<GameObject> gameObjectsToActivate;
    public List<GameObject> gameObjectsToDeactivate;

    public EnemyDeathTrigger enemyDeathTrigger;

    public AudioClip success;

    void Start()
    {
        enemyDeathTrigger.OnTrigger.AddListener(ActivateGameObjects);
        enemyDeathTrigger.OnTrigger.AddListener(DeactivateGameObjects);

        enemyDeathTrigger.OnTrigger.AddListener(() => SFXManager.Instance.PlaySFX(success));
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
