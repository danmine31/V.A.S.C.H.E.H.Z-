using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    private float damage;
    private Health target;
    
    private UnitStats attackerStats;
    private float armorPen;

    public void Setup(Health enemyTarget, float bulletDamage, UnitStats attacker, float penetration)
    {
        target = enemyTarget;
        damage = bulletDamage;
        attackerStats = attacker;
        armorPen = penetration;
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            target.TakeDamage(damage, attackerStats, armorPen);
            Destroy(gameObject);
        }
    }
}