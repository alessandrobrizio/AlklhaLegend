using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlklhaStunAbility", menuName = "Ability/Alklha/Stun")]
public class AlklhaStunAbility : AlklhaAbility
{
    public override void Cast(Alklha alklha)
    {
        base.Cast(alklha);

        //TODO: stun player
        //Area or front
    }
}
