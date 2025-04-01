using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOffset : MonoBehaviour
{
    private CinemachineCameraOffset cameraOffset;
    private SamuraiMovement samuraiMovement;
    private Rigidbody2D rb;

    private Vector2 offsetVelocity;
    public float offsetPower;
    public float smoothTime;

    void Start()
    {
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        samuraiMovement = MainCharacter.Instance.GetComponent<SamuraiMovement>();
        rb = MainCharacter.Instance.GetComponent<Rigidbody2D>();
    }

    void LateUpdate()
    {
        ChangeOffset(samuraiMovement.LastMovement * offsetPower);
    }

    private void ChangeOffset(Vector2 newOffset)
    {
        Vector2 offset = Vector2.SmoothDamp(cameraOffset.m_Offset, newOffset, ref offsetVelocity, smoothTime);

        cameraOffset.m_Offset = offset;
    }
}