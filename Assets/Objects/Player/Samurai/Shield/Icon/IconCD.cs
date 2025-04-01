using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IconCD : MonoBehaviour
{
    public Image shieldCDFill;
    public Image dashCDFill;

    public GameObject shieldIcon;

    private BubbleShield shield;
    private SamuraiMovement movement;

    private IEnumerator Start()
    {
        shield = MainCharacter.Instance.GetComponent<BubbleShield>();
        movement = MainCharacter.Instance.GetComponent<SamuraiMovement>();

        shieldCDFill.fillAmount = 0;
        BubbleShield.OnPlayerShield.AddListener(() => StartCoroutine(StartShieldCD()));
        SamuraiMovement.OnPlayerDash.AddListener(() => StartCoroutine(StartDashCD()));

        yield return null;
        if (shield.hasLootedShield == false)
            shieldIcon.SetActive(false);

        ShieldItemLoot.OnLootShield.AddListener(ActivateUI);
    }

    private IEnumerator StartShieldCD()
    {
        shieldCDFill.fillAmount = 1;

        float timer = 0f;

        while (timer <= shield.shieldCD)
        {
            shieldCDFill.fillAmount = 1 - Tools.NormalizeValue(timer, 0, shield.shieldCD);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        shieldCDFill.fillAmount = 0;
    }
    private IEnumerator StartDashCD()
    {
        dashCDFill.fillAmount = 1;

        float timer = 0f;

        while (timer <= movement.cooldownDash)
        {
            dashCDFill.fillAmount = 1 - Tools.NormalizeValue(timer, 0, movement.cooldownDash);
            yield return null;
            timer += Time.unscaledDeltaTime;
        }

        dashCDFill.fillAmount = 0;
    }

    private void ActivateUI()
    {
        shieldIcon.SetActive(true);
    }
}