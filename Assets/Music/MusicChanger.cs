using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour
{
    public MusicManager.Musics newMusic;

    protected void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.transform.CompareTag("Player"))
            MusicManager.OnTriggerMusic.Invoke(newMusic);
    }
}
