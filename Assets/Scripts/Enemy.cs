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
    //[SerializeField] private List<VisualEffect> smokeEffect = null;
    [SerializeField] private Renderer enemyMesh = null;
    [SerializeField] private AudioClip playerDamageAudioClip = null;
 
    private Player player = null;
    private NavMeshAgent agent = null;
    private bool assimilated = false;
    private static AudioSource lastAudioSource = null; //TEMPORARY FIX
    private Renderer renderer = null;
    private Collider col = null;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        renderer = transform.GetChild(0).GetComponent<Renderer>();
        col = GetComponent<Collider>();
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
        GameManager.Instance.gameOverEvent.AddListener(OnGameOver);
    }

    private void OnDisable()
    {
        GameManager.Instance.moonshotEvent.RemoveListener(OnMoonshot);
        GameManager.Instance.gameOverEvent.RemoveListener(OnGameOver);
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
        //Destroy(gameObject);
        col.enabled = false;
        agent.isStopped = true;
        StartCoroutine(FreezeAnimation(renderer.material));
        for(int i = 1; i < transform.childCount; i++)
        {            
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    IEnumerator FreezeAnimation(Material mat)
    {
        float freezeLevel = 0.0f;
        float duration = 1.0f;
        float time = 0.0f;
        while(freezeLevel <= 1.0f)
        {
            freezeLevel = time / duration;
            mat.SetFloat("FreezeAmount", freezeLevel);
            time += Time.deltaTime;
            yield return null;
        }

        float dissolveLevel = 0.0f;
        time = 0.0f;
        while (dissolveLevel <= 1.0f)
        {
            dissolveLevel = time / duration;
            mat.SetFloat("DissolveAmount", dissolveLevel);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    IEnumerator SmokeAssimilation()
    {
        float destroyDelay = 0f;
        if (playerDamageAudioClip != null && TryGetComponent(out AudioSource audioSource))
        {
            if (lastAudioSource == null || !lastAudioSource.isPlaying)
            {
                audioSource.PlayOneShot(playerDamageAudioClip);
                destroyDelay = playerDamageAudioClip.length;
                lastAudioSource = audioSource;
            }
        }
        float time = destroyDelay - 1.5f;
        agent.isStopped = true;
        while (time > 0.0f)
        {
            transform.position = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * 20.0f);
            time -= Time.deltaTime;
            yield return null;
        }
        /*foreach(VisualEffect vs in smokeEffect)
        {
            vs.Stop();
        }*/
        Destroy(gameObject);
    }

    private void OnGameOver(bool hasWon)
    {
        if (hasWon)
        {
            
        }
        else
        {
            agent.isStopped = true;
        }
    }
}
