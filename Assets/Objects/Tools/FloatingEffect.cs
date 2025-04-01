using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FloatingEffect : MonoBehaviour
{
    public float floatingRange;
    public float speed;

    void Start()
    {
        StartCoroutine(Floating());
    }

    private IEnumerator Floating()
    {
        float startingPosition = transform.position.y; 

        while (gameObject != null)
        {
            float positionUp = startingPosition + floatingRange;
            float positionDown = startingPosition - floatingRange;

            while (transform.position.y < positionUp)
            {
                transform.position = transform.position + Vector3.up * (speed * Time.deltaTime);
                yield return null;
            }

            while (transform.position.y > positionDown)
            {
                transform.position = transform.position + Vector3.down * (speed * Time.deltaTime);
                yield return null;
            }

            yield return null;
        }
    }
}
// incrémenter les while
    

    // float de bas en haut -> position qui change
    // que ce soit smooth -> Change de façon normalisée

