using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float damage = 0.5f;
    [SerializeField] private float radius = 0.5f;
    [SerializeField] private VisualEffect[] smokeEffect = null;
    [SerializeField] private Renderer enemyMesh = null;
 
    private Player player = null;
    private NavMeshAgent agent = null;
    private bool assimilated = false;

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

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(Vector3.Distance(other.transform.position, transform.position) <= radius && !assimilated)
            {
                assimilated = true;
                other.GetComponent<PlayerEnergy>().GetDamage(damage, false);
                GetComponent<Collider>().enabled = false;
                if (enemyMesh != null)
                {
                    enemyMesh.enabled = false;
                }
                Debug.Log("Trigger Damage");
                StartCoroutine(SmokeAssimilation());
            }
        }
    }

    public void OnDie()
    {
        GameManager.Instance.Player.moonshotCharge++;
        Destroy(gameObject);
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
