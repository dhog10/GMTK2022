using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PodiumDiceSpot : MonoBehaviour
{
    [SerializeField]
    private int _number;

    public int Number
        => _number;
}
