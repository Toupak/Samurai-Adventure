using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;
using Unity.VisualScripting;
using static ColliderDamage;

public class SamuraiAttack : MonoBehaviour
{
    public static UnityEvent<Vector2, AttackType> OnHitEnemy = new UnityEvent<Vector2, AttackType>();
    public static UnityEvent OnPlayerAttack = new UnityEvent();

    public List<ColliderDamage> swordColliders;
    public List<ColliderDamage> swordColliders3;

    public float attackDuration;
    public float comboWindow;

    public float zoom;
    public float zoomDuration;

    private SamuraiMovement movement;
    private PlayerHealth playerHealth;
    private LockEnemies lockEnemies;
    private SamuraiFireball samuraiFireball;

    private float attackActivationWindow = 0.1f;
    private float lastAttackTimeStamp = 0f;

    [HideInInspector] public int whichCombo;

    private void Start()
    {
        movement = GetComponent<SamuraiMovement>();
        playerHealth = GetComponent<PlayerHealth>();
        lockEnemies = GetComponent<LockEnemies>();
        samuraiFireball = GetComponent<SamuraiFireball>();
    }

    private void Update()
    {
        if (playerHealth.IsDead)
            return;
        
        if (CanAttack() && PlayerInput.GetAttackInput())
            StartCoroutine(SelectAttack());
    }

    IEnumerator SelectAttack()
    {
        if (Time.time - lastAttackTimeStamp < comboWindow && whichCombo < 3)
            whichCombo += 1;
        else 
            whichCombo = 1;

        lastAttackTimeStamp = Time.time;
        OnPlayerAttack.Invoke();

        int directionIndex;
        if (!lockEnemies.IsTargeting())
            directionIndex = Tools.GetLastRecordedDirectionAsInt(movement.LastMovement);
        else
            directionIndex = Tools.GetLastRecordedDirectionAsInt(lockEnemies.GetTargetDirection());

        if (whichCombo < 3)
            yield return Attack1_2(directionIndex);
        else
            yield return Attack3(directionIndex);
    }

    IEnumerator Attack1_2(int directionIndex)
    {
        swordColliders[directionIndex].ActivateDamage();

        yield return new WaitForSeconds(attackActivationWindow);
        swordColliders[directionIndex].DeactivateDamage();

        yield return new WaitForSeconds(attackDuration - attackActivationWindow); 
    }
    
    IEnumerator Attack3(int directionIndex)
    {
        swordColliders3[directionIndex].ActivateDamage();
        CameraZoom.RequestCameraZoomForDuration.Invoke(zoom, zoomDuration);

        yield return new WaitForSeconds(attackActivationWindow);
        swordColliders3[directionIndex].DeactivateDamage();

        yield return new WaitForSeconds(attackDuration - attackActivationWindow);
    }

    public bool IsAttacking()
    {
        return Time.time > attackDuration && Time.time - lastAttackTimeStamp < attackDuration;
    }

    private bool CanAttack()
    {
        return Time.time - lastAttackTimeStamp > attackDuration && movement.IsBusy() == false && samuraiFireball.isCharging == false;
    }
}