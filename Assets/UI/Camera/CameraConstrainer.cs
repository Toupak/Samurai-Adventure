using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class CameraConstrainer : MonoBehaviour
{
    [HideInInspector] public static UnityEvent<Vector3> OnPlayerRespawnPingPosition = new UnityEvent<Vector3>();
    [HideInInspector] public static UnityEvent<Transform> OnPlayerRespawnSwitchCamera = new UnityEvent<Transform>();

    private CinemachineConfiner2D cameraConfiner;

    void Start()
    {
        cameraConfiner = GetComponent<CinemachineConfiner2D>();

        if (MainCharacter.Instance.GetComponent<SamuraiMovement>().saveOn == false)
        {
            cameraConfiner.enabled = false;
            return;
        }

        TPScriptLDTK.OnPlayerTeleport.AddListener(UpdateCameraConfiner);
        OnPlayerRespawnSwitchCamera.AddListener(UpdateCameraConfiner);
    }

    private void UpdateCameraConfiner(Transform targetLevel)
    {
        if (targetLevel == null)
        {
            cameraConfiner.m_BoundingShape2D = null;
            return;
        }

        if (targetLevel != null)
            cameraConfiner.m_BoundingShape2D = targetLevel.GetComponent<Collider2D>();
    }
}
