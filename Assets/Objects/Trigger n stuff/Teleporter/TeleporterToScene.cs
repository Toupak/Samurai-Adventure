using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleporterToScene : MonoBehaviour
{
    public string newScene;

    private void OnTriggerStay2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
        {
            MainCharacter.Instance.TeleportToNewScene(newScene);
        }
    }
}
