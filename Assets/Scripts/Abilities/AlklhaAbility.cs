using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AlklhaAbility : Ability
{
    public virtual void Cast(Alklha alklha)
    {
        Animator alklhaAnimator = alklha.GetComponent<Animator>();
        if (alklhaAnimator == null)
            return;
        
        Debug.Log($"Alklha is casting {name}");
        alklhaAnimator.runtimeAnimatorController = animatorOverrideCtrl;
        alklhaAnimator.SetTrigger("Attack");
    }
}
