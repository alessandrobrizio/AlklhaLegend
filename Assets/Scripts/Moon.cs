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
    private bool gameEnded = false;

    public float Integrity => integrity;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        integrity = maxIntegrity;
    }

    private void RaiseGameOver()
    {
        GameManager.Instance.gameOverEvent.Invoke(false);
    }

    public void GetDamage(float damage)
    {
        if (gameEnded)
            return;
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

    public void OnGameOver(bool hasWon)
    {
        if (hasWon && !gameEnded)
        {
            StartCoroutine(RebuildMoon());
        }
        gameEnded = true;
    }

    private IEnumerator RebuildMoon()
    {
        Debug.Log("Rebuild Moon");
        float endIntegrity = integrity;
        yield return new WaitForSeconds(2.0f);
        while (endIntegrity < 100.0f)
        {
            endIntegrity += Time.deltaTime * 5.0f;
            endIntegrity = Mathf.Clamp(endIntegrity, 0.0f, 99.0f);
            renderer.material.SetFloat("Moon_Phase", (maxIntegrity - endIntegrity) / maxIntegrity);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
