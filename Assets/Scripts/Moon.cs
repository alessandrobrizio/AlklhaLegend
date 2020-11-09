using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [SerializeField] private float maxIntegrity = 100.0f;

    [SerializeField] [ShowOnly] private float integrity = 0.0f;

    private void Start()
    {
        integrity = maxIntegrity;
    }

    //TODO temporary (use Damage method instead?)
    public float Integrity
    {
        get => integrity;
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }

    public void GetDamage(float damage)
    {
        integrity -= damage;
        if (integrity <= 0f)
        {
            RaiseGameOver();
        }
    }

    public void GetHeal(float heal)
    {
        integrity = Mathf.Max(maxIntegrity, integrity + heal);
    }
}
