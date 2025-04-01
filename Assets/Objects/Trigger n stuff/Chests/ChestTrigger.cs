using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ChestTrigger : Interactable
{
    [HideInInspector] public UnityEvent OnLootAppear = new UnityEvent();

    public AudioSource chestSound;
    public AudioClip error;
    public GameObject item;

    private Animator chestAnimator;
    private Squeeze squeeze;

    public EnemyDeathTrigger enemyDeathTrigger;

    private bool hasBeenLooted;
    private bool canOpenChest = true;

    void Start()
    {
        chestAnimator = GetComponent<Animator>();
        squeeze = GetComponent<Squeeze>();
        OnTrigger.AddListener(LootChest);

        if (enemyDeathTrigger != null)
        {
            canOpenChest = false;
            enemyDeathTrigger.OnTrigger.AddListener(() => canOpenChest = true);
        }
    }

    private void LootChest()
    {
        if (!canOpenChest)
        {
            SFXManager.Instance.PlaySFX(error);
            return;
        }

        ChestAnimation();
        if (squeeze != null)
            squeeze.Trigger();

        if (item != null && hasBeenLooted == false)
        {
            SpawnItem();
            OnLootAppear.Invoke();
            hasBeenLooted = true;
        }
    }

    protected void ChestAnimation()
    {
        chestAnimator.Play("Chest_open");
        chestSound.pitch = Random.Range(0.95f, 1.05f);
        chestSound.Play();
    }

    private void SpawnItem()
    {
        Instantiate(item, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }
}