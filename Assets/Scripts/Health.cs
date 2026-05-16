using UnityEngine;

public enum Fraction { People, Mages, Robots }

public class Health : MonoBehaviour
{
    public Fraction unitFraction;
    public int teamID = 0;
    public int colorID = 0;
    public float maxHealth = 100f;
    public float currentHealth;
    public GameObject healthBarPrefab;
    [HideInInspector] public HealthBar healthBar;

    [Header("Цвета команд")]
    public Material neutralMaterial;
    public Material playerMaterial;
    public Material enemyMaterial;
    public Material allyMaterial;

    void Start()
    {
        currentHealth = maxHealth;

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

        ApplyTeamColor();
    }

    #if UNITY_EDITOR 
        private void OnValidate()
        {
            if (!Application.isPlaying) 
            {
                ApplyTeamColor();
            }
        }
    #endif

    void ApplyTeamColor()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            if (colorID == 0 && neutralMaterial != null) 
                rend.material = neutralMaterial; 
                
            else if (colorID == 1 && playerMaterial != null) 
                rend.material = playerMaterial;
                
            else if (colorID == 2 && enemyMaterial != null) 
                rend.material = enemyMaterial;
                
            else if (colorID == 3 && allyMaterial != null) 
                rend.material = allyMaterial;
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