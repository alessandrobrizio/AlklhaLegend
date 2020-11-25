using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //[Header("Player Settings")]
    [SerializeField]
    float speed = 4f, turnSpeed = 0.2f, deltaX = 10f, deltaZ = 10f, playerDamage;

    [Header("Abilities")]
    public //[SerializeField] TODO temporary
    PlayerAbility[] playerAbilityList = new PlayerAbility[3];
    int currentAbilityIndex;
    const int NO_ABILITY_INDEX = -1;
    const int BASIC_ABILITY_INDEX = 0;
    const int ELEMENTAL_ABILITY_INDEX = 1;
    const int MOONSHOT_ABILITY_INDEX = 2;
    float abilityCooldown;
    public float moonshotCharge;

    [Header("Colliders")]
    [SerializeField]
    SphereCollider rightHandCollider = null, areaAttackCollider = null;

    //GameObject moonTransform;
    Animator anim;

    Vector3 startPosition;


    void Start()
    {
        //moonTransform = GameObject.FindGameObjectWithTag("Moon");
        anim = GetComponent<Animator>();
        startPosition = Vector3.zero;

        rightHandCollider.enabled = false;
        areaAttackCollider.enabled = false;

        currentAbilityIndex = NO_ABILITY_INDEX;
        areaAttackCollider.radius = playerAbilityList[BASIC_ABILITY_INDEX].Range;
        abilityCooldown = .0f;
        moonshotCharge = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (currentAbilityIndex != NO_ABILITY_INDEX)
        {
            if (playerAbilityList[currentAbilityIndex].Apply(this, other))
                currentAbilityIndex = NO_ABILITY_INDEX;
        }
    }

    void Update()
    {
        if (abilityCooldown > 0)
            abilityCooldown -= Time.deltaTime;

        CheckInput();

        UpdateColliders();
    }


    private void CheckInput()
    {
        //Move and Rotate
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;

        if (h != 0 || v != 0)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, turnSpeed);

            Vector3 targetPos = transform.position + (dir * speed * Time.deltaTime);

            if (!ValueInRange(targetPos.x, 0))   //player has reached a horizontal edges
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
            anim.SetBool("Move", true);
        }
        else
            anim.SetBool("Move", false);

        if (abilityCooldown <= 0f)
        {
            currentAbilityIndex = NO_ABILITY_INDEX;
            if (Input.GetMouseButtonDown(0))
            {
                currentAbilityIndex = BASIC_ABILITY_INDEX;
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                // Se preso un ipotetico powerup
                if (playerAbilityList[ELEMENTAL_ABILITY_INDEX] != null)
                {
                    currentAbilityIndex = ELEMENTAL_ABILITY_INDEX;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                if (moonshotCharge >= 3f)
                {
                    playerAbilityList[MOONSHOT_ABILITY_INDEX].Cast(this);
                    moonshotCharge = 0f;
                    abilityCooldown = 2f;
                }
            }
            if (currentAbilityIndex != NO_ABILITY_INDEX)
            {
                playerAbilityList[currentAbilityIndex].Cast(this);
                abilityCooldown = 1f;// playerAbilityList[currentAbilityIndex].Cooldown;
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
        float handCol = anim.GetFloat("Base Attack Trigger");
        rightHandCollider.enabled = handCol > 0.0f;

        float areaCol = anim.GetFloat("Area Attack Trigger");
        areaAttackCollider.enabled = areaCol > 0.0f;
    }
}
