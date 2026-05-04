using UnityEngine;
using UnityEngine.AI;

public class UnitMover : MonoBehaviour
{
    private NavMeshAgent agent;
    public bool isSelected = false;
    private Renderer unitRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unitRenderer = GetComponent<Renderer>();
    }

    public void SetSelected(bool status)
    {
        isSelected = status;
        if (unitRenderer != null)
            unitRenderer.material.color = isSelected ? Color.green : Color.white;
    }

    public void MoveTo(Vector3 destination)
    {
        if (agent != null) agent.SetDestination(destination);
    }
}