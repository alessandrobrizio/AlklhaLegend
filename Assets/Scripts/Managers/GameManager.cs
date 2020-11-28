using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : MonoBehaviour
{
    [System.Serializable]
    class TransformList
    {
        public Transform[] trans;
    }
    public static GameManager Instance { get; private set; }
    
    #region Inspector variables
    [Header("Main characters")]
    [SerializeField] private Moon moon = null;
    [SerializeField] private Player player = null;
    [SerializeField] private Alklha alklha = null;
    [Space]
    [Header("Enemy spawning")]
    [SerializeField] private GameObject enemyPrefab = null;
    //[SerializeField] private Transform enemySpawnPoint = null;
    [SerializeField] private TransformList[] enemySpawnPointsPerWave = null;
    [SerializeField] [ShowOnly] private int wave = 0;
    [SerializeField] private float waveDelay = 5f;
    //[SerializeField] private int enemiesInWave = 3;
    [SerializeField] private int[] enemiesInWave = null;
    [SerializeField] private float enemySpawnDelay = 2f;
    [Space]
    [Header("Power Ups")]
    [SerializeField] private GameObject energyPowerUpPrefab = null;
    [SerializeField] private GameObject elementalPowerUpPrefab = null;
    [SerializeField] private Transform powerUpSpawnPoint = null;
    [SerializeField] private float minEnergyPowerUpDelay = 10f;
    [SerializeField] private float maxEnergyPowerUpDelay = 20f;
    #endregion

    #region Private variables
    private bool isSpawning = true;
    private bool isWaveEnded = true;
    private int enemiesInWaveCount = 0;
    private float enemyWaveTimer = 0f;
    private float enemySpawnTimer = 0f;
    private int bossPhase = 0;
    #endregion

    #region Properties
    public Moon Moon => moon;
    public Player Player => player;
    public Alklha Alklha => alklha;
    #endregion

    #region Events
    [Space]
    [Header("Events")]
    public UnityEvent moonshotEvent = new UnityEvent();
    public UnityEvent bossPhaseEndEvent = new UnityEvent();
    /// <summary>
    /// Event called on gameover conditions
    /// <para>Pass true for player win, false for lose</para>
    /// </summary>
    public UnityEvent<bool> gameOverEvent = new UnityEvent<bool>();
    #endregion

    #region MonoBehaviour methods
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        if (Instance != this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        StartCoroutine(SpawnPowerUps(minEnergyPowerUpDelay, maxEnergyPowerUpDelay));
    }

    private void Update()
    {
        if (isSpawning)
        {
            if (isWaveEnded)
            {
                enemyWaveTimer += Time.deltaTime;
                if (enemyWaveTimer >= waveDelay)
                {
                    NextWave();
                }
            }
            else
            {
                enemySpawnTimer += Time.deltaTime;
                if (enemySpawnTimer >= enemySpawnDelay)
                {
                    for(int i = 0; i < enemySpawnPointsPerWave[bossPhase].trans.Length; i++)
                    {
                        Instantiate(enemyPrefab, enemySpawnPointsPerWave[bossPhase].trans[i].position, 
                            enemySpawnPointsPerWave[bossPhase].trans[i].rotation);
                        enemiesInWaveCount++;
                        enemySpawnTimer = 0f;
                    }
                    

                }
                if (enemiesInWaveCount >= enemiesInWave[bossPhase])
                {
                    EndWave();
                }
            }
        }
    }
    #endregion

    #region Event handlers
    public void OnMoonshot()
    {
        Debug.Log("Moonshot called!");
        isSpawning = false;
        EndWave();
    }

    public void OnBossPhaseEnd()
    {
        Debug.Log("Let's get into next wave!");
        isSpawning = true;
        bossPhase++;
        NextWave();
    }

    public void OnGameOver(bool hasWon)
    {
        Debug.Log($"OnGameover {hasWon}");
        StopAllCoroutines();
        isSpawning = false;
        if (hasWon)
        {
            Debug.Log("You win!");
        }
        else
        {
            Debug.Log("You lose!");
        }
    }
    #endregion

    private void NextWave()
    {
        wave++;
        isWaveEnded = false;
        enemyWaveTimer = 0f;
        if (wave == 3)
        {
            Invoke(nameof(SpawnElementalPowerUp), 5f);
        }
    }

    private void EndWave()
    {
        isWaveEnded = true;
        enemiesInWaveCount = 0;
    }

    private IEnumerator SpawnPowerUps(float minDelay, float maxDelay)
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
            Rigidbody powerUpRigidbody = Instantiate(energyPowerUpPrefab, powerUpSpawnPoint.position, powerUpSpawnPoint.rotation)
                .GetComponent<Rigidbody>();
            powerUpRigidbody.velocity = Random.insideUnitSphere * 3f;
            powerUpRigidbody.angularVelocity = Random.insideUnitSphere * 3f;
        }
    }

    private void SpawnElementalPowerUp()
    {
        Rigidbody powerUpRigidbody = Instantiate(elementalPowerUpPrefab, powerUpSpawnPoint.position, powerUpSpawnPoint.rotation)
            .GetComponent<Rigidbody>();
        powerUpRigidbody.velocity = Random.insideUnitSphere * 3f;
        powerUpRigidbody.angularVelocity = Random.insideUnitSphere * 3f;
    }
}
