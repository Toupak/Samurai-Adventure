using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockedUntilFountainUsed : MonoBehaviour
{
    public HPFountain hpFountain;
    public Collider2D invisibleWall;

    void Start()
    {
        hpFountain.OnTrigger.AddListener(UnlockArea);
    }

    private void UnlockArea()
    {
        invisibleWall.gameObject.SetActive(false);
    }
}
