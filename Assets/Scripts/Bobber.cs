using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField]
    private Vector3 _bobAmount = Vector3.up;

    [SerializeField]
    private float _bobSpeed = 1f;

    [SerializeField]
    private bool _randomTimeOffset = true;

    private Vector3 _originalPosition;
    private float _time;

    private void Start()
    {
        _originalPosition = this.transform.localPosition;

        if (_randomTimeOffset)
        {
            _time = Random.Range(0f, 10000f);
        }
    }

    private void Update()
    {
        this.transform.localPosition = _originalPosition + _bobAmount * Mathf.Cos((Time.time + _time) * _bobSpeed);
    }
}
