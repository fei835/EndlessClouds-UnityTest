using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    private PlayerMain playerMain;

    private void Awake()
    {
        playerMain = GetComponent<PlayerMain>();
    }

    void AttackChainable(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5f)
        {
            playerMain.attackChainable = evt.intParameter;
        }
    }

    void DodgeEnd()
    {
        playerMain.SetInvincible(false);
    }
}
