using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatScript : MonoBehaviour
{
    private Animator catAnim;
    private Rigidbody2D rb;
    public float maxAnimDuration;
    public float catSpeed;
    private float actualSpeed;
    private float animDuration;
    private float lastAnimTimeStamp = 0f;
    public List<string> catAnimList;

    void Start()
    {
        catAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Time.time - lastAnimTimeStamp >= animDuration)
        {
                PlayCatAnim();
                lastAnimTimeStamp = Time.time;
        }
    }

    void PlayCatAnim()
    {
        int whichCatAnim = Random.Range(0, catAnimList.Count);
        animDuration = Random.Range(2.0f, maxAnimDuration);        
        string animationName = catAnimList[whichCatAnim];

        if (whichCatAnim == 0)
        {
            MoveCat(ComputeMoveDirection(), catSpeed);
            catAnim.Play(animationName);
        }

        if (whichCatAnim == 1)
        {
            MoveCat(ComputeMoveDirection(), catSpeed*2);
            catAnim.Play(animationName);
        }

        if (whichCatAnim >= 2)
        {
            MoveCat(Vector2.zero, 0.0f);
            catAnim.Play(animationName);
        }    
    }

    private void MoveCat(Vector2 direction, float newSpeed)
    { 
        rb.velocity = direction * newSpeed;
        actualSpeed = newSpeed;

        if (rb.velocity.x > 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        if (rb.velocity.x < 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    }

    private Vector2 ComputeMoveDirection()
    {
        return Random.insideUnitCircle.normalized;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        MoveCat(other.contacts[0].normal, actualSpeed);
    }
}

// Bonus later : anim lick que quand tu lui parles
// Meow Sound - Meow sound quand il se fait taper - goumgoum quand tu le tape