using UnityEngine;
using UnityEngine.AI;

public class UnitMover : MonoBehaviour
{
    private NavMeshAgent agent;
    private Health targetEnemy;

    public float attackRange = 2f;
    public float attackDamage = 10f;
    public float attackSpeed = 1f;
    private float nextAttackTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (targetEnemy != null)
        {
            float distance = Vector3.Distance(transform.position, targetEnemy.transform.position);

            if (distance <= attackRange)
            {
                agent.isStopped = true;
                Attack();
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(targetEnemy.transform.position);
            }
        }
    }

    public void SetSelected(bool isSelected)
    {
        Debug.Log(gameObject.name + " выделен: " + isSelected);
    }

    public void MoveTo(Vector3 point)
    {
        targetEnemy = null;
        agent.isStopped = false;
        agent.SetDestination(point);
    }

    public void SetTarget(Health enemy)
    {
        targetEnemy = enemy;
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Debug.Log("ѕытаюсь ударить " + targetEnemy.name);
            targetEnemy.TakeDamage(attackDamage);
            nextAttackTime = Time.time + attackSpeed;
        }
    }
}