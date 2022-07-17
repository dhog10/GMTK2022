using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Podium : MonoBehaviour
{
    public static List<Podium> Podiums = new List<Podium>();

    [SerializeField]
    private bool _upgradePodium = true;

    [SerializeField]
    private TextMeshPro _text;

    [SerializeField]
    private Transform _diceSpawn;

    [SerializeField]
    private GameObject _dicePrefab;

    private Vector3 _originalPosition;
    private Vector3 _disabledPosition;
    private PodiumDice _dice;

    private void Start()
    {
        _originalPosition = transform.position;
        _disabledPosition = transform.position + Vector3.up * 20f;

        Podiums.Add(this);

        if (!_upgradePodium)
        {
            this.EnablePodium(false);
        }
        else
        {
            this.SelectRoll();
        }
    }

    public bool PodiumEnabled { get; private set; } = true;

    public bool UpgradePodium
        => _upgradePodium;

    public Stat Stat { get; private set; }

    public void SelectRoll()
    {
        var player = CharacterControl.Instance;
        var stats = _upgradePodium ? player.Stats : player.Stats.Where(s => s.Value > 0).ToArray();
        var upgrade = stats[Random.Range(0, stats.Length)];

        this.Stat = upgrade;

        _text.text = upgrade.Name;
    }

    public void EnablePodium(bool enabled)
    {
        if (_dice != null)
        {
            GameObject.Destroy(_dice.gameObject);
            _dice = null;
        }

        this.PodiumEnabled = enabled;
    }

    public void Roll()
    {
        if (_dice != null)
        {
            return;
        }

        var dice = GameObject.Instantiate(_dicePrefab, _diceSpawn.position, Quaternion.identity);
        _dice = dice.GetComponent<PodiumDice>();
        _dice.transform.rotation = Random.rotation;
        _dice.Rb.AddForce(new Vector3(Random.Range(-0.2f, 0.2f), 1f, Random.Range(-0.2f, 0.2f)).normalized * 8f, ForceMode.Impulse);
        var r = 25f;
        _dice.Rb.angularVelocity = new Vector3(Random.Range(-r, r), Random.Range(-r, r), Random.Range(-r, r));
    }

    public void SelectNumber(int number)
    {
        if (!_upgradePodium)
        {
            number = -number;
        }

        var stat = CharacterControl.Instance.Stats.FirstOrDefault(s => s.Name == this.Stat.Name);
        stat.Value = Mathf.Max(stat.Value + number, 0f);

        StartCoroutine(this.Delay(3f, () =>
        {
            foreach (var podium in Podiums)
            {
                podium.EnablePodium(false);
            }

            if (_upgradePodium)
            {
                foreach (var podium in Podiums.Where(p => !p.UpgradePodium))
                {
                    podium.SelectRoll();
                    podium.EnablePodium(true);
                }
            }
            else
            {
                Stat.ProcessStats();
                RoundManager.Instance.NextRound();
            }
        }));
    }

    private void Update()
    {
        var target = this.PodiumEnabled ? _originalPosition : _disabledPosition;
        this.transform.position += (target - this.transform.position) * Mathf.Min(0.5f, Time.deltaTime * 10f);

        if (_dice != null)
        {
            if (!_dice.IsRolling && !_dice.RollComplete)
            {
                _dice.RollComplete = true;

                var number = _dice.HighestSpot.Number;
                this.SelectNumber(number);
            }
        }
    }

    private void OnDestroy()
    {
        Podiums.Remove(this);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject;

        if (other == CharacterControl.Instance.gameObject)
        {
            this.Roll();
        }
    }

    private IEnumerator Delay(float time, System.Action callback)
    {
        yield return new WaitForSeconds(time);

        callback?.Invoke();
    }
}
