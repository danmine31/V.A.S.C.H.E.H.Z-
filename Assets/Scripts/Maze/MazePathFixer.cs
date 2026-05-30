using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MazePathFixer : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 lastDestination;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (agent.hasPath && agent.destination != lastDestination)
        {
            NavMeshHit hit;
            
            if (!NavMesh.SamplePosition(agent.destination, out hit, 0.1f, NavMesh.AllAreas))
            {
                if (NavMesh.SamplePosition(agent.destination, out hit, 3f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
            
            lastDestination = agent.destination; 
        }
    }
}