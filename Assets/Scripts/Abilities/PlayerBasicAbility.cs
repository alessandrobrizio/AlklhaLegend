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
    [SerializeField] private bool visualEffectOnHit = true;

    //Temporary workaround for the power up ability
    [SerializeField] private bool spawnLogic = false;
    [SerializeField] private GameObject vfxPrefab = null;


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
        if (!spawnLogic)
        {
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
            vfx.Play();
            Debug.Log(spawnpositionName + " spawned");
            if (vfx.HasFloat("Duration"))
            {
                vfx.SetFloat("Duration", duration);
            }

            if (vfx.HasFloat("InitialDelay"))
            {
                vfx.SetFloat("InitialDelay", initialDelay);
            }
            if (vfx.HasFloat("Angle"))
            {
                vfx.SetFloat("Angle", caster.transform.position.y);
            }
        }
        else
        {
            Vector3 newRot = new Vector3(0f, caster.transform.rotation.eulerAngles.y + 90.0f, 0f);
            //newRot.y = -newRot.y + offset;
            GameObject vfx = Instantiate(vfxPrefab, caster.transform.position + new Vector3(0.0f, 0.5f, 0.0f), Quaternion.Euler(newRot));
            Destroy(vfx, 3.0f);
            vfxPrefab.GetComponent<VisualEffect>().Play();
        }
        
    }
}
