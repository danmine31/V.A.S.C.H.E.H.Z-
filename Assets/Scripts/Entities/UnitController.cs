using UnityEngine;
using UnityEngine.AI;

public enum FactionType { None, Human, Mage, Robot }

public class UnitController : EntityController
{
    [Header("Настройки команды")]
    public FactionType faction;
    private NavMeshAgent agent;
    private Renderer unitRenderer;
    private Health targetEnemy;
    private UnitAI autoPilot;
    private UnitInventory inventory;
    private ResourceSource targetResource;
    private float gatherTimer;

    [Header("Gathering Settings")]
    public float gatherRange = 2.5f;
    public float gatherCooldown = 1.5f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        unitRenderer = GetComponentInChildren<Renderer>(); 
        autoPilot = GetComponent<UnitAI>();
        inventory = GetComponent<UnitInventory>();
        
        UnitStats stats = GetComponent<UnitStats>();
        if (stats != null && agent != null)
        {
            agent.speed = stats.moveSpeed;
        }
        if (stats != null && stats.teamID == 1)
        {
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.RegisterPlayerUnit();
            }
        }
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            if (targetEnemy.gameObject == null)
            {
                targetEnemy = null;
                if (agent.hasPath) agent.ResetPath();
                agent.isStopped = true; 
                
                if (autoPilot != null) autoPilot.isManualControl = false;
                return;
            }

            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= autoPilot.attackRange)
            {
                if (!agent.isStopped)
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

                autoPilot.PerformAttack(targetEnemy);
            }
            else if (distance > autoPilot.attackRange + 0.1f) 
            {
                agent.isStopped = false;
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
        else if (targetResource != null)
        {
            HandleResourceGathering();
        }
        else 
        {
            if (autoPilot != null && autoPilot.isManualControl)
            {
                if (!agent.pathPending)
                {
                    bool reachedExactly = agent.remainingDistance <= agent.stoppingDistance;
                    bool stuckInCrowd = agent.remainingDistance <= 2.5f && agent.velocity.sqrMagnitude < 0.1f;

                    if (reachedExactly || stuckInCrowd)
                    {
                        autoPilot.isManualControl = false;
                        agent.isStopped = true;
                        autoPilot.SetBasePosition(transform.position);
                    }
                }
            }
        }
    }

    void HandleResourceGathering()
    {
        if (inventory != null && inventory.IsFull)
        {
            Debug.Log($"<color=yellow>[ДОБЫЧА] Инвентарь юнита {gameObject.name} полон! Добыча остановлена.</color>");
            targetResource = null;
            if (autoPilot != null) autoPilot.isManualControl = false;
            return;
        }

        float distance = Vector3.Distance(transform.position, targetResource.transform.position);
        Health myHealth = GetComponent<Health>();

        if (distance <= gatherRange)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
            }

            gatherTimer += Time.deltaTime;

            if (myHealth != null && myHealth.healthBar != null)
            {
                myHealth.healthBar.UpdateActionBar(gatherTimer / targetResource.gatherTime);
            }

            if (gatherTimer >= targetResource.gatherTime)
            {
                int amount = targetResource.Gather(2);
                if (amount > 0) inventory.AddResource(targetResource.type, amount);
                gatherTimer = 0f;
                if (myHealth != null && myHealth.healthBar != null) myHealth.healthBar.UpdateActionBar(0f);
            }
        }
        else
        {
            gatherTimer = 0f;
            if (myHealth != null && myHealth.healthBar != null) myHealth.healthBar.UpdateActionBar(0f);
            
            agent.isStopped = false;
            agent.SetDestination(targetResource.transform.position);
        }
    }

    public override void MoveTo(Vector3 point)
    {
        if (autoPilot != null) autoPilot.isManualControl = true;
        
        targetEnemy = null;
        targetResource = null;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public override void SetTarget(Health enemy)
    {
        UnitAI ai = GetComponent<UnitAI>();
        if (ai != null && !ai.canAttack)
        {
            Debug.LogWarning($"[{gameObject.name}]: Я -- работяга, атаковать не могу!");
            return;
        }

        if (autoPilot != null) autoPilot.isManualControl = true;

        targetResource = null;
        targetEnemy = enemy;
    }

    public override void SetResourceTarget(ResourceSource resource)
    {
        if (autoPilot != null) autoPilot.isManualControl = true;
        
        targetEnemy = null;
        targetResource = resource;
        agent.SetDestination(resource.transform.position);
    }
}