using UnityEngine;

public class UnitStats : EntityStats
{
    [Header("Перемещение")]
    public float moveSpeed = 5f;

    [Header("Защита Юнита")]
    public float dodgeChance = 10f;

    [Header("Регенерация")]
    public bool canRegen = true;
    public float regenTickRate = 2f;
    public float regenPercentPerTick = 1f;

    [Header("Прокачка")]
    public int level = 1;
    public float currentXP = 0f;

    void Awake()
    {
        Health healthComponent = GetComponent<Health>();
        if (healthComponent != null)
        {
            healthComponent.maxHealth = this.maxHealth;
            if (healthComponent.currentHealth == 0)
            {
                healthComponent.currentHealth = this.maxHealth;
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public void AddXP(float amount)
    {
        currentXP += amount;
        float xpNeeded = GetXPForNextLevel();
        
        while (currentXP >= xpNeeded)
        {
            currentXP -= xpNeeded;
            LevelUp();
            xpNeeded = GetXPForNextLevel();
        }
    }

    public float GetXPForNextLevel() => 100f * Mathf.Pow(1.5f, level - 1);

    void LevelUp()
    {
        level++;
        maxHealth += 10f;

        Health h = GetComponent<Health>();
        if (h != null)
        {
            h.maxHealth = maxHealth;
            h.Heal(10f);
            if (h.healthBar != null) h.healthBar.UpdateLevelText(level); 
        }

        WeaponComponent weapon = GetComponent<WeaponComponent>();
        if (weapon != null)
        {
            weapon.minDamage += 1f;
            weapon.maxDamage += 2f;
        }

        if (level % 3 == 0)
        {
            armor += 1f;
            if (weapon != null) weapon.armorPenetration += 1f;
            Debug.Log($"<color=gold>{entityName} получил +1 к Броне и Пробитию за {level} уровень!</color>");
        }
    }
}