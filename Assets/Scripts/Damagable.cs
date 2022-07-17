using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Team
{
    None,
    Friendly,
    Enemy,
}

public class Damagable : MonoBehaviour
{
    [SerializeField]
    private float _maxHealth = 100f;

    [SerializeField]
    private Team _team;

    [SerializeField]
    private UnityEvent _onKilled;

    public float Health { get; private set; }

    public float MaxHealth { get; set; }

    public Team Team
        => _team;

    public bool Dead { get; set; }

    private void Awake()
    {
        this.Health = _maxHealth;
        this.MaxHealth = _maxHealth;
    }

    public void SetHealth(float hp)
    {
        hp = Mathf.Clamp(hp, 0f, this.MaxHealth);
        this.Health = hp;

        if (this.Health <= 0f && !this.Dead)
        {
            this.Dead = true;
            _onKilled?.Invoke();
        }
    }

    public void Damage(float amount, Team team = Team.None)
    {
        if (team != Team.None && team == this.Team)
        {
            return;
        }

        this.SetHealth(this.Health - amount);
    }
}
