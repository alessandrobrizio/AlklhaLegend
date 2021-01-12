using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "PlayerBasicAbility", menuName = "Ability/Player/Basic")]
public class PlayerBasicAbility : PlayerAbility
{
    [SerializeField] private float damage = 2.0f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float initialDelay = 0.2f;
    [SerializeField] private string spawnpositionName = "BaseAttack_spawnposition";
    [SerializeField] private bool visualEffectOnHit = true;

    //Temporary workaround for the power up ability
    [SerializeField] private GameObject vfxPrefab = null;
    [SerializeField] private bool setCasterRotation = false;


    public override bool Apply(Player caster, Collider target)
    {
        bool returnValue = false;
        if (target.TryGetComponent(out Damageable targetDamageable))
        {
            targetDamageable.GetDamage(damage);
            returnValue = true;
        }
        if (visualEffectOnHit)
        {
            SpawnVisualEffect(caster);
        }
        return returnValue;
    }

    public override void Cast(Player caster)
    {
        base.Cast(caster);
        if (!visualEffectOnHit)
        {
            SpawnVisualEffect(caster);
        }
    }

    private void SpawnVisualEffect(Player caster)
    {
        Quaternion rot = Quaternion.identity;
        if (setCasterRotation)
        {
            Vector3 newRot = new Vector3(0f, caster.transform.rotation.eulerAngles.y + 90.0f, 0f);
            rot = Quaternion.Euler(newRot);
        }

        GameObject ability_spawnposition = GameObject.Find(spawnpositionName);
        if (ability_spawnposition == null)
        {
            Debug.LogError("Player must have a child named " + spawnpositionName + " with a visual effect component");
        }
        GameObject vfx = Instantiate(vfxPrefab, ability_spawnposition.transform.position, rot);
        Destroy(vfx, 3.0f);
        vfxPrefab.GetComponent<VisualEffect>().Play();
    }
}
