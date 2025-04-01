using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    public GameObject sliderPrefab;

    private Color32 shieldCDcolor = new Color32 (42, 110, 226, 255);
    private Color32 dashCDcolor = new Color32(25, 185, 28, 255);

    private BubbleShield shield;
    private SamuraiMovement movement;

    private void Start()
    {
        shield = MainCharacter.Instance.GetComponent<BubbleShield>();
        movement = MainCharacter.Instance.GetComponent<SamuraiMovement>();

        BubbleShield.OnPlayerShield.AddListener(() => CreateNewSlider(shieldCDcolor, shield.shieldCD));
        SamuraiMovement.OnPlayerDash.AddListener(() => CreateNewSlider(dashCDcolor, movement.cooldownDash));
    }

    private void CreateNewSlider(Color32 color, float timerDuration)
    {
        GameObject sliderTemp = Instantiate(sliderPrefab);
        sliderTemp.transform.SetParent(transform);

        sliderTemp.GetComponent<SliderUser>().StartCoroutineUseSlider(color, timerDuration);
    }
}