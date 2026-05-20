using UnityEngine;
using UnityEngine.AI;

public class MazePatrol : MonoBehaviour
{
    [Header("Настройки патруля")]
    public NavMeshAgent agent;
    public float patrolRadius = 20f;
    public float waitTime = 2f;

    private float timer;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        
        SetNewPatrolDestination();
    }

    void Update()
    {
        Health health = GetComponent<Health>();
        if (health != null && health.currentHealth <= 0) return;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                SetNewPatrolDestination();
                timer = 0;
            }
        }
    }

    void SetNewPatrolDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;
        
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }
}