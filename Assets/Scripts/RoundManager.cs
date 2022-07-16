using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [SerializeField]
    private float _roundsDuration = 60f;

    [SerializeField]
    private float _enemySpawnInterval = 10f;

    [SerializeField]
    private float _enemySpawnIntervalDecrease = 0.1f;

    [SerializeField]
    private float _enemyInitialSpawnCount = 1f;

    [SerializeField]
    private float _enemyInitialSpawnIncrease = 0.5f;

    [SerializeField]
    private EnemySpawnOption[] _enemySpawnOptions;

    private float _lastEnemySpawn;

    private void Start()
    {

    }

    public int Round { get; set; }

    public bool RoundActive { get; set; }

    public float RoundDuration { get; set; }

    public float RoundStartTime { get; set; }

    public float RoundPercentage
        => Mathf.Clamp(Time.time - this.RoundStartTime / this.RoundDuration, 0.0f, 1.0f);

    private void Update()
    {
        this.RoundThink();
    }

    public void NextRound()
    {
        this.StartRound(this.Round + 1);
    }

    public void StartRound(int round)
    {
        if (this.RoundActive)
        {
            this.FinishRound();
        }

        Debug.Log("StartRound");

        this.RoundActive = false;
        this.Round = round;
        this.RoundStartTime = Time.time;
        this.RoundDuration = _roundsDuration;

        var initialSpawn = (int)Mathf.Floor(_enemyInitialSpawnCount);
        for (var i = 0; i < initialSpawn; i++)
        {
            this.SpawnEnemy(out _);
        }
    }

    public void FinishRound()
    {
        Debug.Log("FinishRound");

        this.RoundActive = false;

        _enemySpawnInterval -= _enemySpawnInterval * _enemySpawnIntervalDecrease;
        _enemyInitialSpawnCount += _enemyInitialSpawnIncrease;
    }

    public IEnumerable<EnemySpawn> EnemySpawns
        => GameObject.FindObjectsOfType<EnemySpawn>();

    private void RoundThink()
    {
        if (this.RoundPercentage >= 1f)
        {
            this.FinishRound();
        }

        if (Time.time - _lastEnemySpawn >= _enemySpawnInterval)
        {
            this.SpawnEnemy(out _);
        }
    }

    private bool SpawnEnemy(out GameObject enemy)
    {
        var player = Object.FindObjectOfType<CharacterControl>();
        var playerPos = player?.transform.position ?? Vector3.zero;

        enemy = null;

        _lastEnemySpawn = Time.time;

        var spawns = this.EnemySpawns.Where(s => Vector3.Distance(s.transform.position, playerPos) > 5f).ToArray();
        if (spawns.Length == 0)
        {
            Debug.LogError("No enemy spawns");
            return false;
        }

        var validSpawnOptions = _enemySpawnOptions.Where(o => o.LevelThreshold <= this.Round);
        var totalChance = 0f;
        foreach (var option in validSpawnOptions)
        {
            totalChance += option.Chance;
        }

        EnemySpawnOption spawnOption = null;
        var chance = Random.Range(0f, totalChance);
        totalChance = 0f;
        foreach (var option in validSpawnOptions)
        {
            totalChance += option.Chance;
            if (totalChance >= chance)
            {
                spawnOption = option;
                break;
            }
        }

        if (spawnOption == null)
        {
            Debug.LogError("null spawn option");
            return false;
        }

        var spawn = spawns[Random.Range(0, spawns.Length - 1)];
        if (spawn == null)
        {
            Debug.LogError("null spawn");
            return false;
        }

        enemy = GameObject.Instantiate(spawnOption.EnemyPrefab, spawn.transform.position, Quaternion.identity);
        return false;
    }
}
