using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Alklha : MonoBehaviour
{
    private enum AlklhaState { Idle, Chase, Attack, OnMoon, MoonShot, EndPhase }
    [Header("Abilities")]
    [SerializeField] private AlklhaAbility[] abilities = null;
    [SerializeField] private float initialCooldown = 1.0f;
    [Space]
    [Header("Colliders")]
    [SerializeField] private Collider handDxCollider = null;
    [SerializeField] private Collider handSxCollider = null;
    [SerializeField] private Collider feetCollider = null;
    [Space]
    [Header("Characteristics")]
    [SerializeField] private float eatingVelocity = 0.2f;
    [SerializeField] private float bossPhaseDuration = 15.0f;
    [Header("MoonShot")]
    [SerializeField] private Transform moonPosition = null;
    [SerializeField] private Transform earthPosition = null;
    [SerializeField] [Range(0.0f, 1.0f)]
    [Tooltip("Time % that indicates when the apex of the jump should be")] private float jumpApex = 0.35f;
    [SerializeField] [Tooltip("Time required to wolf to cast the moonshot")] private float moonshotDuration = 2.0f;
    [SerializeField] [Tooltip("Time required to Alklha to accomplish the jump")] private float animationDuration = 2.0f;
    [SerializeField] private float jumpHeight = 5.0f;
    [Header("Debug")]
    //TODO: move to Alklha ability


    [SerializeField] [ShowOnly] private int bossPhase = 0;
    private float attackAnimationDuration = 0.0f;
    private Animator animator = null;
    [SerializeField] [ShowOnly] private AlklhaState alklhaState = AlklhaState.OnMoon;
    private AlklhaState alklhaStateOld = AlklhaState.Idle;
    private Player player = null;
    private float distanceFromPlayer = 0.0f;
    private float playerThreshold = 2.0f;
    private bool playerHit = false;
    private Moon moon = null;

    private AnimationCurve animationMoonToEarth = null;
    private Vector3 startJumpPosition = Vector3.zero;
    private bool canStartMoonjumpMovement = false;

    #region Timers
    private float moonshotTimer = 0.0f;
    //Cooldown for each ability in list
    private float[] abilityCooldowns = null;
    private float bossPhaseTimer = 0.0f;
    private float animationTimer = 0.0f;
    #endregion

    private int nextAttack = 0;

    private void Start()
    {
        player = GameManager.Instance.Player;
        moon = GameManager.Instance.Moon;

        //attackAnimationDuration = initialCooldown;
        animator = GetComponent<Animator>();

        //Initialize cooldowns
        abilityCooldowns = new float[abilities.Length];
        float closestAbilityRange = float.MaxValue;
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            abilityCooldowns[i] = initialCooldown;
            if (abilities[i].Range < closestAbilityRange)
            {
                closestAbilityRange = abilities[i].Range;
            }
        }

        playerThreshold = closestAbilityRange;

        animationMoonToEarth = new AnimationCurve();
        animationMoonToEarth.AddKey(new Keyframe(0.0f, moonPosition.position.y));
        animationMoonToEarth.AddKey(new Keyframe(jumpApex, moonPosition.position.y + jumpHeight));
        animationMoonToEarth.AddKey(new Keyframe(1.0f, earthPosition.position.y));
        AnimationUtility.SetKeyRightTangentMode(animationMoonToEarth, 0, AnimationUtility.TangentMode.Linear);
        AnimationUtility.SetKeyLeftTangentMode(animationMoonToEarth, 2, AnimationUtility.TangentMode.Linear);

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
                UpdateBossPhaseTimer();
                CheckTriggerAttackState();
                CheckTriggerChasePlayer();
                break;
            case AlklhaState.Chase:
                UpdateBossPhaseTimer();
                TriggerIdleState();
                CheckTriggerAttackState();
                break;
            case AlklhaState.Attack:
                UpdateBossPhaseTimer();
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
            case AlklhaState.OnMoon:
                moon.GetDamage(eatingVelocity * Time.deltaTime);
                break;
            case AlklhaState.MoonShot:
                moonshotTimer += Time.deltaTime;
                if (moonshotTimer >= moonshotDuration && canStartMoonjumpMovement)
                {
                    animationTimer += Time.deltaTime;
                    MoonEarthTransition(AlklhaState.Idle, moonPosition.position, earthPosition.position, animationTimer, false);
                }
                break;
            case AlklhaState.EndPhase:

                if (/*moonshotTimer >= 0.0f && */canStartMoonjumpMovement)
                {
                    animationTimer -= Time.deltaTime;
                    MoonEarthTransition(AlklhaState.OnMoon, startJumpPosition, moonPosition.position, animationTimer, true);
                }
                break;
        }
    }

    private void UpdateBossPhaseTimer()
    {
        bossPhaseTimer -= Time.deltaTime;
        if (bossPhaseTimer <= 0)
        {
            alklhaState = AlklhaState.EndPhase;
            RaiseBossPhaseEnd();
        }
    }

    private void MoonEarthTransition(AlklhaState stateAtTheEnd, Vector3 startPos, Vector3 endPos, float timePassed, bool reverse)
    {
        float animationKey = timePassed / animationDuration;
        if ((!reverse && animationKey >= 1.0f) || (reverse && animationKey <= 0.0f))
        {
            alklhaState = stateAtTheEnd;
            return;
        }
        float height = animationMoonToEarth.Evaluate(animationKey);
        Vector3 translation = (reverse) ? Vector3.Lerp(endPos, startPos, animationKey) : Vector3.Lerp(startPos, endPos, animationKey);
        transform.position = new Vector3(translation.x, height, translation.z);
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

        if (possibleAttacks.Count > 0)
        {
            alklhaState = AlklhaState.Attack;
            nextAttack = possibleAttacks[Random.Range(0, possibleAttacks.Count)];
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
        if (distanceFromPlayer <= playerThreshold && attackAnimationDuration > 0)
        {
            alklhaState = AlklhaState.Idle;
        }
    }

    #region EventsHandler
    public void OnMoonshot()
    {
        bossPhase++;
        if (bossPhase > 3)
        {
            RaiseGameOver();
        }
        else
        {
            alklhaState = AlklhaState.MoonShot;
        }
    }
    #endregion

    private void RaiseBossPhaseEnd()
    {
        GameManager.Instance.bossPhaseEndEvent.Invoke();
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(true);
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
            case AlklhaState.OnMoon:
                animator.SetBool("Eating", false);
                break;
            case AlklhaState.MoonShot:
                bossPhaseTimer = bossPhaseDuration;
                break;
            case AlklhaState.EndPhase:
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
            case AlklhaState.OnMoon:
                animator.SetBool("Eating", true);
                break;
            case AlklhaState.MoonShot:
                animationTimer = 0.0f;
                moonshotTimer = 0.0f;
                canStartMoonjumpMovement = false;
                StartCoroutine(TriggerJumpAnimation());
                break;
            case AlklhaState.EndPhase:
                startJumpPosition = transform.position;
                animationTimer = animationDuration;
                canStartMoonjumpMovement = false;
                animator.SetTrigger("Moonjump");
                break;
        }
    }

    private IEnumerator TriggerJumpAnimation()
    {
        yield return new WaitForSeconds(moonshotDuration);
        animator.SetTrigger("Moonjump");
    }

    // Orientation via script
    // Movement via root motion 
    private void OnAnimatorMove()
    {
        if (alklhaState == AlklhaState.MoonShot || alklhaState == AlklhaState.EndPhase)
            return;

        //Always orient to player
        ChasePlayer();
        //Apply root motion
        transform.position = animator.rootPosition;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!playerHit)
        {
            //Hit player just once
            playerHit = abilities[nextAttack].Apply(this, other);
        }
    }

    public void GetDamage(float damage)
    {
        moon.GetHeal(damage);
    }

    //Called By Animator
    public void CanStartMoonJumpMovement()
    {
        canStartMoonjumpMovement = true;
    }
}
