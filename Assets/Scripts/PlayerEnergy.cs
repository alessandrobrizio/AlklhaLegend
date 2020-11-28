using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEnergy : MonoBehaviour
{
    [SerializeField] private float initialEnergy = 100.0f;
    [SerializeField] private float minimumEnergyLevel = 20.0f;

    [SerializeField] [ShowOnly] private float currentEnergyLevel = 0.0f;
    [SerializeField] private Renderer meshRenderer = null;

    public float Energy { get { return currentEnergyLevel; } }

    private void Start()
    {
        currentEnergyLevel = initialEnergy;
        meshRenderer.material.SetFloat("EmissionIntensity", currentEnergyLevel / initialEnergy);
    }

    public void GetDamage(float damage, bool isBossDamage)
    {
        
        meshRenderer.material.SetFloat("EmissionIntensity", currentEnergyLevel / initialEnergy);
        //Clamp to minimum value if not against Alklha
        if (!isBossDamage && currentEnergyLevel >= minimumEnergyLevel || isBossDamage)
        {
            currentEnergyLevel -= damage;
        } 


        if (currentEnergyLevel <= 0.0f)
        {
            RaiseGameOver();
        }
    }

    public void Heal(float amount)
    {
        currentEnergyLevel = Mathf.Max(initialEnergy, currentEnergyLevel + amount);
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }
}
