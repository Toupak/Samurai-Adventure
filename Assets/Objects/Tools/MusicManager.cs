using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    //Crit�res d�terminant musique : 
    //endroit ou on est : for�t / donjon / maison / ...
    //�tat : combat / non combat

    //Passage en mode combat trigger une transition
    //Passage dans des nouvelles zones sur des trigger qui changent la musique

    //Playlists qui jouent des sons random
    //Un seul et m�me music manager : tout passe par le m�me gameobject et on remplace l'audioclip : on en cr�e pas des nouveaux � chaque fois comme sur le SFX Manager
}
