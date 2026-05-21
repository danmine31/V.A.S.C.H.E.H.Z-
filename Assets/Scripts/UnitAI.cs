using UnityEngine;
using UnityEngine.AI;

public enum AIBehavior { Passive, Patrol, Defend, Aggressive }

public class UnitAI : MonoBehaviour
{
    [Header("Поведение")]
    public AIBehavior currentBehavior = AIBehavior.Defend;
    public bool canAttack = true;
    
    [Header("Настройки радиусов")]
    public float attackRange = 10f;
    public float aggroRadius = 15f;
    public float maxChaseDistance = 25f;
    public float patrolRadius = 10f;

    [Header("Стрельба")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float nextAttackTime;

    [HideInInspector] public bool isManualControl = false; 

    private NavMeshAgent agent;
    private Health targetEnemy;
    private Vector3 startPosition;
    private float patrolTimer;
    
    private Health myHealth;
    private UnitStats myStats;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<UnitStats>();

        if (myStats != null && agent != null)
        {
            agent.speed = myStats.moveSpeed;
        }
        
        startPosition = transform.position;
        RadiusVisualizer visualizer = GetComponent<RadiusVisualizer>();
        if (visualizer != null)
        {
            visualizer.AddRadius(aggroRadius, new Color(1f, 0.5f, 0f, 0.6f), "AggroCircle");
            visualizer.AddRadius(attackRange, new Color(1f, 0f, 0f, 0.8f), "AttackCircle");
        }
    }

    void Update()
    {
        if (isManualControl || !canAttack) return;

        switch (currentBehavior)
        {
            case AIBehavior.Passive:
                if (agent.isOnNavMesh) agent.isStopped = true;
                break;
            case AIBehavior.Patrol:
                PatrolLogic();
                break;
            case AIBehavior.Defend:
                DefendLogic();
                break;
            case AIBehavior.Aggressive:
                AggressiveLogic();
                break;
        }
    }

    void DefendLogic()
    {
        FindClosestEnemy();

        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            float distanceFromBase = Vector3.Distance(startPosition, transform.position);

            if (distanceToEnemy <= aggroRadius && distanceFromBase <= maxChaseDistance)
            {
                AttackEnemy(distanceToEnemy);
            }
            else
            {
                targetEnemy = null; 
                if (agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(startPosition);
                }
            }
        }
        else if (Vector3.Distance(transform.position, startPosition) > 1f)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
        }
    }

    void PatrolLogic()
    {
        if (agent.isOnNavMesh && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            patrolTimer += Time.deltaTime;
            if (patrolTimer > 2f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
                randomDirection += startPosition;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
                {
                    agent.isStopped = false;
                    agent.SetDestination(hit.position);
                }
                patrolTimer = 0;
            }
        }
    }

    void AggressiveLogic()
    {
        FindClosestEnemy();
        if (targetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, targetEnemy.transform.position);
            AttackEnemy(distanceToEnemy);
        }
        else PatrolLogic();
    }

    void AttackEnemy(float distance)
    {
        if (distance <= attackRange)
        {
            if (agent.isOnNavMesh && !agent.isStopped)
            {
                agent.isStopped = true;
                if (agent.hasPath) agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            Vector3 lookDir = targetEnemy.transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }
            
            PerformAttack(targetEnemy); 
        }
        else if (agent.isOnNavMesh)
        {
            agent.isStopped = false; 
            agent.SetDestination(targetEnemy.transform.position);
        }
    }

    public void PerformAttack(Health target)
    {
        if (target == null || myStats == null) return;

        if (Time.time >= nextAttackTime)
        {
            if (myStats.ownerID == 1)
            {
                UnitInventory inventory = GetComponent<UnitInventory>();
                if (inventory != null)
                {
                    if (inventory.GetItemCount(ItemType.Ammo) > 0)
                    {
                        inventory.RemoveItem(ItemType.Ammo, 1);
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (bulletPrefab == null || firePoint == null) return;

            float roll = Random.Range(myStats.minDamage, myStats.maxDamage);
            bool isCrit = Random.Range(0f, 100f) <= myStats.critChance;
            float finalDamage = isCrit ? roll * myStats.critMultiplier : roll;

            if (isCrit) Debug.Log($"<color=red>КРИТ! {myStats.unitName} бьет на {finalDamage} урона!</color>");

            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = bulletObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                projectile.Setup(target, finalDamage, myStats, myStats.armorPenetration);
            }
            nextAttackTime = Time.time + myStats.attackSpeed;
        }
    }

    void FindClosestEnemy()
    {
        Health[] allUnits = FindObjectsByType<Health>(FindObjectsInactive.Exclude);
        float closestDistance = Mathf.Infinity;
        Health closestTarget = null;

        foreach (Health unitHealth in allUnits)
        {
            if (unitHealth == null || unitHealth == myHealth) continue; 

            UnitStats targetStats = unitHealth.GetComponent<UnitStats>();
            
            if (myStats != null && targetStats != null)
            {
                if (targetStats.teamID == myStats.teamID) continue; 
            }

            float distance = Vector3.Distance(transform.position, unitHealth.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = unitHealth;
            }
        }
        targetEnemy = closestTarget;
    }

    public void SetBasePosition(Vector3 newPos)
    {
        startPosition = newPos;
    }
}