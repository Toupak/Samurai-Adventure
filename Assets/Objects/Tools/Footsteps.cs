using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public List<AudioClip> footsteps;
    public GameObject walkParticlePrefab;
    public Vector2 offset;
    public float customVolume;

    private void PlayFootsteps()
    {
        SFXManager.Instance.PlayRandomSFXAtLocation(footsteps.ToArray(), transform, customVolume);
        Instantiate(walkParticlePrefab, transform.position + (Vector3)offset, Quaternion.identity);
    }
}