using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoonshotAbility", menuName = "Ability/Player/Moonshot")]
public class PlayerMoonshotAbility : PlayerAbility
{
    public override void Cast(Player caster)
    {
        base.Cast(caster);
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
}
