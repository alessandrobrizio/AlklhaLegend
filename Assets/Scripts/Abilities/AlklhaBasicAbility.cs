﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AlklhaBasicAbility", menuName = "Ability/Alklha/Basic")]
public class AlklhaBasicAbility : AlklhaAbility
{
    public override void Cast(Alklha alklha)
    {
        base.Cast(alklha);
        
        //TODO: damage Player
        // Area or front
    }
}
