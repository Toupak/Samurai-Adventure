using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBossTrigger : Trigger
{
    public GameObject boss;
    public GameObject invisibleWallOutofRoom;

    public AudioClip bossMusic1;

    private AudioSource currentMusic;
    private bool hasBeenTriggered;
    private bool hasBeenKilled;

    private void Start()
    {
        PlayerHealth.OnPlayerDeath.AddListener((_) => ResetBoss());
        PlayerHealth.OnPlayerRespawn.AddListener(() => hasBeenTriggered = false);

        //boss.GetComponent<EnemyHealth>().OnDeath.AddListener(OnKillBoss);
    }

    void Update()
    {
        if (isWithinRange == true && hasBeenTriggered == false)
            TriggerBoss();
    }

    private void TriggerBoss()
    {
        hasBeenTriggered = true;
        boss.SetActive(true);
        invisibleWallOutofRoom.SetActive(true);
        currentMusic = SFXManager.Instance.PlaySFXNoPitchModifier(bossMusic1, volume: 0.07f, loop: true);
    }

    private void ResetBoss()
    {
        if (hasBeenTriggered == true && hasBeenKilled == false)
        {
            boss.SetActive(false);
            invisibleWallOutofRoom.SetActive(false);
            boss.GetComponent<SkullBoss_Head>().RegenBoss();
            if (currentMusic.isPlaying == true)
            {
                currentMusic.Stop();
                Destroy(currentMusic.gameObject);
            }
        }
    }

    private void OnKillBoss()
    {
        hasBeenKilled = true;
    }
    //check s'il a été tué
    //full regen remets starting position et desactive 
}
