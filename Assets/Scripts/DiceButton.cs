using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DiceButton : MonoBehaviour
{
    [SerializeField]
    private UnityEvent _onClick;

    private float _selectPercent;

    private void Start()
    {

    }

    public bool Hovered { get; private set; }

    private void Update()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        this.Hovered = false;
        if (Physics.Raycast(ray, out var result))
        {
            this.Hovered = result.transform == this.transform;
        }

        _selectPercent += ((this.Hovered ? 1f : 0f) - _selectPercent) * Time.deltaTime * 5f;

        this.transform.localScale = Vector3.one * (1f + _selectPercent * 0.25f);

        if (this.Hovered && Input.GetMouseButtonDown(0))
        {
            _onClick?.Invoke();
        }
    }
}
