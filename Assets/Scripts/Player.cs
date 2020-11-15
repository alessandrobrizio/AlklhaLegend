using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField]
    float speed = 4f, turnSpeed = 0.2f, deltaX = 10f, deltaZ = 10f, playerDamage;

    [Header("Abilities")]
    [SerializeField]
    PlayerAbility[] playerAbilityList;

    [Header("Colliders")]
    [SerializeField]
    Collider rightHandCollider, areaAttackCollider;

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
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Minion" || other.gameObject.tag == "Boss")
        {
            other.gameObject.GetComponent<Damageable>().GetDamage(playerDamage);
        }
    }

    void Update()
    {
        CheckInput();
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
            
            //targetPos.x = ValueInRange(targetPos.x, 0) ? targetPos.x: transform.position.x;
            targetPos.z = ValueInRange(targetPos.z, 1) ? targetPos.z: transform.position.z;

            transform.position = targetPos;

            anim.SetBool("Move", true);
        }
        else
            anim.SetBool("Move", false);

        if (Input.GetMouseButtonDown(0))    //proto attacco base
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Orc_wolfrider_08_attack_B"))
                anim.SetTrigger("Attack");
        }
        else if (Input.GetMouseButtonDown(1))   //prototipo attacco speciale, da implementare
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Orc_wolfrider_08_attack_B"))
                anim.SetTrigger("Attack");
        }
        else if (Input.GetKeyDown(KeyCode.E))
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

}
