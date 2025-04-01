using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OgreSFX : EnemySFX
{
    protected override void PlayHurt()
    {
        SFXManager.Instance.PlayRandomSFXAtLocation(hurtSounds.ToArray(), transform, pitch:0.4f);
    }

    protected override void PlayAnticipationAttack()
    {
        SFXManager.Instance.PlaySFX(attackSound, pitch:0.4f);
    }
}
