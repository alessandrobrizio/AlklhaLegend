using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] private float initialEnergy = 100.0f;
    [SerializeField] private float minimumEnergyLevel = 20.0f;

    [SerializeField] [ShowOnly] private float currentEnergyLevel = 0.0f;

    public float Energy { get { return currentEnergyLevel; } }

    private void Start()
    {
        currentEnergyLevel = initialEnergy;
    }

    public void GetDamage(float damage, bool isBossDamage)
    {
        currentEnergyLevel -= damage;

        //Clamp to minimum value if not against Alklha
        if(!isBossDamage && currentEnergyLevel <= minimumEnergyLevel)
        {
            currentEnergyLevel = minimumEnergyLevel;
        }

        if (currentEnergyLevel <= 0.0f)
        {
            RaiseGameOver();
        }
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }
}
