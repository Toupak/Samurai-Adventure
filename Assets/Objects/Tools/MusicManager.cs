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

    //Critères déterminant musique : 
    //endroit ou on est : forêt / donjon / maison / ...
    //état : combat / non combat

    //Passage en mode combat trigger une transition
    //Passage dans des nouvelles zones sur des trigger qui changent la musique

    //Playlists qui jouent des sons random
    //Un seul et même music manager : tout passe par le même gameobject et on remplace l'audioclip : on en crée pas des nouveaux à chaque fois comme sur le SFX Manager
}
