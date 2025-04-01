using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleporterReceiver : MonoBehaviour
{
    public static TeleporterReceiver Instance;

    void Awake()
    {
        Instance = this;
    }
}
