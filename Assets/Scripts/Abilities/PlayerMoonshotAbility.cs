using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoonshotAbility", menuName = "Ability/Player/Moonshot")]
public class PlayerMoonshotAbility : PlayerAbility
{
    public override void Cast(Player player)
    {
        base.Cast(player);
        //TODO consume player moonshot charge
        //TODO attack Alklha
        RaiseMoonshot();
    }

    private void RaiseMoonshot()
    {
        GameManager.Instance.moonshotEvent.Invoke();
    }
}
