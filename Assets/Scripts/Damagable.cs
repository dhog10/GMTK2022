using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Friendly,
    Enemy,
}

public class Damagable : MonoBehaviour
{
    [SerializeField] private Team _team;

    public Team Team
        => _team;
}
