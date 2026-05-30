using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    private float damage;
    private Health target;
    
    private UnitStats attackerStats;
    private float armorPen;
    private bool isCritHit; 

    public void Setup(Health enemyTarget, float bulletDamage, UnitStats attacker, float penetration, bool isCrit = false)
    {
        target = enemyTarget;
        damage = bulletDamage;
        attackerStats = attacker;
        armorPen = penetration;
        isCritHit = isCrit;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, moveDistance))
        {
            if (hit.collider.gameObject.layer != LayerMask.NameToLayer("Unit"))
            {
                if (hit.collider.GetComponentInParent<LootBox>() == null)
                {
                    Destroy(gameObject); 
                    return;
                }
            }
        }

        transform.position += direction * moveDistance;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            target.TakeDamage(damage, attackerStats, armorPen, isCritHit);
            Destroy(gameObject);
        }
    }
}