using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine;


public class PoliceCar : MonoBehaviour
{
    [SerializeField] private float speed = 5.3f;
    public bool alert = true;
    private NavMeshAgent navMeshAgent;
    private Transform playerTransform;
    private Vector3 currentDirection;
    private float patrolRadius = 20f;
    private float obstacleDetectionDistance = 5f;
    private Vector3 lastDirection;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = speed;
        navMeshAgent.angularSpeed = 300f;
        navMeshAgent.acceleration = 8f;
        navMeshAgent.stoppingDistance = 1f;
        navMeshAgent.autoBraking = false;
        navMeshAgent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        Taxi taxi = FindAnyObjectByType<Taxi>();
        if (taxi != null)
        {
            playerTransform = taxi.transform;
        }
        else
        {
            Debug.LogError("Taxi object not found in the scene.");
        }

        if (!alert)
        {
            SetNewRandomDirection();
        }
    }

    void Update()
    {
        if (alert)
        {
            PredictAndAdjustPath();
            AvoidObstacles();
        }
        else
        {
            PatrolLogic();
        }

        CorrectPositionIfNeeded();
    }

    private void PatrolLogic()
    {
        if (navMeshAgent.remainingDistance < 1f || navMeshAgent.pathPending)
        {
            SetNewRandomDirection();
        }
        else if (Physics.Raycast(transform.position, transform.forward, obstacleDetectionDistance))
        {
            AdjustForObstacle();
        }
    }

    private void SetNewRandomDirection()
    {
        Vector3 randomDirection = GetRandomDirection();
        Vector3 targetPosition = transform.position + randomDirection * patrolRadius;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
            lastDirection = randomDirection;
        }
    }

    private void AdjustForObstacle()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDetectionDistance))
        {
            Vector3 avoidDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
            Vector3 newDestination = transform.position + avoidDirection * patrolRadius;

            NavMeshHit navHit;
            if (NavMesh.SamplePosition(newDestination, out navHit, patrolRadius, NavMesh.AllAreas))
            {
                navMeshAgent.SetDestination(navHit.position);
            }
        }
    }

    private Vector3 GetRandomDirection()
    {
        Vector3 randomDirection = Random.insideUnitSphere;
        randomDirection.y = 0; // Mantenerse en el plano XZ
        return randomDirection.normalized;
    }

    private void PredictAndAdjustPath()
    {
        Vector3 targetPosition = playerTransform.position;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        Vector3 intermediatePosition = transform.position + directionToTarget * patrolRadius * 0.5f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(intermediatePosition, out hit, patrolRadius, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }
    }

    private void AvoidObstacles()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleDetectionDistance))
        {
            Vector3 obstacleNormal = hit.normal;
            Vector3 avoidDirection = Vector3.Cross(obstacleNormal, Vector3.up).normalized;

            Vector3 newDestination = transform.position + avoidDirection * patrolRadius;
            navMeshAgent.SetDestination(newDestination);
        }
    }

    private void CorrectPositionIfNeeded()
    {
        NavMeshHit hit;
        if (!NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            navMeshAgent.Warp(hit.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Taxi"))
        {
            if (alert)
            {
                SceneManager.LoadScene(2);
            }
        }
    }
}