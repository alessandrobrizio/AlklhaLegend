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
    [SerializeField] private Material deathMaterial = null;

    public float Energy { get { return currentEnergyLevel; } }
    private bool gameEnded = false;

    private void Start()
    {
        currentEnergyLevel = initialEnergy;
        meshRenderer.material.SetFloat("EmissionIntensity", currentEnergyLevel / initialEnergy);
    }

    public void GetDamage(float damage, bool isBossDamage)
    {
        if (gameEnded)
            return;

        UIManager.Instance.AddToOutputQueue(TutorialAction.EnergyInstructions);
        //meshRenderer.material.SetFloat("EmissionIntensity", currentEnergyLevel / initialEnergy);
        //Clamp to minimum value if not against Alklha
        if (!isBossDamage && currentEnergyLevel >= minimumEnergyLevel || isBossDamage)
        {
            currentEnergyLevel -= damage;
        }
        StartCoroutine(DamageAnimation());

        if (currentEnergyLevel <= 0.0f)
        {
            RaiseGameOver();
        }
    }

    private IEnumerator DamageAnimation()
    {
        meshRenderer.material.SetFloat("EmissionIntensity", 0.0f);
        meshRenderer.material.SetFloat("MinLightIntensity", 0.0f);
        yield return new WaitForSeconds(1.0f);
        meshRenderer.material.SetFloat("MinLightIntensity", 0.2f);
        meshRenderer.material.SetFloat("EmissionIntensity", currentEnergyLevel / initialEnergy);
    }

    public void Heal(float amount)
    {
        currentEnergyLevel = Mathf.Min(initialEnergy, currentEnergyLevel + amount);
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }

    public void OnGameOver(bool hasWon)
    {
        if (!hasWon && !gameEnded)
        {
            meshRenderer.material = deathMaterial;
            StartCoroutine(Dissolve());
        }
        gameEnded = true;
    }

    private IEnumerator Dissolve()
    {
        float initialTime = 3.0f;
        float time = 0.0f;
        while(time < initialTime)
        {
            time += Time.deltaTime;
            meshRenderer.material.SetFloat("DissolveAmount", time/initialTime);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
