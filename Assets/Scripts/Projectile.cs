using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    private float damage;
    private Health target;
    private Fraction attackerFraction;
    private float armorPen;

    public void Setup(Health enemyTarget, float bulletDamage, Fraction fraction, float penetration)
    {
        target = enemyTarget;
        damage = bulletDamage;
        attackerFraction = fraction;
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
            target.TakeDamage(damage, attackerFraction, armorPen);
            Destroy(gameObject);
        }
    }
}