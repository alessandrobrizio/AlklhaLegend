using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlklhaAbility : Ability
{
    [SerializeField] protected float range = 2.0f;
    [SerializeField] protected float cooldown = 3.0f;
    [SerializeField] protected AnimatorOverrideController animatorOverrideCtrl = null;

    public virtual void Cast(Alklha alklha)
    {
        Animator alklhaAnimator = alklha.GetComponent<Animator>();
        if (alklhaAnimator == null)
            return;

        Debug.Log("Cast ");
        alklhaAnimator.runtimeAnimatorController = animatorOverrideCtrl;
        alklhaAnimator.SetTrigger("Attack");

        alklha.AbilityCooldown = cooldown + animatorOverrideCtrl["Attack"].length;
    }
}
