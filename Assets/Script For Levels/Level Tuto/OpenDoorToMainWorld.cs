using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorToMainWorld : MonoBehaviour
{
    public EnemyHealth enemy;
    public Collider2D invisibleWall;

    public AudioClip success;   

    void Start()
    {
        enemy.OnDeath.AddListener(OpenNextLevel);
    }

    private void OpenNextLevel()
    {
        invisibleWall.gameObject.SetActive(false);
        SFXManager.Instance.PlaySFX(success);
    }
}
