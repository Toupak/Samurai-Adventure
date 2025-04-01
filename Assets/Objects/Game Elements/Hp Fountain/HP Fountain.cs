using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Serializable maybe
public class HPFountain : Interactable
{
    public AudioSource healSound;
    public Transform currentLevel;

    public GameObject healParticlePrefab;

    private PlayerHealth playerHealth;
    private HeartHealthBar heartHealthBar;
    private SamuraiMana samuraiMana;

    private void Start()
    {
        playerHealth = MainCharacter.Instance.GetComponent<PlayerHealth>();
        heartHealthBar = MainCharacter.Instance.GetComponent<HeartHealthBar>();
        samuraiMana = MainCharacter.Instance.GetComponent<SamuraiMana>();

        CameraConstrainer.OnPlayerRespawnPingPosition.AddListener(CheckForSwitchCamera);

        OnTrigger.AddListener(Heal);
        OnTrigger.AddListener(SaveCheckpoint);
    }

    private void SaveCheckpoint()
    {
        SaveManager.Instance.GetSaveData().currentFountainPosition = transform.position + new Vector3(0, -1f, 0);
        PlayerHealth.OnPlayerRespawn.Invoke();
        
        // écrire "Progress Saved"
        // effet visuel pour montrer la résurrection des mobs
    }

    private void Heal()
    {
        playerHealth.RestoreHealth(playerHealth.maxHearts);
        samuraiMana.RestoreMana(samuraiMana.maxMana);

        PlayHealSoundAndParticle();
    }

    private void PlayHealSoundAndParticle()
    {
        healSound.pitch = Random.Range(0.95f, 1.05f);
        healSound.Play();
        Instantiate(healParticlePrefab, MainCharacter.Instance.transform.position + Vector3.up * 0.2f, Quaternion.identity, MainCharacter.Instance.transform);
    }

    private void CheckForSwitchCamera(Vector3 currentFountainPosition)
    {
        if ((transform.position - currentFountainPosition).magnitude <= 2)
            CameraConstrainer.OnPlayerRespawnSwitchCamera.Invoke(currentLevel);
    }
}
