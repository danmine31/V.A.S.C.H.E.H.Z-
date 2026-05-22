using UnityEngine;
using UnityEngine.AI;

public enum FactionType { Human, Mage, Robot }

public class UnitController : MonoBehaviour
{
    [Header("Настройки команды")]
    public int teamID;
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
        if (teamID == 1 && LevelManager.Instance != null)
        {
            LevelManager.Instance.RegisterPlayerUnit();
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
            targetResource = null;
            if (autoPilot != null) autoPilot.isManualControl = false;
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
                    if (teamID == 1 && targetResource.gameObject.name.ToLower().Contains("artemit"))
                    {
                        if (LevelManager.Instance != null) LevelManager.Instance.GameOverWin();
                    }
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

    public void MoveTo(Vector3 point)
    {
        if (autoPilot != null) autoPilot.isManualControl = true;
        
        targetEnemy = null;
        targetResource = null;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void SetTarget(Health enemy)
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

    public void SetResourceTarget(ResourceSource resource)
    {
        if (autoPilot != null) autoPilot.isManualControl = true;
        
        targetEnemy = null;
        targetResource = resource;
        agent.SetDestination(resource.transform.position);
    }
}