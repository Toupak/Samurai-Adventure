using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossLock : Interactable
{
    public SpriteRenderer cantOpenPanel;
    public TextMeshPro cantOpenText;

    public GameObject barrierToUnlock;

    public AudioClip noKeySound;
    public AudioClip keySound;
    public AudioClip success;
    public GameObject halfKey;

    private Inventory inventory;
    private bool isReading;
    private bool hasBeenOpened;

    void Start()
    {
        inventory = MainCharacter.Instance.GetComponent<Inventory>();

        //Check si hasBeenOpened dans le save et si oui unlock la barriere

        OnTrigger.AddListener(TryOpenDoor);
    }

    private void TryOpenDoor()
    {
        if (inventory.GetKeyState() == (true, true))
            OpenDoor();
        else
            StartCantOpenDoorCoroutine();
    }

    private void OpenDoor()
    {
        if (hasBeenOpened == true)
            return;

        hasBeenOpened = true;
        //enregistre hasBeenOpened dans le start
        StartUnlockAnimation();
        barrierToUnlock.SetActive(false);
    }

    private void StartUnlockAnimation()
    {
        SFXManager.Instance.PlaySFX(keySound);
        SFXManager.Instance.PlaySFX(success);
        halfKey.SetActive(true);
    }

    private void StartCantOpenDoorCoroutine()
    {
        if (!isReading)
            StartCoroutine(CantOpenDoor());

        SFXManager.Instance.PlaySFX(noKeySound);
    }

    private IEnumerator CantOpenDoor()
    {
        isReading = true;

        cantOpenPanel.gameObject.SetActive(true);
        cantOpenText.gameObject.SetActive(true);
        StartCoroutine(Tools.Fade(cantOpenPanel, 1.0f, true));
        StartCoroutine(Tools.Fade(cantOpenText, 1.0f, true));

        yield return new WaitForSeconds(3.0f);

        StartCoroutine(Tools.Fade(cantOpenPanel, 1.0f, false));
        yield return Tools.Fade(cantOpenText, 1.0f, false);
        cantOpenPanel.gameObject.SetActive(false);
        cantOpenText.gameObject.SetActive(false);

        isReading = false;
    }
}
