using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[Header("Player Settings")]
    [SerializeField]
    float speed = 4f, turnSpeed = 0.2f, deltaX = 10f, deltaZ = 10f;

    [Header("Abilities")]
    [SerializeField] PlayerAbility[] abilities;
    [SerializeField] float moonshotChargeRequirement = 3f;
    [SerializeField] [ShowOnly] int currentAbilityIndex;
    const int NO_ABILITY_INDEX = -1;
    const int BASIC_ABILITY_INDEX = 0;
    const int ELEMENTAL_ABILITY_INDEX = 1;
    const int MOONSHOT_ABILITY_INDEX = 2;
    private float attackAnimationDuration = 0f;
    //Cooldown for each ability in list
    private float[] abilityCooldowns = null;
    private bool enemyHit = false;
    public float moonshotCharge;
    private bool canMove = true;

    [Header("Colliders")]
    [SerializeField] SphereCollider headCollider = null;
    [SerializeField] SphereCollider tailCollider = null;

    //GameObject moonTransform;
    Animator anim;

    Vector3 startPosition;


    void Start()
    {
        //moonTransform = GameObject.FindGameObjectWithTag("Moon");
        anim = GetComponent<Animator>();
        startPosition = Vector3.zero;

        headCollider.enabled = false;
        tailCollider.enabled = false;

        currentAbilityIndex = NO_ABILITY_INDEX;
        headCollider.radius = abilities[BASIC_ABILITY_INDEX].Range;
        abilityCooldowns = new float[abilities.Length];
        moonshotCharge = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentAbilityIndex != NO_ABILITY_INDEX)
        {
            if (abilities[currentAbilityIndex].Apply(this, other))
                enemyHit = true;
        }
    }

    void Update()
    {
        //Update cooldowns
        attackAnimationDuration -= Time.deltaTime;
        for (int i = 0; i < abilityCooldowns.Length; i++)
        {
            abilityCooldowns[i] -= Time.deltaTime;
        }

        CheckInput();

        UpdateColliders();
    }


    private void CheckInput()
    {
        if (!canMove)
            return;

        //Move and Rotate
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;

        if (h != 0 || v != 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            anim.SetFloat("Speed", speed);
            anim.SetBool("Move", true);
        }
        else
            anim.SetBool("Move", false);

        if (attackAnimationDuration <= 0f)
        {
            currentAbilityIndex = NO_ABILITY_INDEX;
            enemyHit = false;
            if (Input.GetMouseButtonDown(0) && abilityCooldowns[BASIC_ABILITY_INDEX] <= 0f)
            {
                currentAbilityIndex = BASIC_ABILITY_INDEX;
            }
            else if (Input.GetKeyDown(KeyCode.E) && abilityCooldowns[ELEMENTAL_ABILITY_INDEX] <= 0f)
            {
                // Se preso un ipotetico powerup
                if (abilities[ELEMENTAL_ABILITY_INDEX] != null)
                {
                    currentAbilityIndex = ELEMENTAL_ABILITY_INDEX;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q) && abilityCooldowns[MOONSHOT_ABILITY_INDEX] <= 0f)
            {
                if (moonshotCharge >= moonshotChargeRequirement)
                {
                    currentAbilityIndex = MOONSHOT_ABILITY_INDEX;
                    moonshotCharge = 0f;
                }
            }
            if (currentAbilityIndex != NO_ABILITY_INDEX)
            {
                abilities[currentAbilityIndex].Cast(this);
                abilityCooldowns[currentAbilityIndex] = abilities[currentAbilityIndex].Cooldown;
                attackAnimationDuration = abilities[currentAbilityIndex].AttackDuration;
            }
        }
    }

    /// <summary>
    /// Return true if the input value is in the specified range, otherwise false
    /// </summary>
    /// <param name="val"> value to confront </param>
    /// <param name="selector"> 0: X-Axis; 1: Z-Axis</param>
    /// <returns>True if in range, false otherwise</returns>
    bool ValueInRange(float val, int selector)
    {
        //check X Axes
        if (selector == 0)
        {
            return (val > startPosition.x - deltaX && val < startPosition.x + deltaX) ? true : false;
        }
        else if (selector == 1)
        {
            return (val > startPosition.z - deltaZ && val < startPosition.z + deltaZ) ? true : false;
        }

        //fallback Value
        Debug.LogWarning("haven't passed a legal parameter to function ValueInRange. Has been returned true by default");
        return true;
    }

    private void UpdateColliders()
    {
        float handCol = anim.GetFloat("HeadTrigger");
        headCollider.enabled = handCol > 0.5f;

        float areaCol = anim.GetFloat("TailTrigger");
        tailCollider.enabled = areaCol > 0.5f;
    }

    private void OnAnimatorMove()
    {
        Vector3 targetPos = anim.rootPosition;
        if (!ValueInRange(targetPos.x, 0))   //Player has reached an horizontal edge
        {
            targetPos.x = transform.position.x;
            Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(true, targetPos.x);
        }
        else
        {
            Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(false, targetPos.x);
        }

        targetPos.z = ValueInRange(targetPos.z, 1) ? targetPos.z : transform.position.z;
        transform.position = targetPos;
    }

    public void EarnElementalAbility(PlayerAbility ability)
    {
        abilities[ELEMENTAL_ABILITY_INDEX] = ability;
        tailCollider.radius = ability.Range;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }

    public void OnGameOver(bool hasWon)
    {
        if (!hasWon)
        {
            gameObject.SetActive(false);
        }
    }
}
