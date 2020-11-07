using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alklha : MonoBehaviour
{
    private enum AlklhaState { Idle, Chase, Attack /*//TODO: Stunned*/}

    [SerializeField] AlklhaAbility[] abilities = null;
    [SerializeField] float initialCooldown = 1.0f;
    [SerializeField] float playerThreshold = 1.0f;

    private int bossPhase = 0;
    private float abilityCooldown = 0.0f;
    private Animator animator = null;
    private AlklhaState alklhaState = AlklhaState.Idle;
    private AlklhaState alklhaStateOld = AlklhaState.Idle;
    private Player player = null;
    private float distanceFromPlayer = 0.0f;

    //TODO: probability to stun Alhkla for a moment
    //TODO: damage player
    //TODO: attack colliders   

    private void Start()
    {
        player = GameManager.Instance.Player;

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
        if (alklhaState != alklhaStateOld)
        {
            OnExitState(alklhaStateOld);
            OnEnterState(alklhaState);
            alklhaStateOld = alklhaState;
        }
        //update state
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
    }

    private void OnEnable()
    {
        GameManager.Instance.moonshotEvent.AddListener(OnMoonshot);
    }

    private void OnDisable()
    {
        GameManager.Instance.moonshotEvent.RemoveListener(OnMoonshot);
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
            alklhaState = AlklhaState.Attack;
        }
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
        if (distanceFromPlayer <= playerThreshold && abilityCooldown > 0)
        {
            alklhaState = AlklhaState.Idle;
        }
    }

    private void OnMoonshot()
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
        switch (alklhaState)
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
        switch (alklhaState)
        {
            case AlklhaState.Idle:
                break;
            case AlklhaState.Chase:
                animator.SetBool("Walking", true);
                break;
            case AlklhaState.Attack:
                //Choose an ability and cast
                int i = Random.Range(0, abilities.Length);
                abilities[i].Cast(this);
                abilityCooldown = abilities[i].Cooldown;
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
