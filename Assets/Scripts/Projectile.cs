using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _damage;

    [SerializeField]
    private float _speed = 30f;

    [SerializeField]
    private float _lifetime = 5f;

    [SerializeField]
    private AudioSource[] _shootAudio;

    [SerializeField]
    private GameObject _trail;

    private Rigidbody _rb;
    private float _startTime;

    public Team Team { get; set; }

    public float Damage { get; set; }

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();

        _startTime = Time.time;
        this.Damage = _damage;
    }

    private void Update()
    {
        if (Time.time - _startTime > _lifetime)
        {
            this.DestroyProjectile();
            return;
        }
    }

    public virtual void Shoot(Vector3 direction, float speedOverride = 0f)
    {
        var speed = speedOverride > 0f ? speedOverride : _speed;
        _rb.velocity = direction.normalized * speed;

        if (_shootAudio != null)
        {
            foreach (var source in _shootAudio)
            {
                source.Play();
            }
        }
    }

    public void DestroyProjectile()
    {
        _trail.transform.parent = null;
        GameObject.Destroy(this.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        var other = collision.gameObject;
        var damagable = other.GetComponent<Damagable>() ?? other.GetComponentInChildren<Damagable>();

        if (damagable != null)
        {
            damagable.Damage(this.Damage, this.Team);
        }

        this.DestroyProjectile();
    }
}
