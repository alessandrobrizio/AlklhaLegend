using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : MonoBehaviour
{
    [SerializeField] private float maxIntegrity = 100.0f;

    [SerializeField] [ShowOnly] private float integrity = 0.0f;

    private Renderer renderer = null;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

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
        UpdateMoonShader();
        if (integrity <= 0f)
        {
            RaiseGameOver();
        }
    }

    public void GetHeal(float heal)
    {
        integrity = Mathf.Min(maxIntegrity, integrity + heal);
        UpdateMoonShader();
    }

    private void UpdateMoonShader()
    {
        renderer.material.SetFloat("Vector1_1BD8129C", (maxIntegrity - integrity)/maxIntegrity);
    }
}
