﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Moon moon = null;
    [SerializeField] private Player player = null;
    [SerializeField] private Alklha alklha = null;
    [SerializeField] private GameObject enemyPrefab = null;
    [SerializeField] private Transform enemySpawnPoint = null;
    [SerializeField] private int wave = 0;
    [SerializeField] private float waveDelay = 5f;
    [SerializeField] private int enemiesInWave = 3;
    [SerializeField] private float enemySpawnDelay = 2f;

    private bool isWaveEnded = true;
    private int enemiesInWaveCount = 0;
    private float enemyWaveTimer = 0f;
    private float enemySpawnTimer = 0f;

    public Moon Moon => moon;
    public Player Player => player;
    public Alklha Alklha => alklha;

    public UnityEvent moonshotEvent = new UnityEvent();
    public UnityEvent bossPhaseEndEvent = new UnityEvent();
    public UnityEvent<bool> gameOverEvent = new UnityEvent<bool>();

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

    private void OnEnable()
    {
        moonshotEvent.AddListener(OnMoonshot);
        bossPhaseEndEvent.AddListener(OnBossPhaseEnd);
        gameOverEvent.AddListener(OnGameOver);
    }

    private void OnDisable()
    {
        moonshotEvent.RemoveListener(OnMoonshot);
        bossPhaseEndEvent.RemoveListener(OnBossPhaseEnd);
        gameOverEvent.RemoveListener(OnGameOver);
    }

    private void OnMoonshot()
    {
        Debug.Log("Moonshot called!");
    }

    private void OnBossPhaseEnd()
    {
        Debug.Log("Let's get into next boss phase!");
    }

    private void OnGameOver(bool hasWon)
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
}
