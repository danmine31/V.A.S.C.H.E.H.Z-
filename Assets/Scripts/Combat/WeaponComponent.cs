using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    [Header("Характеристики оружия")]
    public float minDamage = 8f;
    public float maxDamage = 12f;
    public float critChance = 15f; 
    public float critMultiplier = 2.0f;
    public float armorPenetration = 0f;
    public float attackSpeed = 1f;
    public float attackRange = 10f;

    [Header("Точность и Разброс")]
    [Tooltip("0 = летит идеально прямо. Чем больше, тем выше шанс промаха")]
    public float spreadAngle = 2f; 
    
    [HideInInspector] public float damageMultiplier = 1f;

    [Header("Ссылки для стрельбы")]
    [Tooltip("Если пустой (null), юнит будет бить в ближнем бою (рукопашная)")]
    public GameObject bulletPrefab; 
    public Transform firePoint;
    
    public AudioClip shootSound;
    private AudioSource audioSource;
    private float nextAttackTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public bool CanAttack()
    {
        return Time.time >= nextAttackTime;
    }

    public void Fire(Health target, EntityStats attackerStats)
    {
        if (!CanAttack() || target == null) return;

        if (attackerStats.ownerID == 1)
        {
            UnitInventory inv = GetComponent<UnitInventory>();
            if (inv != null)
            {
                if (inv.GetItemCount(ItemType.Ammo) > 0) inv.RemoveItem(ItemType.Ammo, 1);
                else return;
            }
        }

        float roll = Random.Range(minDamage, maxDamage);
        bool isCrit = Random.Range(0f, 100f) <= critChance;
        float finalDamage = (isCrit ? roll * critMultiplier : roll) * damageMultiplier;

        if (isCrit) Debug.Log($"<color=red>КРИТ! {attackerStats.entityName} бьет на {finalDamage}!</color>");

        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Projectile projectile = bulletObj.GetComponent<Projectile>();

            if (projectile != null)
            {
                Vector3 trueDirection = (target.transform.position + Vector3.up * 1f - firePoint.position).normalized;
                
                float randomX = Random.Range(-spreadAngle, spreadAngle);
                float randomY = Random.Range(-spreadAngle, spreadAngle);
                Vector3 spreadDirection = Quaternion.Euler(randomX, randomY, 0) * trueDirection;

                projectile.Setup(spreadDirection, finalDamage, attackerStats, armorPenetration, isCrit);
            }
        }
        else
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= attackRange)
            {
                target.TakeDamage(finalDamage, attackerStats, armorPenetration, isCrit);
            }
        }

        if (shootSound != null && audioSource != null) audioSource.PlayOneShot(shootSound);

        nextAttackTime = Time.time + attackSpeed;
    }
}