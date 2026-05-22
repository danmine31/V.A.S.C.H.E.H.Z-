using UnityEngine;

public class ResourceGatherer : MonoBehaviour
{
    private UnitController controller;
    private UnitInventory inventory;
    private ResourceSource targetResource;
    private float timer;

    void Start()
    {
        controller = GetComponent<UnitController>();
        inventory = GetComponent<UnitInventory>();
    }

    public void SetResourceTarget(ResourceSource resource)
    {
        targetResource = resource;
        controller.MoveTo(resource.transform.position);
    }

    void Update()
    {
        if (targetResource == null || inventory.IsFull) return;

        float distance = Vector3.Distance(transform.position, targetResource.transform.position);

        if (distance <= 2.5f) 
        {
            timer += Time.deltaTime;
            if (timer >= targetResource.gatherTime)
            {
                int gathered = targetResource.Gather(5);
                inventory.AddResource(targetResource.type, gathered);
                timer = 0;
            }
        }
    }
}