using UnityEngine;

public class Health : MonoBehaviour
{
    private UnitStats stats;

    [Header("Прочность")]
    public float maxHealth = 100f; 
    public float currentHealth;

    [Header("UI")]
    public GameObject healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    private float regenTimer = 0f;

    void Start()
    {
        stats = GetComponent<UnitStats>();

        if (stats != null) maxHealth = stats.maxHealth;
        if (currentHealth == 0) currentHealth = maxHealth;

        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab, this.transform);
            healthBar = hbObj.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.target = this.transform;
                healthBar.UpdateHealthBar(currentHealth, maxHealth);
            }
        }
    }

    void Update()
    {
        if (stats != null && stats.canRegen && currentHealth < maxHealth && currentHealth > 0)
        {
            regenTimer += Time.deltaTime;
            
            if (regenTimer >= stats.regenTickRate)
            {
                float healAmount = maxHealth * (stats.regenPercentPerTick / 100f);
                Heal(healAmount);
                regenTimer = 0f;
            }
        }
    }

    public void Heal(float amount)
    {
        if (currentHealth <= 0) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount, Fraction attackerFraction)
    {
        float finalDamage = amount;

        if (stats != null)
        {
            if (attackerFraction == Fraction.Mages && stats.unitFraction == Fraction.People)
                finalDamage *= 1.5f;
            if (attackerFraction == Fraction.People && stats.unitFraction == Fraction.Robots)
                finalDamage *= 1.5f;
            if (attackerFraction == Fraction.Robots && stats.unitFraction == Fraction.Mages)
                finalDamage *= 1.5f;
        }

        currentHealth -= finalDamage;

        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
        
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (healthBar != null) Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}