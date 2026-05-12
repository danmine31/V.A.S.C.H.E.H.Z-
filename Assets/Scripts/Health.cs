using UnityEngine;

public enum Fraction { People, Mages, Robots }

public class Health : MonoBehaviour
{
    public Fraction unitFraction;
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;
    public GameObject healthBarPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab);
            healthBar = hbObj.GetComponent<HealthBar>();
            if (healthBar != null)
            {
                healthBar.target = this.transform;
                healthBar.UpdateHealthBar(currentHealth, maxHealth);
            }
        }
    }

    public void TakeDamage(float amount, Fraction attackerFraction)
    {
        float finalDamage = amount;

        if (attackerFraction == Fraction.Mages && unitFraction == Fraction.People)
            finalDamage *= 1.5f;
        if (attackerFraction == Fraction.People && unitFraction == Fraction.Robots)
            finalDamage *= 1.5f;
        if (attackerFraction == Fraction.Robots && unitFraction == Fraction.Mages)
            finalDamage *= 1.5f;

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