using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnOption
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private int _levelThreshold;

    [SerializeField]
    private float _chance = 100f;

    public GameObject EnemyPrefab
        => _enemyPrefab;

    public int LevelThreshold
        => _levelThreshold;

    public float Chance
        => _chance;
}
