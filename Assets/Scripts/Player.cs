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
    [SerializeField]
    PlayerAbility[] playerAbilityList;
    int currentAbilityIndex;
    float abilityCooldown;

    [Header("Colliders")]
    [SerializeField]
    SphereCollider rightHandCollider, areaAttackCollider;

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

        //Debug, should be -1 normally
        currentAbilityIndex = 0;
        areaAttackCollider.radius = playerAbilityList[currentAbilityIndex].Range;
        abilityCooldown = .0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        string tag = other.gameObject.tag;

        switch(tag){

            case "Minion":
                Debug.Log("Player has damaged " + other.tag);
                other.gameObject.GetComponent<Damageable>().GetDamage(playerDamage);
                break;
            case "Boss":
                Debug.Log("Player has damaged " + other.tag);
                other.gameObject.GetComponent<Damageable>().GetDamage(playerDamage);
                break;
            case "Fire":
                currentAbilityIndex = 0;
                areaAttackCollider.radius = playerAbilityList[currentAbilityIndex].Range;
                break;
            case "Ice":
                currentAbilityIndex = 1;
                areaAttackCollider.radius = playerAbilityList[currentAbilityIndex].Range;
                break;
        }

        /*
        if (other.gameObject.tag == "Minion" || other.gameObject.tag == "Boss")
        {
            Debug.Log("Player has damaged " + other.tag);
            other.gameObject.GetComponent<Damageable>().GetDamage(playerDamage);
        }
        else if(other.gameObject.tag == "FirePowerUp")
        {
            currentAbilityIndex = 0;
        }
        */
    }

    void Update()
    {
        if(currentAbilityIndex >= 0 && abilityCooldown > 0)
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

            if(!ValueInRange(targetPos.x, 0))   //player has reached a horizontal edges
            {
                targetPos.x = transform.position.x;
                Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(true, targetPos.x);
            }
            else
            {
                Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(false, targetPos.x);
            }
            

            targetPos.z = ValueInRange(targetPos.z, 1) ? targetPos.z: transform.position.z;
            transform.position = targetPos;
            anim.SetBool("Move", true);
        }
        else
            anim.SetBool("Move", false);

        if (Input.GetMouseButtonDown(0))    //proto attacco base
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Basic Attack"))
                anim.SetTrigger("Basic Attack");
        }
        else if (Input.GetKeyDown(KeyCode.E))   //prototipo attacco speciale, TODO: abilitare solo tramite pwrUP
        {
            // Se preso un ipotetico powerup
            if(currentAbilityIndex >= 0)
            {
                if (abilityCooldown <= 0.0f)
                {
                    playerAbilityList[currentAbilityIndex].Cast(this);
                    abilityCooldown = playerAbilityList[currentAbilityIndex].Cooldown;
                }
            }
            
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            //TODO sostituire con l'ability
            GameManager.Instance.moonshotEvent.Invoke();
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
