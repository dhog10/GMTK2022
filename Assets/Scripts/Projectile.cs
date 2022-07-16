using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    private float _speed = 30f;

    [SerializeField]
    private float _lifetime = 5f;

    [SerializeField]
    private AudioSource[] _shootAudio;

    private Rigidbody _rb;
    private float _startTime;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();

        _startTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - _startTime > _lifetime)
        {
            GameObject.DestroyImmediate(this.gameObject);
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
}
