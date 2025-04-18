using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public List<AudioClip> footsteps;
    public GameObject walkParticlePrefab;
    public Vector2 offset;
    public float customVolume;

    private float lastFootstepTimeStamp;

    private void PlayFootsteps()
    {
        if (CanPlayFootsteps())
        {
            SFXManager.Instance.PlayRandomSFXAtLocation(footsteps.ToArray(), transform, customVolume);
            Instantiate(walkParticlePrefab, transform.position + (Vector3)offset, Quaternion.identity);
            lastFootstepTimeStamp = Time.time;
        }
    }

    private bool CanPlayFootsteps()
    {
        return Time.time - lastFootstepTimeStamp > 0.1f;
    }
}