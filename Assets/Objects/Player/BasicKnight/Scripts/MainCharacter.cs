using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainCharacter : MonoBehaviour
{
    public static MainCharacter Instance;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Cursor.visible = false;
    }

    public void TeleportToNewScene(string newScene)
    {
        StartCoroutine(TeleportToNewSceneCoroutine(newScene));
    }

    IEnumerator TeleportToNewSceneCoroutine(string newScene)
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(newScene);
        yield return new WaitUntil(() => loading.isDone);
        transform.position = TeleporterReceiver.Instance.transform.position;
    }
}
