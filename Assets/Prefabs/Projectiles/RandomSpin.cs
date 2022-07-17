using UnityEngine;

public class RandomSpin : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;
    private Vector3 _randomDirection;

    private void Start()
        => _randomDirection = Random.insideUnitCircle.normalized;

    private void Update()
        => transform.Rotate(_randomDirection, _speed);
}
