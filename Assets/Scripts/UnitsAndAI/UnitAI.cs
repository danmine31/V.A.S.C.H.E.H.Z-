using UnityEngine;
using UnityEngine.AI;

public enum AIBehavior { Passive, Patrol, Defend, Aggressive }

public class UnitAI : MonoBehaviour
{
    [Header("Звуки")]
    public AudioClip shootSound;
    private AudioSource audioSource;

    [Header("Для Зданий (Вращение пушки)")]
    public Transform turretModel;

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
    [Header("Принадлежность")]
    public int teamID;
    public FactionType faction;

    [Header("Характеристики")]
    public float baseDamage = 10f;
    public float currentDamage;

    void OnEnable()
    {
        EnvironmentManager.OnTimeChanged += ApplyWeatherBuffs;
    }
    void OnDisable()
    {
        EnvironmentManager.OnTimeChanged -= ApplyWeatherBuffs;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        agent = GetComponent<NavMeshAgent>();
        myHealth = GetComponent<Health>();
        myStats = GetComponent<UnitStats>();
        currentDamage = baseDamage;

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
                if (agent != null && agent.isOnNavMesh) agent.isStopped = true;
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
                if (agent != null && agent.isOnNavMesh)
                {
                    agent.isStopped = false;
                    agent.SetDestination(startPosition);
                }
            }
        }
        else if (Vector3.Distance(transform.position, startPosition) > 1f)
        {
            if (agent != null && agent.isOnNavMesh)
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
        if (distance <= attackRange && CanSeeTarget(targetEnemy))
        {
            if (agent != null && agent.isOnNavMesh && !agent.isStopped)
            {
                agent.isStopped = true;
                if (agent.hasPath) agent.ResetPath();
                agent.velocity = Vector3.zero;
            }

            Vector3 lookDir = targetEnemy.transform.position - transform.position;
            lookDir.y = 0;
            if (lookDir != Vector3.zero)
            {
                Transform thingToRotate = turretModel != null ? turretModel : transform;
                thingToRotate.rotation = Quaternion.Slerp(thingToRotate.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
            }
            
            PerformAttack(targetEnemy); 
        }
        else 
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = false; 
                agent.SetDestination(targetEnemy.transform.position);
            }
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
                projectile.Setup(target, finalDamage, myStats, myStats.armorPenetration, isCrit);
            }

            if (shootSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(shootSound);
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
                if (myStats.teamID == 0 || targetStats.teamID == 0) continue; 
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

    void ApplyWeatherBuffs(TimeOfDay time)
    {
        currentDamage = baseDamage;

        if (teamID == 1 && time == TimeOfDay.Night)
        {
            currentDamage *= 1.5f; 
            Debug.Log("Маги усилены ночью!");
        }
        else if (teamID == 2 && time == TimeOfDay.Day)
        {
            currentDamage *= 1.5f;
            Debug.Log("Роботы усилены днём!");
        }
    }

    bool CanSeeTarget(Health target)
    {
        if (target == null) return false;
        
        Vector3 start = transform.position + Vector3.up * 1f;
        Vector3 end = target.transform.position + Vector3.up * 1f;
        Vector3 dir = end - start;
        
        if (Physics.Raycast(start, dir.normalized, out RaycastHit hit, dir.magnitude))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Unit") && 
                hit.collider.gameObject.layer != LayerMask.NameToLayer("Ground"))
            {
                return false;
            }
        }
        return true;
    }
}