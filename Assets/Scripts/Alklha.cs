using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alklha : MonoBehaviour
{
    private enum AlklhaState { Idle, Chase, Attack /*//TODO: Stunned*/}

    [SerializeField] private AlklhaAbility[] abilities = null;
    [SerializeField] private float initialCooldown = 1.0f;
    [SerializeField] private Collider handDxCollider = null;
    [SerializeField] private Collider handSxCollider = null;
    [SerializeField] private Collider feetCollider = null;

    private int bossPhase = 0;
    private float attackAnimationDuration = 0.0f;
    private Animator animator = null;
    private AlklhaState alklhaState = AlklhaState.Idle;
    private AlklhaState alklhaStateOld = AlklhaState.Idle;
    private Player player = null;
    private float distanceFromPlayer = 0.0f;
    private float playerThreshold = 2.0f;
    private bool playerHit = false;

    //Cooldown for each ability in list
    private float[] abilityCooldowns = null;

    private int nextAttack = 0;    

    //TODO: probability to stun Alhkla for a moment
    //TODO: damage player
    //TODO: attack colliders   

    private void Start()
    {
        //TODO: take gameobject from GameManager
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //attackAnimationDuration = initialCooldown;
        animator = GetComponent<Animator>();

        //Initialize cooldown
        abilityCooldowns = new float[abilities.Length];
        float closestAbilityRange = float.MaxValue;
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            abilityCooldowns[i] = initialCooldown;
            if(abilities[i].Range < closestAbilityRange)
            {
                closestAbilityRange = abilities[i].Range;
            }
        }

        playerThreshold = closestAbilityRange;
    }

    private void Update()
    {
        //Update timer
        attackAnimationDuration -= Time.deltaTime;
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            abilityCooldowns[i] -= Time.deltaTime;
        }

        //Calculate distance from player
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //Change state
        if (alklhaState != alklhaStateOld)
        {
            OnExitState(alklhaStateOld);
            OnEnterState(alklhaState);
            alklhaStateOld = alklhaState;
        }

        //Update state
        switch (alklhaState)
        {
            case AlklhaState.Idle:
                CheckTriggerAttackState();
                CheckTriggerChasePlayer();
                break;
            case AlklhaState.Chase:
                TriggerIdleState();
                CheckTriggerAttackState();
                break;
            case AlklhaState.Attack:
                alklhaState = AlklhaState.Idle;
                break;
        }

        //Activate/deactivate attack colliders
        float attackTriggerHandDx = animator.GetFloat("AttackTriggerHandDx");
        float attackTriggerHandSx = animator.GetFloat("AttackTriggerHandSx");
        float attackFeetTrigger = animator.GetFloat("AttackTriggerFeet");

        handDxCollider.enabled = attackTriggerHandDx > 0.5f;
        handSxCollider.enabled = attackTriggerHandSx > 0.5f;
        feetCollider.enabled = attackFeetTrigger > 0.5f;

        //Attack Animation Ended
        if (attackAnimationDuration < 0.0f)
        {
            playerHit = false;
        }


    }

    private void ChasePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
        direction.Normalize();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 3.0f);
    }

    //If player is close enough and cooldown allows it, chose one ability and attack
    private void CheckTriggerAttackState()
    {
        List<int> possibleAttacks = new List<int>();

        //Check which abilities can be used
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            if (distanceFromPlayer <= abilities[i].Range && abilityCooldowns[i] <= 0
                && attackAnimationDuration <= 0)
            {
                possibleAttacks.Add(i);
            }
        }

        if(possibleAttacks.Count > 0)
        {
            alklhaState = AlklhaState.Attack;
            nextAttack = possibleAttacks[UnityEngine.Random.Range(0, possibleAttacks.Count)];
        }
       
        /*if (distanceFromPlayer <= playerThreshold && abilityCooldown <= 0)
        {
            alklhaState = AlklhaState.Attack;
        }*/
    }

    private void CheckTriggerChasePlayer()
    {
        //Get closer to player if too distant
        if (distanceFromPlayer > playerThreshold)
        {
            alklhaState = AlklhaState.Chase;
        }
    }

    private void TriggerIdleState()
    {
        //Player is close but alhkla can't attack, so he'll wait in Idle
        if (distanceFromPlayer <= playerThreshold && attackAnimationDuration > 0)
        {
            alklhaState = AlklhaState.Idle;
        }
    }

    private void OnMoonShot()
    {
        bossPhase++;
        //TODO
    }

    private void RaiseBossPhaseEnd()
    {
        //TODO
    }

    private void RaiseGameOver()
    {
        if(bossPhase > 3)
        {
            //TODO
        }
    }

    private void OnExitState(AlklhaState state)
    {
        switch (state)
        {
            case AlklhaState.Idle:
                break;
            case AlklhaState.Chase:
                animator.SetBool("Walking", false);
                break;
            case AlklhaState.Attack:
                break;
        }
    }

    private void OnEnterState(AlklhaState state)
    {
        switch (state)
        {
            case AlklhaState.Idle:
                break;
            case AlklhaState.Chase:
                animator.SetBool("Walking", true);
                break;
            case AlklhaState.Attack:
                //Cast ability
                abilities[nextAttack].Cast(this);
                abilityCooldowns[nextAttack] = abilities[nextAttack].Cooldown;
                attackAnimationDuration = abilities[nextAttack].AttackDuration;
                break;
        }
    }

    // Orientation via script
    // Movement via root motion 
    private void OnAnimatorMove()
    {
        //Always orient to player
        ChasePlayer();
        //Apply root motion
        transform.position = animator.rootPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !playerHit)
        {
            //hit player just once
            playerHit = true;
            Debug.Log("Trigger");
            //TODO player damage
        }
    }
}
