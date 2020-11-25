using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Moon : MonoBehaviour
{
    [SerializeField] private float maxIntegrity = 100.0f;

    [SerializeField] [ShowOnly] private float integrity = 0.0f;
    [SerializeField] VisualEffect[] smokeEffect = null;

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
        integrity = Mathf.Max(maxIntegrity, integrity + heal);
        UpdateMoonShader();
    }

    private void UpdateMoonShader()
    {
        renderer.material.SetFloat("Moon_Phase", (maxIntegrity - integrity)/maxIntegrity);
    }

    public void StopVFX()
    {
        foreach(VisualEffect vs in smokeEffect)
        {
            vs.Stop();
        }
    }

    public void ResumeVFX()
    {
        foreach (VisualEffect vs in smokeEffect)
        {
            vs.Play();
        }
    }
}
