using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "ElementalPowerUp", menuName = "PowerUp/Elemental")]
public class ElementalPowerUp : PowerUp
{
    private enum ElementalType { Fire, /*Ice, Electricity*/ } //TODO
    [SerializeField] ElementalType type = ElementalType.Fire;
    [SerializeField] PlayerAbility playerAbility = null;
    [SerializeField] float duration = 0f;
    [SerializeField] private VisualEffectAsset abilityVisualEffect = null;
    [SerializeField] private float initialDelay = 0.2f;


    public override bool Collect(GameObject collector)
    {
        if (collector.TryGetComponent(out Player player))
        {
            //Assign secondary ability
            player.EarnElementalAbility(playerAbility);
            Debug.Log("Here's a new ability! And some fancy effects...");
            //TODO handle particles
            UIManager.Instance.AddToOutputQueue(TutorialAction.ElementalAttackCollected);
            return true;
        }
        return false;

       
    }

    //TODO enable duration
}
