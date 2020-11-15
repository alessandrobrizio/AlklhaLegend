using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{

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
            //TODO : Damage player
            Destroy(gameObject);
        }
    }

    //Moonshot ability terminates enemy wave
    private void OnMoonshot()
    {
        Destroy(gameObject);
    }
}
