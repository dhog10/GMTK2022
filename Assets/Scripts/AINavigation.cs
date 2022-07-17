using UnityEngine;
using UnityEngine.AI;

public class AINavigation : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private float _targetDistanceMin;
    [SerializeField] private float _targetDistanceMax;
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.stoppingDistance = _targetDistanceMax;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = Object.FindObjectOfType<CharacterControl>()?.gameObject;
        }

        if (_player == null)
        {
            return;
        }

        var playerDistance = Vector3.Distance(_player.transform.position, transform.position);

        if (playerDistance < _targetDistanceMin)
        {
            var radius = 100.0f;
            var navMeshBounds = GenerateNavMeshBounds();
            if (navMeshBounds != new Bounds())
            {
                radius = navMeshBounds.extents.x;
            }
            var distantPosition = transform.position + (-radius * (_player.transform.position - transform.position).normalized);
            if (NavMesh.SamplePosition(distantPosition, out var hit, radius, 1))
            {
                _navMeshAgent.SetDestination(hit.position);
            }
        }
        else
        {
            _navMeshAgent.SetDestination(_player.transform.position);
        }
    }

    private static Bounds GenerateNavMeshBounds()
    {
        var navMeshVertices = NavMesh.CalculateTriangulation().vertices;

        if (navMeshVertices.Length == 0)
        {
            Debug.LogWarning("NavMesh bounds could not be generated");
            return new Bounds();
        }

        var bounds = new Bounds(navMeshVertices[0], Vector3.zero);
        foreach (var vertex in navMeshVertices)
        {
            bounds.Encapsulate(vertex);
        }

        return bounds;
    }
}
