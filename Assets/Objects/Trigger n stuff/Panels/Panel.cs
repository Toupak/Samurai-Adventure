using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Linq;

public class Panel : Interactable
{
    public GameObject textbox;
    public SpriteRenderer spriteTextbox;
    public SpriteRenderer shadowTextbox;
    public AudioSource open;
    public AudioSource close;
    public AudioSource writingText;
    public TextMeshPro textMesh;
    [TextArea] public string textMeshContent;

    public float timing;
    public float fadeDuration;
    private float initialOpenVolume;
    private float initialCloseVolume;
    private float initialWritingTextVolume;
    private bool isBeingRead;

    void Start()
    {
        textbox.SetActive(false);
        initialOpenVolume = open.volume;
        initialCloseVolume = close.volume;
        initialWritingTextVolume = writingText.volume;

        OnTrigger.AddListener(SetPanelState);
    }

    protected override void Update()
    {
        base.Update();
        
        if (isWithinRange == false && isBeingRead == true)
        {
            StopAllCoroutines();
            StartCoroutine(ClosePanel());
        }
    }

    private void SetPanelState()
    {
        StopAllCoroutines();

        if (isBeingRead)
            StartCoroutine(ClosePanel());
        else
            StartCoroutine(DisplayPanel());
    }

    private IEnumerator DisplayPanel()
    {
        isBeingRead = true;

        textbox.SetActive(true);
        textMesh.gameObject.SetActive(false);
        

        open.pitch = Random.Range(0.95f, 1.05f);
        open.volume = Random.Range(initialOpenVolume - 0.02f, initialOpenVolume + 0.02f);
        open.Play();

        StartCoroutine(Tools.Fade(spriteTextbox, fadeDuration, true));
        StartCoroutine(Tools.Fade(shadowTextbox, fadeDuration, true, 0.7f));
        yield return new WaitForSeconds (fadeDuration);

        textMesh.gameObject.SetActive(true);
        StartCoroutine(Tools.Fade(textMesh, 0.0f, true));

        string textContent = textMeshContent;
        
        textMesh.text = "";
        for (int i = 0; i < textContent.Length; i++)
        {
            writingText.pitch = Random.Range(0.95f, 1.05f);
            writingText.volume = Random.Range(initialWritingTextVolume - 0.02f, initialWritingTextVolume + 0.02f);
            writingText.Play();

            textMesh.text = $"{textMesh.text}{textContent[i]}";
            yield return new WaitForSeconds(timing);
        }
    }

    private IEnumerator ClosePanel()
    {
        isBeingRead = false;

        close.pitch = Random.Range(0.95f, 1.05f);
        close.volume = Random.Range(initialCloseVolume - 0.02f, initialCloseVolume + 0.02f);
        close.Play();

        StartCoroutine(Tools.Fade(textMesh, fadeDuration, false));
        yield return new WaitForSeconds (fadeDuration);

        StartCoroutine(Tools.Fade(spriteTextbox, fadeDuration, false, maxAlpha: spriteTextbox.color.a));
        StartCoroutine(Tools.Fade(shadowTextbox, fadeDuration, false, 0.7f));
        yield return new WaitForSeconds(fadeDuration);

        textbox.SetActive(false);
    }

    //Cancel all coroutine avec un bruit de cancel de panel quand on se barre
    //Accrocher le panel au Canvas qui ne bouge pas tant qu'on a pas appuyé sur Y pour annuler
}
