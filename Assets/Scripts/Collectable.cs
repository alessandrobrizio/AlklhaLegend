using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Collectable : MonoBehaviour
{
    [SerializeField] private Object collectable;

    private void OnValidate()
    {
        if (collectable && !(collectable is ICollectable))
        {
            Debug.LogError("Object does not implements ICollectable");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((collectable as ICollectable).Collect(other.gameObject))
        {
            //Debug.Log("Start collecting...");
            Destroy(gameObject);
        }
    }
}
