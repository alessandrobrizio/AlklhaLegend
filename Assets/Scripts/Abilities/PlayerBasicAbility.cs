using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBasicAbility", menuName = "Ability/Player/Basic")]
public class PlayerBasicAbility : PlayerAbility
{
    [SerializeField] private float damage = 2.0f;

    public override bool Apply(Player caster, Collider target)
    {
        if (target.TryGetComponent(out Damageable targetDamageable))
        {
            targetDamageable.GetDamage(damage);
            return true;
        }
        return false;
    }
}
