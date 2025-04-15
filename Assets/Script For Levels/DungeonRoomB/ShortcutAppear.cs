using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskTrigger : MonoBehaviour
{
    public Trigger obeliskTrigger;

    public GameObject metalDoor1;
    public GameObject metalDoor2;

    public AudioClip success;

    void Start()
    {
        obeliskTrigger.OnTrigger.AddListener(ShortcutAppear);
    }

    private void ShortcutAppear()
    {
        SFXManager.Instance.PlaySFX(success);
        metalDoor1.SetActive(false);
        metalDoor2.SetActive(false);
    }
}
