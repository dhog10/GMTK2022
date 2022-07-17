using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : DiceCharacter
{
    private NavMeshAgent _agent;

    public override float MaxSpeed
        => _agent.speed;

    protected override void Awake()
    {
        base.Awake();

        _agent = this.GetComponent<NavMeshAgent>();
    }

    public void Die()
    {
        GameObject.Destroy(this.gameObject);
    }
}
