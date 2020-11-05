using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alklha : MonoBehaviour
{
    [SerializeField] AlklhaAbility[] abilities = null;
    [SerializeField] float initialCooldown = 3.0f;

    private int bossPhase = 0;
    private float abilityTimer = 0.0f;
    private Animator animator = null;

    public float AbilityTimer{ set { abilityTimer = value; } }

    private void Start()
    {
        abilityTimer = initialCooldown;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        abilityTimer -= Time.deltaTime;
        if(abilityTimer <= 0)
        {
            int i = Random.Range(0, abilities.Length);
            abilities[i].Cast(this);
        }
        //chase player
    }


    private void OnMoonShot()
    {
        //TODO
    }

    private void RaiseBossPhaseEnd()
    {
        //TODO
    }

    private void RaiseGameOver()
    {
        //TODO
    }
}
