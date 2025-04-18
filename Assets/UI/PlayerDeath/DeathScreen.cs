using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DeathScreen : MonoBehaviour
{
    public SpriteRenderer playerSpriteRenderer;
    public Image blackscreen;
    public Image aButton;
    public Image bButton;
    public Image bonfire;
    public TextMeshProUGUI gameoverText;
    public TextMeshProUGUI continueText;
    public TextMeshProUGUI quitText;
    public AudioClip gameOverMusic;
    public AudioClip buttonConfirm;

    public float fadeInDuration;
    public float rapidFadeInDuration;

    private bool areDeathButtonsDisplayed;

    private PlayerHealth playerHealth;

    private void Start()
    {
        playerHealth = MainCharacter.Instance.GetComponent<PlayerHealth>();

        PlayerHealth.OnPlayerDeath.AddListener((_) => StartCoroutine(PlayDeathScreen()));
    }

    private void Update()
    {
        if (areDeathButtonsDisplayed && PlayerInput.GetInteractInput())
        {
            StartCoroutine(ContinueGame());
            areDeathButtonsDisplayed = false;
        }

        if (areDeathButtonsDisplayed && PlayerInput.GetDashInput())
        {
            ExitGame();
            areDeathButtonsDisplayed = false;
        }
    }

    private IEnumerator PlayDeathScreen()
    {
        playerSpriteRenderer.sortingOrder = 1001;

        Time.timeScale = 0.05f;

        blackscreen.gameObject.SetActive(true);
        yield return Tools.Fade(blackscreen, fadeInDuration, true, scaledTime:false);

        StartCoroutine(PlayGameOverMusic());

        yield return Tools.Fade(playerSpriteRenderer, fadeInDuration, false, scaledTime:false);

        Time.timeScale = 1.0f;
        bonfire.gameObject.SetActive(true);
        yield return Tools.Fade(bonfire, fadeInDuration, true, scaledTime: false);

        gameoverText.gameObject.SetActive(true);
        yield return Tools.Fade(gameoverText, fadeInDuration, true, scaledTime: false);

        aButton.gameObject.SetActive(true);
        bButton.gameObject.SetActive(true);
        continueText.gameObject.SetActive(true);
        quitText.gameObject.SetActive(true);
        StartCoroutine(Tools.Fade(aButton, rapidFadeInDuration, true, scaledTime: false));
        StartCoroutine(Tools.Fade(bButton, rapidFadeInDuration, true, scaledTime: false));
        StartCoroutine(Tools.Fade(continueText, rapidFadeInDuration, true, scaledTime: false));
        yield return Tools.Fade(quitText, rapidFadeInDuration, true, scaledTime: false);

        areDeathButtonsDisplayed = true;
    }

    private IEnumerator ContinueGame()
    {
        SFXManager.Instance.PlaySFX(buttonConfirm);
        
        bonfire.gameObject.SetActive(false);
        
        StartCoroutine(Tools.Fade(aButton, fadeInDuration, false, scaledTime: false));
        StartCoroutine(Tools.Fade(bButton, fadeInDuration, false, scaledTime: false));
        StartCoroutine(Tools.Fade(continueText, fadeInDuration, false, scaledTime: false));
        StartCoroutine(Tools.Fade(quitText, fadeInDuration, false, scaledTime: false));
        yield return Tools.Fade(gameoverText, fadeInDuration, false, scaledTime: false);
        aButton.gameObject.SetActive(false);
        bButton.gameObject.SetActive(false);
        gameoverText.gameObject.SetActive(false);
        continueText.gameObject.SetActive(false);
        quitText.gameObject.SetActive(false);

        //respawn a la derniere position sauvegardée - respawn les ennemis
        MainCharacter.Instance.transform.position = SaveManager.Instance.GetSaveData().currentFountainPosition;
        CameraConstrainer.OnPlayerRespawnPingPosition.Invoke(SaveManager.Instance.GetSaveData().currentFountainPosition);

        yield return Tools.Fade(playerSpriteRenderer, fadeInDuration, true, scaledTime: false);

        yield return Tools.Fade(blackscreen, fadeInDuration, false, scaledTime: false);
        blackscreen.gameObject.SetActive(false);

        playerSpriteRenderer.sortingOrder = 0;
        
        playerHealth.Respawn();
    }

    private IEnumerator PlayGameOverMusic()
    {
        float volumeTemp = 0.01f;

        AudioSource gameOverMusicTemp = SFXManager.Instance.PlaySFX(gameOverMusic, volumeTemp);

        while (gameOverMusicTemp != null)
        {
            volumeTemp = volumeTemp + 0.02f;
            gameOverMusicTemp.volume = volumeTemp;
            yield return new WaitForSecondsRealtime(0.08f);
        }
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
