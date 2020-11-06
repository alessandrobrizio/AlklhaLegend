using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alklha : MonoBehaviour
{
    private enum AlhlkaState { Idle, Chase, Attack /*//TODO: Stunned*/}

    [SerializeField] AlklhaAbility[] abilities = null;
    [SerializeField] float initialCooldown = 1.0f;
    [SerializeField] float playerThreshold = 1.0f;

    private int bossPhase = 0;
    private float abilityCooldown = 0.0f;
    private Animator animator = null;
    private AlhlkaState alhklaState = AlhlkaState.Idle;
    private AlhlkaState alhklaStateOld = AlhlkaState.Idle;
    private Player player = null;
    private float distanceFromPlayer = 0.0f;
    
    //TODO: probability to stun Alhkla for a moment
    //TODO: damage player
    //TODO: attack colliders

    /// <summary>
    /// Time necessary to allow the player to attack again
    /// </summary>
    public float AbilityCooldown { set { abilityCooldown = value; } }

    private void Start()
    {
        //TODO: take gameobject from GameManager
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        abilityCooldown = initialCooldown;
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        //Update timer
        abilityCooldown -= Time.deltaTime;

        //Calculate distance from player
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);

        //change state
        if (alhklaState != alhklaStateOld)
        {
            OnExitState(alhklaStateOld);
            OnEnterState(alhklaState);
            alhklaStateOld = alhklaState;
        }
        //update state
        switch (alhklaState)
        {
            case AlhlkaState.Idle:
                CheckTriggerAttackState();
                CheckTriggerChasePlayer();
                break;
            case AlhlkaState.Chase:
                TriggerIdleState();
                CheckTriggerAttackState();
                break;
            case AlhlkaState.Attack:
                alhklaState = AlhlkaState.Idle;
                break;
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction = Vector3.ProjectOnPlane(direction, Vector3.up);
        direction.Normalize();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 3.0f);
    }

    
    private void CheckTriggerAttackState()
    {
        //Attack if player is close enough and cooldown allows it
        if (distanceFromPlayer <= playerThreshold && abilityCooldown <= 0)
        {
            alhklaState = AlhlkaState.Attack;
        }
    }

    private void CheckTriggerChasePlayer()
    {
        //Get closer to player if too distant
        if (distanceFromPlayer > playerThreshold)
        {
            alhklaState = AlhlkaState.Chase;
        }
    }

    private void TriggerIdleState()
    {
        //Player is close but alhkla can't attack, so he'll wait in Idle
        if (distanceFromPlayer <= playerThreshold && abilityCooldown > 0)
        {
            alhklaState = AlhlkaState.Idle;
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

    private void OnExitState(AlhlkaState state)
    {
        switch (alhklaState)
        {
            case AlhlkaState.Idle:
                break;
            case AlhlkaState.Chase:
                animator.SetBool("Walking", false);
                break;
            case AlhlkaState.Attack:
                break;
        }
    }

    private void OnEnterState(AlhlkaState state)
    {
        switch (alhklaState)
        {
            case AlhlkaState.Idle:
                break;
            case AlhlkaState.Chase:
                animator.SetBool("Walking", true);
                break;
            case AlhlkaState.Attack:
                //Choose an ability and cast
                int i = UnityEngine.Random.Range(0, abilities.Length);
                abilities[i].Cast(this);
                break;
        }
    }

    // Orientation via script
    // Movement via root motion 
    private void OnAnimatorMove()
    {
        //Alwas orient to player
        ChasePlayer();
        //Apply root motion
        transform.position = animator.rootPosition;
    }
}
