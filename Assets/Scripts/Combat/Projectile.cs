using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 40f;
    private float damage;
    private EntityStats attackerStats;
    private float armorPen;
    private bool isCritHit; 
    
    private Vector3 flyDirection;

    public void Setup(Vector3 direction, float bulletDamage, EntityStats attacker, float penetration, bool isCrit = false)
    {
        flyDirection = direction;
        damage = bulletDamage;
        attackerStats = attacker;
        armorPen = penetration;
        isCritHit = isCrit;

        transform.rotation = Quaternion.LookRotation(flyDirection);

        Destroy(gameObject, 3f);
    }

    void Update()
    {
        float moveDistance = speed * Time.deltaTime;

        if (Physics.Raycast(transform.position, flyDirection, out RaycastHit hit, moveDistance))
        {
            int hitLayer = hit.collider.gameObject.layer;
            
            if (hitLayer == LayerMask.NameToLayer("Unit") || hitLayer == LayerMask.NameToLayer("Vehicle") || hitLayer == LayerMask.NameToLayer("Building"))
            {
                Health targetHealth = hit.collider.GetComponentInParent<Health>();
                if (targetHealth != null)
                {
                    targetHealth.TakeDamage(damage, attackerStats, armorPen, isCritHit);
                }
                Destroy(gameObject);
                return;
            }
            else if (hit.collider.GetComponentInParent<LootBox>() == null)
            {
                Destroy(gameObject); 
                return;
            }
        }

        transform.position += flyDirection * moveDistance;
    }
}