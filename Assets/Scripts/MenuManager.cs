using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField]
    private float _spawnFrequency = 1f;

    [SerializeField]
    private GameObject _upDownSpawns;

    [SerializeField]
    private DiceButton _startButton;

    [SerializeField]
    private GameObject _dicePrefab;

    [SerializeField]
    private string _gameSceneName;

    [Header("UI")]

    [SerializeField]
    private Image _blackoutPanel;

    [SerializeField]
    private TextMeshProUGUI _roundText;

    [SerializeField]
    private TextMeshProUGUI _hpText;

    [SerializeField]
    private CanvasGroup _deathScreenCG;

    [SerializeField]
    private TextMeshProUGUI _deathRounds;

    [SerializeField]
    private GameObject _statsPanel;

    [SerializeField]
    private GameObject _statsLabelPrefab;

    private Transform[] _spawns;
    private List<GameObject> _upDownFloatyDice = new List<GameObject>();
    private float _lastSpawn;
    private int _removed;
    private float _blackoutTarget = 1f;
    private bool _blackoutTransitioning;
    private float _blackoutTransitioningTime;
    private float _blackoutFullTime;
    private float _blackoutAlpha = 1f;
    private System.Action _blackoutAction;

    private Vector3 _startButtonPos;
    private float _deathScreenOpacity;
    private bool _showDeathScreen;
    private bool _hasAddedStats;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            GameObject.DestroyImmediate(Instance.gameObject);
            return;
        }

        Instance = this;

        GameObject.DontDestroyOnLoad(this);
    }

    private void Start()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.Music = SoundManagerMusic.Menu;
        }

        _spawns = _upDownSpawns.GetComponentsInChildren<Transform>().Select(t => t.transform).ToArray();

        _startButtonPos = _startButton.transform.position;
        _startButton.transform.position += Vector3.up * -70f;

        this.Blackout(false, 4f);

        _deathScreenCG.enabled = true;
        _deathScreenCG.gameObject.SetActive(true);
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
        _blackoutAlpha += (blackoutTarget - _blackoutAlpha) * Mathf.Min(0.5f, Time.deltaTime * (blackoutTarget == 1f ? 0.4f : 0.2f));
        blackoutCol.a = 1f - Mathf.Pow(1f - _blackoutAlpha, 3f);

        if ((_blackoutTransitioning || Time.time - _blackoutFullTime < _blackoutTransitioningTime) && blackoutCol.a > 0.995f)
        {
            blackoutCol.a = 1f;
            if (_blackoutTransitioning)
            {
                _blackoutFullTime = Time.time;
                _blackoutAction?.Invoke();
                _blackoutAction = null;
            }

            _blackoutTransitioning = false;
        }

        _blackoutPanel.color = blackoutCol;

        #endregion

        #region Death screen

        var deathScreenTarget = _showDeathScreen ? 1f : 0f;
        _deathScreenOpacity += (deathScreenTarget - _deathScreenOpacity) * Time.deltaTime;
        _deathScreenCG.alpha = _deathScreenOpacity;

        #endregion

        if (!_hasAddedStats && CharacterControl.Instance != null)
        {
            _hasAddedStats = true;

            foreach (var stat in CharacterControl.Instance.Stats)
            {
                var label = GameObject.Instantiate(_statsLabelPrefab, _statsPanel.transform);
                var text = label.GetComponent<StatsLabel>();
                text.Stat = stat;
            }
        }

        var buttonTarget = _startButtonPos + Vector3.up * Mathf.Cos(Time.time) * 0.9f;
        var vel = Mathf.Min(5f, buttonTarget.y - _startButton.transform.position.y);
        _startButton.transform.position += Vector3.up * vel * Time.deltaTime;
        _startButton.transform.localRotation = Quaternion.Euler(Mathf.Cos(Time.time * 0.3f) * 1f, Mathf.Cos(Time.time * 0.5f) * 5f, 0f);

        if (RoundManager.Instance != null && RoundManager.Instance.Round > 0)
        {
            var text = "round ";
            for (var i = 0; i < Mathf.Floor(RoundManager.Instance.Round / 9); i++)
            {
                text += 9.ToString();
            }

            if (RoundManager.Instance.Round % 9 > 0)
            {
                text += (RoundManager.Instance.Round % 9).ToString();
            }

            _roundText.text = text;

            var player = CharacterControl.Instance;
            if (player == null)
            {
                _hpText.text = "";
            }
            else
            {
                text = "";
                var hp = Mathf.Ceil(player.Damagable.Health);
                for (var i = 0; i < Mathf.Floor(hp / 9); i++)
                {
                    text += 9.ToString();
                }

                if (hp % 9 > 0)
                {
                    text += (hp % 9).ToString();
                }

                _hpText.text = text;
                _hpText.color = Color.Lerp(Color.red, Color.white, player.Damagable.Health / player.Damagable.MaxHealth);
            }
        }
        else
        {
            _roundText.text = "";
            _hpText.text = "";
        }

        if (SceneManager.GetActiveScene().name == "menu")
        {
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

            var toRemove = new List<GameObject>();


            foreach (var dice in _upDownFloatyDice)
            {
                Random.InitState((int)Mathf.Round(dice.transform.position.x * 10));

                var speed = Random.Range(1.5f, 4.5f);
                var rot = Quaternion.AngleAxis(Random.Range(2f, 8f) * Mathf.Deg2Rad, Random.rotation * Vector3.forward);
                dice.transform.position += Vector3.up * speed * Time.deltaTime;
                dice.transform.rotation *= rot;

                if (dice.transform.position.y > 150f)
                {
                    toRemove.Add(dice);
                    GameObject.DestroyImmediate(dice);
                }
            }

            foreach (var dice in toRemove)
            {
                _upDownFloatyDice.Remove(dice);
            }
        }

        Random.InitState((int)(Time.time * 10));
    }

    public void StartGame()
    {
        _startButtonPos = _startButtonPos + Vector3.up * 30f;

        this.Blackout(false, 2f, () =>
        {
            _upDownFloatyDice.Clear();

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            SceneManager.LoadSceneAsync(_gameSceneName, LoadSceneMode.Single);
            SoundManager.Instance.Music = SoundManagerMusic.Action;
            Debug.Log("Start game");
        });
    }

    public void ReturnToMenu(System.Action callback = null)
    {
        this.Blackout(false, 2f, () =>
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;

            SceneManager.LoadSceneAsync("menu", LoadSceneMode.Single);
            SoundManager.Instance.Music = SoundManagerMusic.Menu;
            Debug.Log("Return to menu");
        });
    }

    public void DeathScreen()
    {
        Debug.Log("DeathScreen");

        _showDeathScreen = true;
        _deathRounds.text = $"{RoundManager.Instance.Round - 1} Rounds Survived";

        StartCoroutine(this.Delay(8f, () =>
        {
            this.ReturnToMenu(() =>
            {
                _showDeathScreen = false;
            });
        }));
    }

    public void Blackout(bool on, float sustainTime = 0.1f, System.Action callback = null)
    {
        _blackoutTarget = on ? 1f : 0f;
        _blackoutTransitioning = true;
        _blackoutTransitioningTime = sustainTime;
        _blackoutAction = callback;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= this.OnSceneLoaded;

        if (scene.name == _gameSceneName)
        {
            RoundManager.Instance.ResetRounds();

            StartCoroutine(this.Delay(3f, () =>
            {
                RoundManager.Instance.NextRound();
            }));
        }
    }

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
