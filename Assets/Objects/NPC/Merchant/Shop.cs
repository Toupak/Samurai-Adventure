using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : Interactable
{
    //ref au canvas pour l'ouvrir sur interactable
    //Freeze le personnage & curseur qui se déplace dans le shop
    //Buy items si le bon montant d'argent
    //Active les items quand buy - Event
    //Close le shop sur B

    //référence sur les 3 curseurs

    public GameObject shopScreen;

    [HideInInspector] public bool isShopping;

    public List<GameObject> shopItems;
    private int currentCursor;

    //fleche droite +1 avec un clamp
    //fleche gauche -1 avec un clamp
    //prends les 3 curseurs, celui au bon index s'active et les deux autres non
    //Liste avec les curseurs

    //for sur la liste a chaque input

    //Quand j'appuie sur A regarde la valeur de l'index
    //Update la liste quand on achète un item pour retirer un élément

    //Clamp avec 0 et .ToCount de la liste 

    void Start()
    {
        OnTrigger.AddListener(OpenShop);
    }

    protected override void Update()
    {
        base.Update();

        if (isShopping == false)
            return;

        //regarde les input et déplace le cursor droite/gauche + close le shop sur B + buy stuff sur A
    }

    private void OpenShop()
    {
        shopScreen.SetActive(true);
        isShopping = true;
    }

    private void CloseShop()
    {
        shopScreen.SetActive(false);
        isShopping = false;
    }
}
