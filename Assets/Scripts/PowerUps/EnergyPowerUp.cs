using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnergyPowerUp", menuName = "PowerUp/Energy")]
public class EnergyPowerUp : PowerUp
{
    [SerializeField] private float energy = 1f;

    public override bool Collect(GameObject collector)
    {
        if (collector.TryGetComponent(out PlayerEnergy playerEnergy))
        {
            playerEnergy.Heal(energy);
            Debug.Log("Energy shot!");
            return true;
        }
        return false;
    }
}
