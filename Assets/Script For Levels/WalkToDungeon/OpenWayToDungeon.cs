using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWayToDungeon : MonoBehaviour
{
    public GameObject roadBlock;
    public AudioClip success;

    public EnemyDeathTrigger triggerOpenLevel;
    
    void Start()
    {
        triggerOpenLevel.OnTrigger.AddListener(OpenNextLevel);
    }

    private void OpenNextLevel()
    {
        roadBlock.SetActive(false);
        SFXManager.Instance.PlaySFX(success);
    }
}
