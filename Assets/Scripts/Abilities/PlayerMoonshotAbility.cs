using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoonshotAbility", menuName = "Ability/Player/Moonshot")]
public class PlayerMoonshotAbility : PlayerAbility
{
    public override void Cast(Player caster)
    {
        base.Cast(caster);
        Apply(caster, null);
    }

    public override bool Apply(Player caster, Collider target)
    {
        RaiseMoonshot();
        return true;
    }

    private void RaiseMoonshot()
    {
        GameManager.Instance.moonshotEvent.Invoke();
    }
}
