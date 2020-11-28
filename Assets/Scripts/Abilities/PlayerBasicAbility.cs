using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "PlayerBasicAbility", menuName = "Ability/Player/Basic")]
public class PlayerBasicAbility : PlayerAbility
{
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private VisualEffectAsset abilityVisualEffect = null;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float initialDelay = 0.2f;
    [SerializeField] private string spawnpositionName = "BaseAttack_spawnposition";

    public override bool Apply(Player caster, Collider target)
    {
        if (target.TryGetComponent(out Damageable targetDamageable))
        {
            targetDamageable.GetDamage(damage);
            return true;
        }

        GameObject ability_spawnposition = GameObject.Find(spawnpositionName);
        if (ability_spawnposition == null)
        {
            Debug.LogError("Player must have a child named " + spawnpositionName + " with a visual effect component");
        }
        VisualEffect vfx = ability_spawnposition.GetComponent<VisualEffect>();
        if (vfx == null)
        {
            Debug.LogError("Player must have a child named " + spawnpositionName + " with a visual effect component");
        }
        vfx.visualEffectAsset = abilityVisualEffect;
        if (vfx.HasFloat("Duration"))
        {
            vfx.SetFloat("Duration", duration);
        }

        if (vfx.HasFloat("InitialDelay"))
        {
            vfx.SetFloat("InitialDelay", initialDelay);
        }
        return false;
    }
}
