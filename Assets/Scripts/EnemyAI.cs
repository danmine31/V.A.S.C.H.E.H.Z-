using UnityEngine;
using UnityEngine.AI;

public enum AIBehavior { Passive, Patrol, Defend, Aggressive }

public class EnemyAI : MonoBehaviour
{
    [Header("Поведение")]
    public AIBehavior currentBehavior = AIBehavior.Defend;
    
    [Header("Настройки радиусов")]
    public float attackRange = 10f;
    public float aggroRadius = 15f;
    public float maxChaseDistance = 25f;
    public float patrolRadius = 10f;

    [Header("Стрельба")]
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    private float nextAttackTime;

    private NavMeshAgent agent;
    private Health targetPlayer;
    private Vector3 startPosition;
    private float patrolTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
    }

    void Update()
    {
        switch (currentBehavior)
        {
            case AIBehavior.Passive:
                agent.isStopped = true;
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
        FindClosestPlayer();

        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            float distanceFromBase = Vector3.Distance(startPosition, transform.position);

            if (distanceToPlayer <= aggroRadius && distanceFromBase <= maxChaseDistance)
            {
                AttackPlayer(distanceToPlayer);
            }
            else
            {
                targetPlayer = null; 
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, startPosition) > 1f)
            {
                agent.isStopped = false;
                agent.SetDestination(startPosition);
            }
        }
    }

    void PatrolLogic()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
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
        FindClosestPlayer();
        if (targetPlayer != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            AttackPlayer(distanceToPlayer);
        }
        else PatrolLogic();
    }

    void AttackPlayer(float distance)
    {
        if (distance <= attackRange)
        {
            agent.isStopped = true; 
            
            if (Time.time >= nextAttackTime)
            {
                EnemyShoot();
                nextAttackTime = Time.time + attackSpeed;
            }
        }
        else
        {
            agent.isStopped = false; 
            agent.SetDestination(targetPlayer.transform.position);
        }
    }

    void EnemyShoot()
    {
        if (targetPlayer == null || bulletPrefab == null || firePoint == null) return;

        Vector3 lookDir = targetPlayer.transform.position - transform.position;
        lookDir.y = 0;
        transform.rotation = Quaternion.LookRotation(lookDir);

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Projectile projectile = bulletObj.GetComponent<Projectile>();

        if (projectile != null)
        {
            Fraction myFraction = GetComponent<Health>().unitFraction;
            projectile.Setup(targetPlayer, attackDamage, myFraction);
        }
    }

    void FindClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        float closestDistance = Mathf.Infinity;
        Health closestEnemy = null;

        foreach (GameObject p in players)
        {
            float distance = Vector3.Distance(transform.position, p.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = p.GetComponent<Health>();
            }
        }
        targetPlayer = closestEnemy;
    }
}