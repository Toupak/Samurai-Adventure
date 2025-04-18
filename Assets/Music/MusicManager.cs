using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicManager : MonoBehaviour
{
    public static UnityEvent<Musics> OnTriggerMusic = new UnityEvent<Musics>();
    public enum Musics
    {
        TutoBeginning,
        Forest,
        Dungeon,
        Boss
    }

    public AudioClip TutoBeginning;
    public AudioClip Forest;
    public AudioClip Dungeon;
    public AudioClip Boss;

    private Musics currentMusicEnum;
    private AudioSource currentMusic;

    void Start()
    {
        OnTriggerMusic.AddListener(ChangeMusic);

        PlayerHealth.OnPlayerDeath.AddListener((_) => PauseMusic());
        PlayerHealth.OnPlayerRespawn.AddListener(UnPauseMusic);
    }

    private void ChangeMusic(Musics newMusic)
    {
        if (newMusic == currentMusicEnum)
            return;

        currentMusicEnum = newMusic;

        if (currentMusic != null && currentMusic.isPlaying == true)
            StopMusic();
        
        currentMusic = SFXManager.Instance.PlaySFX(GetClipFromEnum(newMusic), loop: true);
    }

    private AudioClip GetClipFromEnum(Musics newMusic)
    {
        switch (newMusic)
        {
            case Musics.TutoBeginning:
                return TutoBeginning;
            case Musics.Forest:
                return Forest;
            case Musics.Dungeon:
                return Dungeon;
            case Musics.Boss:
                return Boss;
            default: return null;
        }
    }

    private void StopMusic()
    {
        currentMusic.Stop();
        Destroy(currentMusic.gameObject);
    }

    private void PauseMusic()
    {
        currentMusic.volume = 0;
    }

    private void UnPauseMusic()
    {
        currentMusic.volume = 0.1f;
    }

    //Quand on reload le jeu : sauvegarde la musique actuelle et la rejoue au respawn ?

    //Coupe la musique quand on die
    //Remets une autre musique quand on respawn après un petit temps

    //Zones au sol colliderTrigger pour switch les musiques quand on rentre dans une nouvelle zone

    //Fontaines doivent prendre en compte les locations -> Liste endroits qui trigger la musique qui va loop ?

    //Change la musique que si elle est différente de la musique
}
