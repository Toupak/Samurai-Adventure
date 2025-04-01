using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BlinkBeforeAttack : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color blinkColor;
    public float duration;
    public int blinkCount;

    void Start()
    {
        GetComponent<OLDEnemyController>().OnAttack.AddListener(BlinkBeforeHitting);
    }

    void BlinkBeforeHitting()
    {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine()
    {
        Color OGColor = spriteRenderer.color;

        int index = 0;
        while (index <= blinkCount)
        {
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = OGColor;
            yield return new WaitForSeconds(duration);
            index++;
        }
    }
}