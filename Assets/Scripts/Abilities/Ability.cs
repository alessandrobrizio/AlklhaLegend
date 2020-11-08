using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    [SerializeField] protected float range = 2.0f;
    [SerializeField] protected float cooldown = 3.0f;
    [Tooltip("Rest time between the end of an attack and the next cast")]
    [SerializeField] protected float rest = 0.5f;
    [SerializeField] protected AnimatorOverrideController animatorOverrideCtrl = null;

    public float Range => range;
    public float Cooldown => cooldown + animatorOverrideCtrl["Attack"].length;
    public float AttackDuration => animatorOverrideCtrl["Attack"].length + rest;
}
