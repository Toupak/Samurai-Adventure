using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialoguePanel : MonoBehaviour
{
    public static UnityEvent OnTriggerDialogue = new UnityEvent();

    public static DialoguePanel Instance;
    public GameObject dialogueManager;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterName;
    public Image characterImage;
    public Image buttonImage;
    public Image dialogueBox;
    public Image characterBox;
    public Image boxCharacterImage;

    public List<AudioClip> letterSounds;
    public float timeBetweenLetters;
    public float fadeDuration;

    [HideInInspector] public bool isReading;

    private Coroutine buttonBlink;

    private void Start()
    {
        Instance = this;
    }
    
    public void DisplayDialogue(DialogueData dialogueData)
    {
        if (isReading == false)
            StartCoroutine(CoroutineDisplayDialogue(dialogueData));
    }

    private IEnumerator CoroutineDisplayDialogue(DialogueData dialogueData)
    {
        isReading = true;
        OnTriggerDialogue.Invoke();

        EmptyContent();
        dialogueManager.SetActive(true);

        characterName.text = dialogueData.characterName;
        characterImage.sprite = dialogueData.characterImage;

        SFXManager.Instance.PlayRandomSFXAtLocation(dialogueData.characterSounds.ToArray(), null);

        yield return FadeUI(true);

        for (int i = 0; i < dialogueData.textContent.Count; i++)
        {
            yield return null;

            yield return DisplayLetters(dialogueData.textContent[i]);
            DisplayButton();

            while (!PlayerInput.GetInteractInput())
                yield return null;

            RemoveButton();
        }

        SFXManager.Instance.PlayRandomSFXAtLocation(dialogueData.characterSounds.ToArray(), null);

        yield return FadeUI(false);

        dialogueManager.SetActive(false);
        isReading = false;
    }

    private IEnumerator DisplayLetters(string textContent)
    {
        dialogueText.text = "";
        for (int i = 0; i < textContent.Length; i++)
        {
            SFXManager.Instance.PlayRandomSFXAtLocation(letterSounds.ToArray(), null, volume: 0.03f);

            dialogueText.text = $"{dialogueText.text}{textContent[i]}";
            yield return new WaitForSeconds(timeBetweenLetters);
        }
    }

    private IEnumerator FadeUI(bool state)
    {
        StartCoroutine(Tools.Fade(dialogueBox, fadeDuration, state));
        StartCoroutine(Tools.Fade(dialogueText, fadeDuration/2, state));
        StartCoroutine(Tools.Fade(characterBox, fadeDuration, state));
        StartCoroutine(Tools.Fade(characterName, fadeDuration/2, state));
        StartCoroutine(Tools.Fade(characterImage, fadeDuration, state));
        StartCoroutine(Tools.Fade(boxCharacterImage, fadeDuration, state));
        yield return new WaitForSeconds(fadeDuration);
    }

    private void EmptyContent()
    {
        characterName.text = string.Empty;
        dialogueText.text = string.Empty;
        characterImage.sprite = null;
        buttonImage.gameObject.SetActive(false);
    }

    private void DisplayButton()
    {
        buttonImage.gameObject.SetActive(true);
        buttonBlink = StartCoroutine(BlinkButton());
    }

    private void RemoveButton()
    {
        StopCoroutine(buttonBlink);
        StartCoroutine(Tools.Fade(buttonImage, 0.01f, false));
    }

    private IEnumerator BlinkButton()
    {
        while (isReading)
        {
            yield return Tools.Fade(buttonImage, fadeDuration, true);
            yield return Tools.Fade(buttonImage, fadeDuration, false);
        }
    }

    //Zone interactable autour de l'endroit
    //Bouton d'UI qui flotte qui on rentre dans la zone au dessus du personnage ?
}
