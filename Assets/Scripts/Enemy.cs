using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : DiceCharacter
{
    [SerializeField]
    private List<DiceGun> _guns = new List<DiceGun>();

    [SerializeField]
    private Vector2 _gunsOrbitDistance = new Vector2(1f, 1.2f);

    [SerializeField]
    private Vector2 _gunsOrbitHeight = new Vector2(0.8f, 1f);

    [SerializeField]
    private GameObject _gunsObject;

    [SerializeField] private GameObject _deathParticles;
    private NavMeshAgent _agent;

    public override float MaxSpeed
        => _agent.speed;

    public float GunOrbitDistance
        => Mathf.Lerp(_gunsOrbitDistance.x, _gunsOrbitDistance.y, this.SpeedP);

    public float GunOrbitHeight
        => Mathf.Lerp(_gunsOrbitHeight.x, _gunsOrbitHeight.y, this.SpeedP);

    private int _seed;

    protected override void Awake()
    {
        base.Awake();

        _agent = this.GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();

        _seed = Random.Range(0, 10000);

        for (var i = 0; i < _gunsObject.transform.childCount; i++)
        {
            var child = _gunsObject.transform.GetChild(i);
            var gun = child.GetComponent<DiceGun>();

            if (gun != null && !_guns.Contains(gun))
            {
                _guns.Add(gun);
            }
        }

        foreach (var gun in _guns)
        {
            if (gun is AIGun aiGun)
            {
                aiGun.Enemy = this;
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        this.UpdateGuns();
    }

    public bool IsInputFiring()
    {
        var seed = Random.Range(0, 1000000);

        Random.InitState(_seed);

        var t = Time.time + Random.Range(0f, 100f);
        Random.InitState(seed);

        if (t % 5 < 2)
        {
            return true;
        }

        return false;
    }

    public void Die()
    {
        if (_deathParticles != null)
        {
            Instantiate(_deathParticles, _visualCharacter.transform.position, _visualCharacter.transform.rotation);
        }

        GameObject.Destroy(this.gameObject);
    }

    private void UpdateGuns()
    {
        var player = CharacterControl.Instance;
        if (player == null)
        {
            return;
        }

        foreach (var gun in _guns)
        {
            var orbitOrigin = _visualObject.transform.position + Vector3.up * this.GunOrbitHeight;
            var orbitDistance = this.GunOrbitDistance + gun.OrbitDistanceAdd;
            var t = Time.time;

            var toGun = (gun.transform.position - orbitOrigin).normalized;
            var rotation = Quaternion.LookRotation(toGun, Vector3.up);
            var targetRotation = Quaternion.LookRotation(new Vector3(Mathf.Cos(t), 0f, Mathf.Sin(t)), Vector3.up);
            rotation = Quaternion.Lerp(rotation, targetRotation, Time.deltaTime * 5f);

            var fwd = (rotation * Vector3.forward);
            gun.transform.position = orbitOrigin + fwd * orbitDistance;

            gun.AimDirection = (player.VisualCharacter.transform.position - gun.transform.position).normalized;
        }
    }
}
