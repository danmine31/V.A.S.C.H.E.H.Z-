using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public HealthBar healthBarPrefab;
    public HealthBar healthBarInstance;
    public Vector3 healthBarOffset = new Vector3(0, 2.5f, 0);

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(healthBarPrefab);
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                healthBarInstance.transform.SetParent(canvas.transform, false);
            }
            healthBarInstance.target = transform;
            healthBarInstance.offset = healthBarOffset;
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - amount);
        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        if (healthBarInstance != null)
        {
            healthBarInstance.UpdateHealthBar(currentHealth, maxHealth);
        }
    }

    void Die()
    {
        if (healthBarInstance != null && healthBarInstance.gameObject != null)
        {
            Destroy(healthBarInstance.gameObject);
        }
        Destroy(gameObject);
    }
}