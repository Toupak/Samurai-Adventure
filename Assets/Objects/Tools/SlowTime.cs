using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour
{
    public float freezeEnemyDuration;
    private float freezePlayerDuration = 0.2f;

    void Start()
    {
        BubbleShield.OnPlayerShieldDamage.AddListener((_,_) => StartCoroutine(SlowPlayer()));
        BubbleShield.OnPlayerParry.AddListener((_) => StartCoroutine(SlowPlayer()));
        BubbleShield.OnPlayerShieldDestroy.AddListener((_,_) => StartCoroutine(SlowPlayer()));
        PlayerHealth.OnPlayerTakeDamage.AddListener((_, _) => StartCoroutine(SlowPlayer()));
        SamuraiAttack.OnHitEnemy.AddListener((_,_) => StartCoroutine(SlowEnemy()));
    }

    private IEnumerator SlowPlayer()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(freezePlayerDuration);
        Time.timeScale = 1f;
    }

    private IEnumerator SlowEnemy()
    {
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(freezeEnemyDuration);
        Time.timeScale = 1f;
    }
}
