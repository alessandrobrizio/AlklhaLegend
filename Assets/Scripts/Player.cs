using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float turnSpeed = 0.2f;
    [SerializeField] private float deltaX = 10f;
    [SerializeField] private float deltaZ = 10f;
    [SerializeField] private float XThreshold = 7.0f;
    public enum AbilityIndex
    {
        Basic = 0,
        Elemental = 1,
        Moonshot = 2
    }

    [Header("Abilities")]
    [SerializeField] private float moonshotChargeRequirement = 3f;
    public float moonshotCharge;
    private bool canMove = true;

    public AbilityCaster<Player> abilityCaster = new AbilityCaster<Player>();

    Animator anim;

    Vector3 startPosition;

    void Start()
    {
        anim = GetComponent<Animator>();
        abilityCaster.Awake(this, anim);
        startPosition = Vector3.zero;

        moonshotCharge = 0f;
        abilityCaster.abilitiesInfo[(int)AbilityIndex.Moonshot].customIsReady = () => moonshotCharge >= moonshotChargeRequirement;
    }

    void Update()
    {
        CheckInput();
        abilityCaster.Update();
    }

    private void FixedUpdate()
    {
        abilityCaster.FixedUpdate();
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
            if (!abilityCaster.IsCasting)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, turnSpeed * Time.deltaTime);
            }
            anim.SetFloat("Speed", speed);
            anim.SetFloat("Move", Mathf.Abs(h) == 1f || Mathf.Abs(v) == 1f ? 1f : new Vector3(h, 0f, v).magnitude / new Vector3(h < 0f ? -1f : Mathf.Ceil(h), 0f, v < 0f ? -1f : Mathf.Ceil(v)).magnitude);
        }
        else
            anim.SetFloat("Move", 0);

        if (Input.GetMouseButtonDown(0))
        {
            abilityCaster.TryCast((int)AbilityIndex.Basic);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            abilityCaster.TryCast((int)AbilityIndex.Elemental);

        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (abilityCaster.TryCast((int)AbilityIndex.Moonshot))
            {
                moonshotCharge = 0f;
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
            return (val > startPosition.x - deltaX && val < startPosition.x + deltaX);
        }
        else if (selector == 1)
        {
            return (val > startPosition.z - deltaZ && val < startPosition.z + deltaZ);
        }

        //fallback Value
        Debug.LogWarning("haven't passed a legal parameter to function ValueInRange. Has been returned true by default");
        return true;
    }

    private void OnAnimatorMove()
    {
        Vector3 targetPos = anim.rootPosition;
        if (!ValueInRange(targetPos.x, 0))   //Player has reached an horizontal edge
        {
            targetPos.x = transform.position.x;
            Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(true, -targetPos.x);
        }
        else if (targetPos.x > startPosition.x - (deltaX + XThreshold) && targetPos.x < startPosition.x + (deltaX + XThreshold))
        {
            Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(true, -targetPos.x);
        }
        else
        {
            Camera.main.transform.GetComponentInParent<CameraManager>().SetApplyOffset(false, -targetPos.x);
        }

        targetPos.z = ValueInRange(targetPos.z, 1) ? targetPos.z : transform.position.z;
        transform.position = targetPos;
        transform.rotation = anim.rootRotation;
    }

    public void EarnElementalAbility(PlayerAbility ability)
    {
        abilityCaster.abilitiesInfo[(int)AbilityIndex.Elemental].ability = ability;
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
        DisableMovement();
    }

    private void OnDrawGizmos()
    {
        abilityCaster.OnDrawGizmos(Color.blue);
    }
}
