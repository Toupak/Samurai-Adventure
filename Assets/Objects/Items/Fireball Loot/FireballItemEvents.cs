using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballItemEvents : MonoBehaviour
{
    public List<GameObject> gameObjectsToActivate;
    public List<GameObject> gameObjectsToDeactivateOnSave;
    public List<EnemyHealth> enemyHealths;

    private IEnumerator Start()
    {
        FireballItemLoot.OnLootFireball.AddListener(DisplayInstructions);

        yield return null;
        DisplayInstructionsWithSave();
    }

    private void DisplayInstructions()
    {
        foreach (GameObject go in gameObjectsToActivate)
        {
            go.SetActive(true);
        }
    }
    private void DisplayInstructionsWithSave()
    {
        if (SaveManager.Instance.GetSaveData().hasLootedFireball == true)
        {
            foreach (GameObject go in gameObjectsToActivate)
            {
                go.SetActive(true);
            }

            foreach (GameObject go in gameObjectsToDeactivateOnSave)
            { 
                go.SetActive(false); 
            }
            foreach (EnemyHealth enemy in enemyHealths)
            {
                enemy.gameObject.SetActive(true);
                enemy.cantRespawn = true;
                enemy.gameObject.SetActive(false);
            }
        }
    }
}
