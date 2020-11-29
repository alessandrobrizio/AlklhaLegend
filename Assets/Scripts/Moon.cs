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
            StartCoroutine(rebuildMoon());
        }
        gameEnded = true;
    }

    private IEnumerator rebuildMoon()
    {
        yield return new WaitForSeconds(2.0f);

        while (integrity < 1.0f)
        {
            integrity += Time.deltaTime/2;

            renderer.material.SetFloat("Moon_Phase", integrity);
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
