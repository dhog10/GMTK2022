using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private float _spawnFrequency = 1f;

    [SerializeField]
    private GameObject _upDownSpawns;

    [SerializeField]
    private GameObject _dicePrefab;

    [Header("UI")]

    [SerializeField]
    private Image _blackoutPanel;

    private Transform[] _spawns;
    private List<GameObject> _upDownFloatyDice = new List<GameObject>();
    private float _lastSpawn;
    private int _removed;
    private float _blackoutTarget = 1f;
    private bool _blackoutTransitioning;
    private float _blackoutTransitioningTime;
    private float _blackoutFullTime;

    private void Start()
    {
        SoundManager.Instance.Music = SoundManagerMusic.Menu;

        _spawns = _upDownSpawns.GetComponentsInChildren<Transform>().Select(t => t.transform).ToArray();

        this.Blackout(false, 5f);
    }

    private void Update()
    {
        #region Blackout

        var blackoutTarget = _blackoutTarget;

        if (_blackoutTransitioning || Time.time - _blackoutFullTime < _blackoutTransitioningTime)
        {
            blackoutTarget = 1f;
        }

        var blackoutCol = _blackoutPanel.color;
        var blackoutAlpha = blackoutCol.a;
        blackoutAlpha += (blackoutTarget - blackoutAlpha) * Mathf.Min(0.5f, Time.deltaTime * 0.05f);
        blackoutCol.a = blackoutAlpha;

        if ((_blackoutTransitioning || Time.time - _blackoutFullTime < _blackoutTransitioningTime) && blackoutCol.a > 0.99f)
        {
            blackoutCol.a = 1f;
            _blackoutTransitioning = false;
        }

        _blackoutPanel.color = blackoutCol;

        #endregion

        var spawnInterval = 60f / _spawns.Length * _spawnFrequency;
        if (Time.time - _lastSpawn >= spawnInterval)
        {
            _lastSpawn = Time.time;

            var spawn = _spawns[Random.Range(0, _spawns.Length)];
            var dice = GameObject.Instantiate(_dicePrefab, spawn.transform.position, Random.rotation);
            var scale = Random.Range(1f, spawn.localScale.x);
            dice.transform.localScale = Vector3.one * scale;
            _upDownFloatyDice.Add(dice);
        }

        foreach (var dice in _upDownFloatyDice)
        {
            Random.InitState((int)Mathf.Round(dice.transform.position.x * 10));

            var speed = Random.Range(1.5f, 4.5f);
            var rot = Quaternion.AngleAxis(Random.Range(2f, 8f) * Mathf.Deg2Rad, Random.rotation * Vector3.forward);
            dice.transform.position += Vector3.up * speed * Time.deltaTime;
            dice.transform.rotation *= rot;

            if (dice.transform.position.y > 150f)
            {
                _upDownFloatyDice.Remove(dice);
                GameObject.DestroyImmediate(dice);
            }
        }

        Random.InitState((int)(Time.time * 10));
    }

    public void Blackout(bool on, float sustainTime = 0.1f)
    {
        _blackoutTarget = on ? 1f : 0f;
        _blackoutTransitioning = true;
        _blackoutTransitioningTime = sustainTime;
    }
}
