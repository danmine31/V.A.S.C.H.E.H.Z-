using UnityEngine;
using UnityEngine.AI;

public class UnitMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private bool isSelected = false;
    private Renderer unitRenderer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unitRenderer = GetComponent<Renderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == this.transform)
                {
                    isSelected = true;
                    unitRenderer.material.color = Color.green;
                }
                else
                {
                    isSelected = false;
                    unitRenderer.material.color = Color.white;
                }
            }
        }

        if (Input.GetMouseButtonDown(1) && isSelected) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }
}
