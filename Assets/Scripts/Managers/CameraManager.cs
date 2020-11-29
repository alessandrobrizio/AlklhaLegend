using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    float offset = 0f, rotation = 25f, lerpVal = 0.01f;

    Vector3 negOffsetPosition;
    Vector3 posOffsetPosition;
    Vector3 initPos;

    Quaternion posRot;
    Quaternion negRot;
    Quaternion initRot;

    bool applyingOffset;
    float sign;

    // Start is called before the first frame update
    void Start()
    {
        negOffsetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - offset);
        posOffsetPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + offset);
        initPos = transform.position;

        posRot = Quaternion.Euler(0f, rotation, 0f);
        negRot = Quaternion.Euler(0f, -rotation, 0f);
        initRot = transform.rotation;

        applyingOffset = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (applyingOffset)
        {
            if (sign < 0)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, posRot, lerpVal * Time.deltaTime);

                transform.position = Vector3.Lerp(transform.position, negOffsetPosition, lerpVal * Time.deltaTime);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, negRot, lerpVal * Time.deltaTime);

                transform.position = Vector3.Lerp(transform.position, posOffsetPosition, lerpVal * Time.deltaTime);
            }
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initRot, lerpVal);

            transform.position = Vector3.Lerp(transform.position, initPos, lerpVal);
        }
    }

    public void SetApplyOffset(bool b, float offsetSign)
    {
        applyingOffset = b;
        this.sign = offsetSign;
    }
}
