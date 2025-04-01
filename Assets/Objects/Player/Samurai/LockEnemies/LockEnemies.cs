using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LockEnemies : MonoBehaviour
{
    public GameObject targetIconPrefab;
    public float lockRange;

    private PlayerHealth playerHealth;

    private Vector3 offsetToCenter = new Vector3(0.0f, 0.3f, 0.0f);

    private bool wasJoystickReleased = true;

    private GameObject currentTarget;
    private GameObject currentTargetIcon;

    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    void Update()
    {
        if (playerHealth.IsDead)
            return;

        if (!IsTargeting() && PlayerInput.GetLockInput())
            Lock(LookForClosestEnemyInLockRange());

        if (IsTargeting() && !PlayerInput.GetLockInput())
            Unlock();

        if (wasJoystickReleased && IsTargeting() && PlayerInput.ComputeLookDirection().magnitude > 0.1f)
        {
            Lock(ChangeTarget());
            wasJoystickReleased = false;
        }

        if (wasJoystickReleased == false && PlayerInput.ComputeLookDirection().magnitude < 0.1f)
            wasJoystickReleased = true;
    }

    private void Lock(GameObject newTarget)
    {
        currentTarget = newTarget;

        if (currentTarget != null && currentTargetIcon == null)
            currentTargetIcon = Instantiate(targetIconPrefab, currentTarget.transform.position + offsetToCenter, Quaternion.identity);

        if (currentTarget != null && currentTargetIcon != null)
        {
            currentTargetIcon.transform.position = currentTarget.transform.position + offsetToCenter;
            currentTargetIcon.transform.SetParent(currentTarget.transform);
        }
    }

    private void Unlock()
    {
        currentTarget = null;
        
        if (currentTargetIcon != null)
            Destroy(currentTargetIcon);
    }

    private GameObject LookForClosestEnemyInLockRange()
    {
        RaycastHit2D[] enemiesInLockRange = Physics2D.CircleCastAll(transform.position +  offsetToCenter, lockRange, Vector2.zero, 0, LayerMask.GetMask("EnemyHurtbox"));

        float minimumValue = float.MaxValue;
        int minimumIndex = -1;

        for (int i = 0; i < enemiesInLockRange.Length; i++)
        {
            GameObject enemy = enemiesInLockRange[i].rigidbody.gameObject;
            float distance = (enemy.transform.position - transform.position).magnitude;

            if (distance < minimumValue)
            {
                minimumValue = distance;
                minimumIndex = i;
            }
        }

        if (minimumIndex != -1)
            return enemiesInLockRange[minimumIndex].rigidbody.gameObject;
        else
            return null;
    }

    private GameObject ChangeTarget()
    {
        RaycastHit2D[] enemiesInLockRange = Physics2D.CircleCastAll(transform.position + offsetToCenter, lockRange, Vector2.zero, 0, LayerMask.GetMask("EnemyHurtbox"));

        Vector2 currentTargetDirection = GetTargetDirection();
        Vector2 currentLookDirection = PlayerInput.ComputeLookDirection();
        float minimumValue = float.MaxValue;
        int minimumIndex = -1;

        for (int i = 0; i < enemiesInLockRange.Length; i++)
        {
            GameObject enemy = enemiesInLockRange[i].rigidbody.gameObject;
            
            if (enemy == currentTarget)
                continue;

            Vector2 direction = (enemy.transform.position - transform.position).normalized;

            float angleBetweenTargets = Vector2.SignedAngle(currentTargetDirection, direction);

            if (Mathf.Sign(angleBetweenTargets) == Mathf.Sign(currentLookDirection.x))
                continue;

            float absoluteValueAngleBetweenTargets = Mathf.Abs(angleBetweenTargets);

            if (absoluteValueAngleBetweenTargets < minimumValue)
            {
                minimumValue = absoluteValueAngleBetweenTargets;
                minimumIndex = i;
            }
        }

        if (minimumIndex != -1)
            return enemiesInLockRange[minimumIndex].rigidbody.gameObject;
        else
            return null;

    }

    public Vector2 GetTargetDirection()
    {
        return (currentTarget.transform.position - transform.position).normalized;
    }

    public bool IsTargeting()
    {
        return currentTarget != null;
    }
}
