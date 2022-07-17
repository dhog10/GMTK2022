using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PodiumDice : MonoBehaviour
{
    [SerializeField]
    private PodiumDiceSpot[] _spots;

    private Rigidbody _rb;

    private float _spawnTime;

    public PodiumDiceSpot HighestSpot
        => _spots.OrderByDescending(s => s.transform.position.y).FirstOrDefault();

    public bool IsRolling
        => Time.time - _spawnTime < 1f || this.Rb.velocity.magnitude > 0.1f || this.Rb.angularVelocity.magnitude > 0.1f;

    public bool RollComplete { get; set; }

    public Rigidbody Rb
        => _rb;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody>();
        _spawnTime = Time.time;
    }
}
