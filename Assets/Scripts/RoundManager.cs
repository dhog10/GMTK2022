using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    public static RoundManager Instance;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            GameObject.DestroyImmediate(this.gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        GameObject.DontDestroyOnLoad(this);
    }

    public int Round { get; set; }

    public bool RoundActive { get; set; }

    public bool RoundEndBlocked
        => this.RoundPercentage >= 1f && Object.FindObjectsOfType<Enemy>().Length > 0;

    public float RoundDuration { get; set; }

    public float RoundStartTime { get; set; }

    public float RoundPercentage
        => Mathf.Clamp((Time.time - this.RoundStartTime) / this.RoundDuration, 0.0f, 1.0f);

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
            this.FinishRound(false);
        }

        Debug.Log("StartRound");

        SoundManager.Instance.Music = SoundManagerMusic.Action;
        this.RoundActive = true;
        this.Round = round;
        this.RoundStartTime = Time.time;
        this.RoundDuration = _roundsDuration;

        StartCoroutine(this.Delay(4f, () =>
        {
            var initialSpawn = (int)Mathf.Floor(_enemyInitialSpawnCount);
            for (var i = 0; i < initialSpawn; i++)
            {
                this.SpawnEnemy(out _);
            }
        }));
    }

    public void FinishRound(bool roundsContinue = true)
    {
        if (!this.RoundActive)
        {
            return;
        }

        Debug.Log($"FinishRound {roundsContinue}");

        this.RoundActive = false;
        SoundManager.Instance.Music = SoundManagerMusic.Menu;

        _enemySpawnInterval -= _enemySpawnInterval * _enemySpawnIntervalDecrease;
        _enemyInitialSpawnCount += _enemyInitialSpawnIncrease;

        if (roundsContinue)
        {
            StartCoroutine(this.HealPlayer());
            StartCoroutine(this.Delay(15f, () =>
            {
                this.NextRound();
            }));
        }
    }

    public void ResetRounds()
    {
        this.FinishRound(false);
        this.Round = 0;
    }

    public IEnumerable<EnemySpawn> EnemySpawns
        => GameObject.FindObjectsOfType<EnemySpawn>();

    private void RoundThink()
    {
        var roundEndBlocked = this.RoundEndBlocked;

        if (this.RoundPercentage >= 1f && !roundEndBlocked)
        {
            this.FinishRound();
        }

        if (this.RoundActive && !roundEndBlocked)
        {
            if (Time.time - _lastEnemySpawn >= _enemySpawnInterval)
            {
                this.SpawnEnemy(out _);
            }
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

        if (_enemySpawnOptions == null)
        {
            Debug.LogError("no enemy spawn options set");
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

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }

    private IEnumerator HealPlayer()
    {
        var player = CharacterControl.Instance;
        if (player == null)
        {
            yield return null;
        }

        while (player.Damagable.Health < player.Damagable.MaxHealth)
        {
            var incr = 1f / player.Damagable.MaxHealth * 200f;
            player.Damagable.SetHealth(player.Damagable.Health + incr);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
