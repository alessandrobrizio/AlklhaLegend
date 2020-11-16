using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alklha : MonoBehaviour
{
    private enum AlklhaState { Idle, Chase, Attack /*//TODO: Stunned*/}
    [Header("Abilities")]
    [SerializeField] private AlklhaAbility[] abilities = null;
    [SerializeField] private float initialCooldown = 1.0f;
    [Space]
    [Header("Colliders")]
    [SerializeField] private Collider handDxCollider = null;
    [SerializeField] private Collider handSxCollider = null;
    [SerializeField] private Collider feetCollider = null;
    [Space]
    [Header("Debug")]
    //TODO: move to Alklha ability
    [SerializeField] private float damage = 5.0f;

    [SerializeField] [ShowOnly] private int bossPhase = 0;
    private float attackAnimationDuration = 0.0f;
    private Animator animator = null;
    [SerializeField] [ShowOnly] private AlklhaState alklhaState = AlklhaState.Idle;
    private AlklhaState alklhaStateOld = AlklhaState.Idle;
    private Player player = null;
    private float distanceFromPlayer = 0.0f;
    private float playerThreshold = 2.0f;
    private bool playerHit = false;

    //Cooldown for each ability in list
    private float[] abilityCooldowns = null;

    private int nextAttack = 0;    

    //TODO: probability to stun Alhkla for a moment

    private void Start()
    {
        player = GameManager.Instance.Player;

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
        //Update cooldowns
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
                //Debug.Log("Is IDLE");
                break;
            case AlklhaState.Chase:
                TriggerIdleState();
                CheckTriggerAttackState();
                break;
            case AlklhaState.Attack:
                //Attack Animation Ended
                if (attackAnimationDuration < 0.0f)
                {
                    playerHit = false;
                    alklhaState = AlklhaState.Idle;
                }
                //Activate/deactivate attack colliders
                float attackTriggerHandDx = animator.GetFloat("AttackTriggerHandDx");
                float attackTriggerHandSx = animator.GetFloat("AttackTriggerHandSx");
                float attackFeetTrigger = animator.GetFloat("AttackTriggerFeet");

                handDxCollider.enabled = attackTriggerHandDx > 0.5f;
                handSxCollider.enabled = attackTriggerHandSx > 0.5f;
                feetCollider.enabled = attackFeetTrigger > 0.5f;
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

    public void OnMoonshot()
    {
        bossPhase++;
        //TODO
    }

    private void RaiseBossPhaseEnd()
    {
        //TODO
        GameManager.Instance.bossPhaseEndEvent.Invoke();
    }

    private void RaiseGameOver()
    {
        if(bossPhase > 3)
        {
            //TODO
            GameManager.Instance.gameOverEvent.Invoke(true);
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
                handDxCollider.enabled = false;
                handSxCollider.enabled = false;
                feetCollider.enabled = false;
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
            other.GetComponent<PlayerEnergy>().GetDamage(damage, true);
        }
    }
}
