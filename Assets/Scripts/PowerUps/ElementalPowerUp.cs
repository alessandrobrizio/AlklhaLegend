using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ElementalPowerUp", menuName = "PowerUp/Elemental")]
public class ElementalPowerUp : PowerUp
{
    private enum ElementalType { Fire, /*Ice, Electricity*/ } //TODO
    [SerializeField] ElementalType type = ElementalType.Fire;
    [SerializeField] PlayerAbility playerAbility = null;
    [SerializeField] float duration = 0f;
    [SerializeField] ParticleSystem[] particles = null;


    public override bool Collect(GameObject collector)
    {
        if (collector.TryGetComponent(out Player player))
        {
            //Assign secondary ability
            player.playerAbilityList[1] = playerAbility;
            Debug.Log("Here's a new ability! And some fancy effects...");
            //TODO handle particles
            return true;
        }
        return false;
    }

    //TODO enable duration
}
