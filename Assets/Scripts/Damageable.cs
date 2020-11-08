using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    #region Inspector variables
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] [ShowOnly] private float health;
    [SerializeField] private bool isInvincible = false;
    #endregion

    #region Properties
    public float Health => health;
    #endregion

    #region Events
    [Space]
    /// <summary>
    /// Event called when receiving damage
    /// <para>Pass damage amount</para>
    /// </summary>
    public UnityEvent<float> damageEvent = new UnityEvent<float>();
    public UnityEvent dieEvent = new UnityEvent();
    #endregion

    private void Awake()
    {
        health = maxHealth;
    }

    public void GetDamage(float damage)
    {
        if (!isInvincible)
            health -= damage;

        damageEvent.Invoke(damage);
        if (health <= 0f)
        {
            dieEvent.Invoke();
        }
    }
}
