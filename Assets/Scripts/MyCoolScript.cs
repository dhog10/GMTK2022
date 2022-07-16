using UnityEngine;
using UnityEngine.AI;

public class MyCoolScript : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float targetDistanceMin;
    [SerializeField] private float targetDistanceMax;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = targetDistanceMax;
    }

    private void Update()
    {
        var playerDistance = Vector3.Distance(player.transform.position, transform.position);

        if (playerDistance < targetDistanceMin)
        {
            var radius = 100.0f;
            var navMeshBounds = GenerateNavMeshBounds();
            if (navMeshBounds != new Bounds())
            {
                radius = navMeshBounds.extents.x;
            }
            var distantPosition = transform.position + (-radius * (player.transform.position - transform.position).normalized);
            if (NavMesh.SamplePosition(distantPosition, out var hit, radius, 1))
            {
                navMeshAgent.SetDestination(hit.position);
            }
        }
        else
        {
            navMeshAgent.SetDestination(player.transform.position);
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
