using UnityEngine;
using UnityEngine.AI;

public class UnitMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private Health targetEnemy;
    private Renderer unitRenderer;
    private Color originalColor;
    private UnitInventory inventory;
    private ResourceSource targetResource;
    private float gatherTimer;

    [Header("Combat Settings")]
    public float attackRange = 10f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    private float nextAttackTime;
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Gathering Settings")]
    public float gatherRange = 2.5f;
    public float gatherCooldown = 1.5f;

    [Header("Selection")]
    public Color selectedColor = Color.green;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unitRenderer = GetComponent<Renderer>();
        inventory = GetComponent<UnitInventory>();

        if (unitRenderer != null)
        {
            originalColor = unitRenderer.material.color;
        }
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            if (targetEnemy == null || targetEnemy.gameObject == null)
            {
                targetEnemy = null;
                agent.isStopped = false;
                return;
            }

            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= attackRange)
            {
                Vector3 direction = (targetEnemy.transform.position - firePoint.position).normalized;
                RaycastHit hit;
                if (Physics.Raycast(firePoint.position, direction, out hit, attackRange))
                {
                    if (hit.transform.root == targetEnemy.transform.root)
                    {
                        agent.isStopped = true;

                        if (Time.time >= nextAttackTime)
                        {
                            Shoot();
                            nextAttackTime = Time.time + attackSpeed;
                        }
                    }
                    else
                    {
                        agent.isStopped = false;
                        agent.SetDestination(targetEnemy.transform.position);
                    }
                }
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
        else if (targetResource != null)
        {
            HandleResourceGathering();
        }
    }

    void HandleResourceGathering()
    {
        if (inventory != null && inventory.IsFull)
        {
            targetResource = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, targetResource.transform.position);
        if (distance <= gatherRange)
        {
            agent.isStopped = true;
            gatherTimer += Time.deltaTime;

            if (gatherTimer >= gatherCooldown)
            {
                int amount = targetResource.Gather(5);
                if (inventory != null)
                {
                    inventory.AddResource(targetResource.type, amount);
                }
                gatherTimer = 0;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(targetResource.transform.position);
        }
    }

    public void SetSelected(bool isSelected)
    {
        if (unitRenderer != null)
        {
            unitRenderer.material.color = isSelected ? selectedColor : originalColor;
        }
    }

    public void MoveTo(Vector3 point)
    {
        targetEnemy = null;
        targetResource = null;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void SetTarget(Health enemy)
    {
        targetResource = null;
        targetEnemy = enemy;
    }

    public void SetResourceTarget(ResourceSource resource)
    {
        targetEnemy = null;
        targetResource = resource;
        agent.SetDestination(resource.transform.position);
    }

    void Shoot()
    {
        if (targetEnemy == null || bulletPrefab == null || firePoint == null) return;

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = bulletObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            Fraction myFraction = GetComponent<Health>().unitFraction;
            projectile.Setup(targetEnemy, attackDamage, myFraction);
        }
    }
}