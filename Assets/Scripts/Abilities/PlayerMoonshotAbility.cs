using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "PlayerMoonshotAbility", menuName = "Ability/Player/Moonshot")]
public class PlayerMoonshotAbility : PlayerAbility
{
    [SerializeField] private VisualEffectAsset moonshotVisualEffect = null;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float moonRadius = 5.0f;


    public override void Cast(Player caster)
    {
        base.Cast(caster);
        GameObject moonshot_spawnposition = GameObject.Find("Moonshot_spawnposition");
        if(moonshot_spawnposition == null)
        {
            Debug.LogError("Player must have a child named moonshot_spawnposition with a visual effect component");
        }
        VisualEffect vfx = moonshot_spawnposition.GetComponent<VisualEffect>();
        if (vfx == null)
        {
            Debug.LogError("Player must have a child named moonshot_spawnposition with a visual effect component");
        }

        vfx.visualEffectAsset = moonshotVisualEffect;

        Vector3 offsetCaster = GameManager.Instance.Moon.transform.position - caster.transform.position;
        offsetCaster.Normalize();
        offsetCaster.y = 0.0f;
        caster.transform.rotation = Quaternion.LookRotation(offsetCaster);

        Vector3 offestMoonshot = GameManager.Instance.Moon.transform.position - moonshot_spawnposition.transform.position;
        vfx.SetFloat("MoonshotMaxSize", offestMoonshot.magnitude - moonRadius);
        offestMoonshot.Normalize();

        moonshot_spawnposition.transform.rotation = Quaternion.LookRotation(offestMoonshot);

        vfx.Play();
        caster.StartCoroutine(StopMoonshotAfter(duration, vfx));
        Apply(caster, null);

        RaiseMoonshot();
    }

    public override bool Apply(Player caster, Collider target)
    {
        return false;
    }

    private void RaiseMoonshot()
    {
        GameManager.Instance.moonshotEvent.Invoke();
    }

    private IEnumerator StopMoonshotAfter(float stopTime, VisualEffect vfx)
    {
        yield return new WaitForSeconds(stopTime);
        vfx.Stop();
    }
}
