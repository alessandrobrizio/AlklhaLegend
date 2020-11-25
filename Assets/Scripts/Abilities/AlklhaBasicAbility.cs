using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlklhaBasicAbility", menuName = "Ability/Alklha/Basic")]
public class AlklhaBasicAbility : AlklhaAbility
{
    [SerializeField] private float damage = 5.0f;

    public override bool Apply(Alklha caster, Collider target)
    {
        if (target.CompareTag("Player"))
        {
            target.GetComponent<PlayerEnergy>().GetDamage(damage, true);
            return true;
        }
        return false;
    }
}
