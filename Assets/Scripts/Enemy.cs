using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float damage = 0.5f;
    [SerializeField] VisualEffect[] smokeEffect = null;
 
    private Player player = null;
    private NavMeshAgent agent = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        player = GameManager.Instance.Player;
    }

    private void Update()
    {
        agent.SetDestination(player.transform.position);
    }

    private void OnEnable()
    {
        GameManager.Instance.moonshotEvent.AddListener(OnMoonshot);
    }

    private void OnDisable()
    {
        GameManager.Instance.moonshotEvent.RemoveListener(OnMoonshot);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerEnergy>().GetDamage(damage, false);
            GetComponent<Collider>().enabled = false;
            GetComponent<Renderer>().enabled = false;
            Debug.Log("Trigger Damage");
            StartCoroutine(SmokeAssimilation());
        }
    }

    //Moonshot ability terminates enemy wave
    private void OnMoonshot()
    {
        Destroy(gameObject);
    }

    IEnumerator SmokeAssimilation()
    {
        float time = 1.0f;
        while (time > 0.0f)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 3.0f);
            time -= Time.deltaTime;
            yield return null;
        }
        foreach(VisualEffect vs in smokeEffect)
        {
            vs.Stop();
        }
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }
}
