using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    #region Inspector variables
    [Header("Main characters")]
    [SerializeField] private Moon moon = null;
    [SerializeField] private Player player = null;
    [SerializeField] private Alklha alklha = null;
    [Space]
    [Header("Enemy spawning")]
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private Transform enemySpawnPoint = null;
    [SerializeField] private int wave = 0;
    [SerializeField] private float waveDelay = 5f;
    [SerializeField] private int enemiesInWave = 3;
    [SerializeField] private float enemySpawnDelay = 2f;
    #endregion

    #region Private variables
    private bool isWaveEnded = true;
    private int enemiesInWaveCount = 0;
    private float enemyWaveTimer = 0f;
    private float enemySpawnTimer = 0f;
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

    private void Update()
    {
        if (isWaveEnded)
        {
            enemyWaveTimer += Time.deltaTime;
            if (enemyWaveTimer >= waveDelay)
            {
                wave++;
                isWaveEnded = false;
                enemyWaveTimer = 0f;
            }
        }
        else
        {
            enemySpawnTimer += Time.deltaTime;
            if (enemySpawnTimer >= enemySpawnDelay)
            {
                Instantiate(enemyPrefab, enemySpawnPoint.position, enemySpawnPoint.rotation);
                enemiesInWaveCount++;
                enemySpawnTimer = 0f;
            }
            if (enemiesInWaveCount >= enemiesInWave)
            {
                isWaveEnded = true;
                enemiesInWaveCount = 0;
            }
        }
    }
    #endregion

    #region Event handlers
    public void OnMoonshot()
    {
        Debug.Log("Moonshot called!");
        //TODO interrompere wave
    }

    public void OnBossPhaseEnd()
    {
        Debug.Log("Let's get into next wave!");
        //TODO far partire una nuova wave
    }

    public void OnGameOver(bool hasWon)
    {
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
}
