using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerBasicAbility", menuName = "Ability/Player/Basic")]
public class PlayerBasicAbility : PlayerAbility
{
    public override void Cast(Player player)
    {
        base.Cast(player);

        //TODO: damage Enemy/Alklha
        // Area or front
    }
}
