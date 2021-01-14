using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "PlayerMoonshotAbility", menuName = "Ability/Player/Moonshot")]
public class PlayerMoonshotAbility : PlayerAbility
{
    [SerializeField] private VisualEffectAsset moonshotVisualEffect = null;
    [SerializeField] private GameObject impactVFX = null;
    [SerializeField] private float duration = 1.0f;
    [SerializeField] private float moonRadius = 5.0f;
    [SerializeField] private float initialDelay = 2.0f;


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
        caster.StartCoroutine(StartMoonshotAfter(initialDelay, vfx, caster, moonshot_spawnposition));
        
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

    private IEnumerator StopMoonshotAfter(float stopTime, VisualEffect vfx, Player caster)
    {
        yield return new WaitForSeconds(stopTime);
        vfx.Stop();
        //Wait for the last particle to disappear
        yield return new WaitForSeconds(2*stopTime);
        caster.EnableMovement();
        
    }

    private IEnumerator StartMoonshotAfter(float startTime, VisualEffect vfx, Player caster, GameObject moonshot_spawnposition)
    {
        yield return new WaitForSeconds(startTime);
        vfx.visualEffectAsset = moonshotVisualEffect;

        Vector3 offsetCaster = GameManager.Instance.Moon.transform.position - caster.transform.position;
        offsetCaster.Normalize();
        offsetCaster.y = 0.0f;
        caster.transform.rotation = Quaternion.LookRotation(offsetCaster);

        Vector3 offestMoonshot = GameManager.Instance.Moon.transform.position/2.5f - moonshot_spawnposition.transform.position/2.5f;
        vfx.SetFloat("MoonshotMaxSize", offestMoonshot.magnitude - moonRadius);
        offestMoonshot.Normalize();

        moonshot_spawnposition.transform.rotation = Quaternion.LookRotation(offestMoonshot);

        vfx.Play();
        Vector3 impactPosition = caster.transform.position;
        impactPosition.y = 0.05f;
        Destroy(Instantiate(impactVFX, impactPosition, Quaternion.identity), duration);
        caster.DisableMovement();
        caster.StartCoroutine(StopMoonshotAfter(duration, vfx, caster));
    }
}
