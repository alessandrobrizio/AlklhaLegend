using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerAbility : Ability
{
    public virtual void Cast(Player player)
    {
        Animator playerAnimator = player.GetComponent<Animator>();
        if (playerAnimator == null)
            return;

        Debug.Log($"Player is casting {name}");
        playerAnimator.runtimeAnimatorController = animatorOverrideCtrl;
        playerAnimator.SetTrigger("Attack");
    }
}
