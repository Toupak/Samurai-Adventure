using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;


public class SliderUser : MonoBehaviour
{
    private Slider slider;
    public Image fill;
    public Image border;

    public void StartCoroutineUseSlider(Color32 color, float duration)
    {
        if (slider == null)
            slider = GetComponent<Slider>();

        StartCoroutine(UseSlider(color, duration));
    }

    private IEnumerator UseSlider(Color32 color, float duration)
    {
        float timer = duration;
        
        fill.color = color;
        slider.value = 0;

        while (timer >= 0.0f)
        {
            slider.value = Tools.NormalizeValue(timer, duration, 0);

            timer -= Time.deltaTime;
            yield return null;
        }

        yield return Tools.Fade(border, 0.1f, false);
        Destroy(gameObject);
    }
}
