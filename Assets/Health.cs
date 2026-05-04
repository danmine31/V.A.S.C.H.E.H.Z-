using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (healthBar != null) healthBar.UpdateHealthBar(currentHealth, maxHealth);

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        if (healthBar != null) Destroy(healthBar.gameObject);
        Destroy(gameObject);
    }
}